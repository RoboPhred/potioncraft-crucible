// <copyright file="CruciblePotionEffect.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using global::PotionCraft.LocalizationSystem;
    using global::PotionCraft.ObjectBased.RecipeMap;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionEffectMapItem;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionEffectMapItem.Settings;
    using global::PotionCraft.ScriptableObjects;
    using HarmonyLib;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="PotionEffect"/>s.
    /// </summary>
    public sealed class CruciblePotionEffect : IEquatable<CruciblePotionEffect>
    {
        private static readonly Dictionary<PotionEffect, PotionEffectSettings> EffectSettings = new();
        private static readonly HashSet<Icon> ClonedLockIcons = new();

        private static readonly HashSet<PotionEffect> AtlasOverriddenPotionEffects = new();
        private static CrucibleSpriteAtlas spriteAtlas;

        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePotionEffect"/> class.
        /// </summary>
        /// <param name="potionEffect">The potion effect to wrap.</param>
        internal CruciblePotionEffect(PotionEffect potionEffect)
        {
            this.PotionEffect = potionEffect;
        }

        /// <summary>
        /// Gets the ID of this effect.
        /// </summary>
        public string ID
        {
            get
            {
                return this.PotionEffect.name;
            }
        }

        /// <summary>
        /// Gets or sets the name of the potion in the user's current language.
        /// </summary>
        public string Name
        {
            get
            {
                return new Key($"effect_{this.PotionEffect.name}").GetText();
            }

            set
            {
                CrucibleLocalization.SetLocalizationKey($"effect_{this.PotionEffect.name}", value);
            }
        }

        /// <summary>
        /// Gets the <see cref="PotionEffect"/> this api object is configuring.
        /// </summary>
        public PotionEffect PotionEffect
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="PotionEffectSettings"/> for this effect.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This data is not normally associated with a <see cref="PotionEffect"/>, but instead
        /// is stored on a <see cref="PotionEffectMapItem"/> component in the potion base map.
        /// </p>
        /// <p>
        /// When creating a custom potion effect, a <see cref="PotionEffectSettings"/> will be created and managed by Crucible.
        /// </p>
        /// <p>
        /// When referencing a base game potion effect, the <see cref="PotionEffectSettings"/> will be captured from the potion effect map.
        /// If you are receiving a null value on base game effects, make sure another mod is not clearing out the requested potion effect, or try
        /// capturing a reference to the <see cref="CruciblePotionEffect"/> earlier than the mod clearing out effects.
        /// </p>
        /// </remarks>
        public PotionEffectSettings PotionEffectSettings
        {
            get
            {
                if (!EffectSettings.ContainsKey(this.PotionEffect))
                {
                    TryResolveSettingsFromMap(this.PotionEffect);
                }

                return EffectSettings[this.PotionEffect];
            }
        }

        /// <summary>
        /// Gets or sets the texture of the icon representing this effect.
        /// </summary>
        public Texture2D IconTexture
        {
            get
            {
                return this.PotionEffect.icon.textures[0];
            }

            set
            {
                new CrucibleIcon(this.PotionEffect.icon).SetTexture(value);
                SetPotionEffectIcon(this.PotionEffect, value);
            }
        }

        /// <summary>
        /// Gets or sets the texture of the icon to use when this effect is unknown.
        /// </summary>
        public Texture2D EffectUnknownIconTexture
        {
            get
            {
                return this.GetSettingsOrFail().lockedEffectIcon.textures[0];
            }

            set
            {
                var settings = this.GetSettingsOrFail();
                var icon = new CrucibleIcon(settings.lockedEffectIcon);

                // To avoid changing the shared icon, we need to clone it.
                if (!ClonedLockIcons.Contains(icon.Icon))
                {
                    icon = icon.Clone($"Crucible Effect {this.PotionEffect.name} Locked Icon");
                    ClonedLockIcons.Add(icon.Icon);
                }

                icon.SetTexture(value);
            }
        }

#if POTION_EFFECT_ICONS
        /// <summary>
        /// Gets or sets the icon for this effect when the effect is known.
        /// <p>
        /// <b>Warning:</b> Icons require distinct names, and changing the effect icon to one with a different name may result
        /// in problems with the save file.  To change the texture without changing the icon, use <see cref="CrucibleIcon.SetTexture(Texture2D)"/>.
        /// </p>
        /// </summary>
        /// <remarks>
        /// A new icon can be generated from a texture using <see cref="CrucibleIcon.FromTexture(string, Texture2D)"/>.
        /// </remarks>
        public CrucibleIcon EffectIcon
        {
            get
            {
                return new CrucibleIcon(this.PotionEffect.icon);
            }

            set
            {
                this.PotionEffect.icon = value.Icon;
            }
        }

        /// <summary>
        /// Gets or sets the icon to use on the potion map for this effect when the effect is unknown.
        /// <b>Warning:</b> Icons require distinct names, and changing the effect icon to one with a different name may result
        /// in problems with the save file.  To change the texture without changing the icon, use <see cref="CrucibleIcon.SetTexture(Texture2D)"/>.
        /// </p>
        /// </summary>
        /// <remarks>
        /// A new icon can be generated from a texture using <see cref="CrucibleIcon.FromTexture(string, Texture2D)"/>.
        /// </remarks>
        public CrucibleIcon EffectUnknownIcon
        {
            get
            {
                return new CrucibleIcon(this.GetSettingsOrFail().lockedEffectIcon);
            }

            set
            {
                if (!EffectSettings.TryGetValue(this.PotionEffect, out PotionEffectSettings settings) || settings == null)
                {
                    settings = new PotionEffectSettings();
                    EffectSettings[this.PotionEffect] = settings;
                }

                settings.lockedEffectIcon = value.Icon;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the price of this effect for use when calculating potion costs.
        /// </summary>
        public int BasePrice
        {
            get
            {
                return this.PotionEffect.price;
            }

            set
            {
                this.PotionEffect.price = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of this potion effect as seen in potion color gradiants.
        /// </summary>
        public Color PotionColor
        {
            get
            {
                return this.PotionEffect.color;
            }

            set
            {
                this.PotionEffect.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite that surrounds the potion effect.
        /// </summary>
        /// <remarks>
        /// This sprite provides the visuals for the bottle that surrounds the <see cref="EffectIcon"/>.
        /// </remarks>
        /// <seealso cref="BottleActiveSprite"/>
        public Sprite BottleSprite
        {
            get
            {
                return this.GetSettingsOrFail().effectSlotIdleSprite;
            }

            set
            {
                if (!EffectSettings.TryGetValue(this.PotionEffect, out PotionEffectSettings settings) || settings == null)
                {
                    settings = new PotionEffectSettings();
                    EffectSettings[this.PotionEffect] = settings;
                }

                settings.effectSlotIdleSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite that surrounds the potion effect when the player has placed the potion bottle over this effect.
        /// </summary>
        /// <remarks>
        /// This sprite provides the visuals for the bottle that surrounds the <see cref="EffectIcon"/> when the player has positioned their potion over the effect.
        /// </remarks>
        /// <seealso cref="BottleSprite"/>
        public Sprite BottleActiveSprite
        {
            get
            {
                return this.GetSettingsOrFail().effectSlotActiveSprite;
            }

            set
            {
                if (!EffectSettings.TryGetValue(this.PotionEffect, out PotionEffectSettings settings) || settings == null)
                {
                    settings = new PotionEffectSettings();
                    EffectSettings[this.PotionEffect] = settings;
                }

                settings.effectSlotActiveSprite = value;
            }
        }

        /// <summary>
        /// Creates a new Potion Effect.
        /// </summary>
        /// <param name="id">The ID of the potion effect to create.</param>
        /// <returns>A <see cref="CruciblePotionEffect"/> for manipulating the created potion effect.</returns>
        public static CruciblePotionEffect CreatePotionEffect(string id)
        {
            if (PotionEffect.allPotionEffects.Find(x => x.name == id) != null)
            {
                throw new ArgumentException($"A potion effect with the ID \"{id}\" already exists.");
            }

            var effect = ScriptableObject.CreateInstance<PotionEffect>();
            effect.name = id;

            var blankTexture = TextureUtilities.CreateBlankTexture(1, 1, Color.clear);
            effect.icon = CrucibleIcon.FromTexture($"Crucible Effect {id} Icon", blankTexture).Icon;

            effect.color = Color.white;
            effect.price = 10;

            PotionEffect.allPotionEffects.Add(effect);

            var settings = ScriptableObject.CreateInstance<PotionEffectSettings>();
            var defaultSettings = GetAnyEffectSettings();

            // All of the below seem to be shared across all effects.
            settings.lockedEffectIcon = defaultSettings.lockedEffectIcon;
            settings.effectSlotActiveSprite = defaultSettings.effectSlotActiveSprite;
            settings.effectSlotIdleSprite = defaultSettings.effectSlotIdleSprite;
            settings.idleSepiaSettings = defaultSettings.idleSepiaSettings;
            settings.unknownSepiaSettings = defaultSettings.unknownSepiaSettings;
            settings.collectedSepiaSettings = defaultSettings.collectedSepiaSettings;
            settings.collectAnimationTime = defaultSettings.collectAnimationTime;

            EffectSettings.Add(effect, settings);

            CrucibleLocalization.SetLocalizationKey($"potion_{id}", id);
            CrucibleLocalization.SetLocalizationKey($"effect_{id}", id);

            return new CruciblePotionEffect(effect);
        }

        /// <summary>
        /// Gets a potion effect by id.
        /// </summary>
        /// <param name="id">The id of the potion effect to get.</param>
        /// <returns>A <see cref="CruciblePotionEffect"/> object for manipulating the potion effect, or null if no potion effect exists with the given id.</returns>
        public static CruciblePotionEffect GetPotionEffectByID(string id)
        {
            var effect = PotionEffect.allPotionEffects.Find(x => x.name == id);
            if (effect == null)
            {
                return null;
            }

            return new CruciblePotionEffect(effect);
        }

        /// <inheritdoc/>
        public bool Equals(CruciblePotionEffect other)
        {
            return this.PotionEffect == other.PotionEffect;
        }

        /// <summary>
        /// Sets the localized name of this potion.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public void SetLocalizedName(LocalizedString name)
        {
            CrucibleLocalization.SetLocalizationKey($"potion_{this.PotionEffect.name}", name);
            CrucibleLocalization.SetLocalizationKey($"effect_{this.PotionEffect.name}", name);
        }

        private static void SetPotionEffectIcon(PotionEffect potionEffect, Texture2D texture)
        {
            if (spriteAtlas == null)
            {
                spriteAtlas = new CrucibleSpriteAtlas("CruciblePotionEffects");

                IconsResolveAtlasEvent.OnAtlasRequest += (_, e) =>
                {
                    if (e.Object is not PotionEffect potionEffect)
                    {
                        return;
                    }

                    if (AtlasOverriddenPotionEffects.Contains(potionEffect))
                    {
                        e.AtlasResult = spriteAtlas.AtlasName;
                    }
                };

                CrucibleSpriteAtlasManager.AddAtlas(spriteAtlas);
            }

            spriteAtlas.SetIcon(potionEffect.icon.name, texture, yOffset: texture.height - 15);

            AtlasOverriddenPotionEffects.Add(potionEffect);
        }

        private static void TryResolveSettingsFromMap(PotionEffect effect)
        {
            foreach (var mapState in MapLoader.loadedMaps)
            {
                var mapObject = mapState.transform.gameObject;
                foreach (var potionEffectItem in mapObject.GetComponentsInChildren<PotionEffectMapItem>())
                {
                    if (potionEffectItem.effect != effect)
                    {
                        continue;
                    }

                    EffectSettings[effect] = Traverse.Create(potionEffectItem).Field<PotionEffectSettings>("settings").Value;
                    return;
                }
            }

            // We didn't find any settings.  Store a null so we know not to look in the future.
            EffectSettings[effect] = null;
        }

        private static PotionEffectSettings GetAnyEffectSettings()
        {
            // FIXME: We might capture a custom potion effect here, which might have customized
            // bottle sprites.  We should restrict our search to base game effects.
            foreach (var mapState in MapLoader.loadedMaps)
            {
                var mapObject = mapState.transform.gameObject;
                var mapItem = mapObject.GetComponentInChildren<PotionEffectMapItem>();
                if (mapItem != null)
                {
                    return Traverse.Create(mapItem).Field<PotionEffectSettings>("settings").Value;
                }
            }

            throw new Exception("Failed to find a valid PotionEffectMapItem.  This can happen if another mod clears out all base game potion maps.");
        }

        private PotionEffectSettings GetSettingsOrFail()
        {
            if (!EffectSettings.ContainsKey(this.PotionEffect))
            {
                TryResolveSettingsFromMap(this.PotionEffect);
            }

            if (!EffectSettings.TryGetValue(this.PotionEffect, out PotionEffectSettings settings) || settings == null)
            {
                throw new InvalidOperationException($"No settings found for potion effect \"{this.PotionEffect.name}\".  This can occur for non-Crucible potion effects that have not been added to any potion base.");
            }

            return settings;
        }
    }
}
