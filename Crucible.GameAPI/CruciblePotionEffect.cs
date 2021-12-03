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
    using System.Collections;
    using System.Collections.Generic;
    using HarmonyLib;
    using ObjectBased.RecipeMap;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="PotionEffect"/>s.
    /// </summary>
    public sealed class CruciblePotionEffect : IEquatable<CruciblePotionEffect>
    {
        private static readonly Dictionary<PotionEffect, PotionEffectSettings> EffectSettings = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePotionEffect"/> class.
        /// </summary>
        /// <param name="potionEffect">The potion effect to wrap.</param>
        internal CruciblePotionEffect(PotionEffect potionEffect)
        {
            this.PotionEffect = potionEffect;
        }

        /// <summary>
        /// Gets the effect ID of this effect.
        /// </summary>
        public string ID
        {
            get
            {
                return this.PotionEffect.name;
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
        /// Gets or sets the icon texture for this effect.
        /// </summary>
        public Texture2D EffectIcon
        {
            get
            {
                return this.PotionEffect.icon.textures[0];
            }

            set
            {
                var icon = this.PotionEffect.icon;

                if (icon.textures.Length != 1)
                {
                    icon.textures = new Texture2D[1];
                }

                icon.textures[0] = value;

                // Clear the icon cache so our new sprite is generated.
                // TODO: Should make this into a utility class/func.
                // Do we want to try resetting the icon like this, or is it better to generate a new icon
                // and remove the old icon from Icon.allIcons?
                var variants = Traverse.Create(icon).Field("renderedVariants").GetValue() as IList;
                variants.Clear();
            }
        }

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
        /// Gets or sets the sprite to use when this effect is active.
        /// </summary>
        public Sprite ActiveSprite
        {
            get
            {
                if (!EffectSettings.ContainsKey(this.PotionEffect))
                {
                    TryResolveSettingsFromMap(this.PotionEffect);
                }

                return EffectSettings[this.PotionEffect]?.effectSlotActiveSprite;
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
        /// Gets or sets the sprite to use when this effect is idle.
        /// </summary>
        public Sprite IdleSprite
        {
            get
            {
                if (!EffectSettings.ContainsKey(this.PotionEffect))
                {
                    TryResolveSettingsFromMap(this.PotionEffect);
                }

                return EffectSettings[this.PotionEffect]?.effectSlotIdleSprite;
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
        /// Creates a new Potion Effect.
        /// </summary>
        /// <param name="id">The ID of the potion effect to create.</param>
        /// <returns>A <see cref="CruciblePotionEffect"> for manipulating the created potion effect.</returns>
        public static CruciblePotionEffect CreatePotionEffect(string id)
        {
            var effect = ScriptableObject.CreateInstance<PotionEffect>();
            effect.name = id;

            var icon = ScriptableObject.CreateInstance<Icon>();
            var blankTexture = TextureUtilities.CreateBlankTexture(1, 1, new Color(0, 0, 0, 0));
            icon.textures = new[] { blankTexture };
            icon.contourTexture = blankTexture;
            icon.scratchesTexture = blankTexture;
            icon.defaultIconColors = new Color[0];

            // Perform icon initialization logic.
            Icon.allIcons.Add(icon);
            icon.GetElementBackgroundSprite();

            effect.icon = icon;

            effect.color = Color.white;
            effect.price = 10;

            PotionEffect.allPotionEffects.Add(effect);

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
    }
}
