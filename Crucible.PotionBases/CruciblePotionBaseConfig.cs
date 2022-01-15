// <copyright file="CruciblePotionBaseConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.PotionBases
{
    using System;
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.MapEntities;
    using RoboPhredDev.PotionCraft.Crucible.PotionBases.Entities;
    using UnityEngine;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration subject for a PotionCraft potion base.
    /// </summary>
    public class CruciblePotionBaseConfig : CruciblePackageConfigSubjectNode<CruciblePotionBase>
    {
        private static readonly HashSet<CruciblePotionBase> CreatedPotionBases = new();
        private static readonly HashSet<string> UnlockIdsOnStart = new();

        static CruciblePotionBaseConfig()
        {
            CrucibleGameEvents.OnSaveLoaded += (_, __) =>
            {
                // HACK: Saves that did not previously contain a potion effect on a potion base will still load
                // an empty serialized data object into the effect, zeroing out the status and making the effect
                // unusable.
                // FIXME: We might want to directly patch the game to supply sensible defaults in this case,
                // but I want to avoid behavioral patches to keep crucible compatible with other mods.
                // Hook into various MapItem OnLoad functions and implement the fix there?
                foreach (var potionBase in CreatedPotionBases)
                {
                    foreach (var effect in potionBase.GetEffects())
                    {
                        effect.FixInvalidStatus();
                    }
                }

                foreach (var potionBaseId in UnlockIdsOnStart)
                {
                    var potionBase = CruciblePotionBase.GetPotionBaseById(potionBaseId);
                    potionBase?.GiveToPlayer();
                }
            };
        }

        /// <summary>
        /// Gets or sets the ID of this potion base.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this potion base.
        /// </summary>
        public LocalizedString Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this potion base.
        /// </summary>
        public LocalizedString Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this potion base is available from the start of the game.
        /// </summary>
        public bool UnlockedOnStart { get; set; }

        /// <summary>
        /// Gets or sets the color of this potion base.
        /// </summary>
        public Color? LiquidColor { get; set; }

        /// <summary>
        /// Gets or sets the small icon to display for this potion base in tooltips and ingredient lists.
        /// </summary>
        public Sprite IngredientListIcon { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu.
        /// </summary>
        public Sprite MenuButtonImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base button when this base is selected.
        /// </summary>
        public Sprite MenuButtonSelectedImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu when hovering over the menu.
        /// </summary>
        public Sprite MenuButtonHoverImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu when the potion base is locked.
        /// </summary>
        public Sprite MenuButtonLockedImage { get; set; }

        /// <summary>
        /// Gets or sets the tooltip image to use for this potion base when hovering over the base in the potion base menu.
        /// </summary>
        public Sprite TooltipImage { get; set; }

        /// <summary>
        /// Gets or sets the image to place on the ladle when this potion base is selected.
        /// </summary>
        public Texture2D LadleImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for this potion base in the recipe book.
        /// </summary>
        public Sprite RecipeStepImage { get; set; }

        /// <summary>
        /// Gets or sets the image to display at the center of the potion effect map.
        /// </summary>
        public Sprite MapOriginImage { get; set; }

        /// <summary>
        /// Gets or sets the list of map entities to spawn on this potion effect.
        /// </summary>
        public List<CrucibleMapEntityConfig> MapEntities { get; set; } = new();

        /// <inheritdoc/>
        protected override void OnDeserializeCompleted(Mark start, Mark end)
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                throw new Exception($"Potion base at {start} must have an id.");
            }
        }

        /// <inheritdoc/>
        protected override CruciblePotionBase GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;

            var potionBase = CruciblePotionBase.GetPotionBaseById(id);
            if (potionBase != null)
            {
                return potionBase;
            }

            potionBase = CruciblePotionBase.CreatePotionBase(id);
            CreatedPotionBases.Add(potionBase);
            return potionBase;
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CruciblePotionBase subject)
        {
            if (this.Name != null)
            {
                subject.Name = this.Name;
            }

            if (this.Description != null)
            {
                subject.Description = this.Description;
            }

            if (this.LiquidColor.HasValue)
            {
                subject.LiquidColor = this.LiquidColor.Value;
            }

            if (this.IngredientListIcon != null)
            {
                subject.IngredientListIcon = this.IngredientListIcon;
            }

            if (this.MenuButtonImage != null)
            {
                subject.MenuIcon = this.MenuButtonImage;
            }

            if (this.MenuButtonSelectedImage != null)
            {
                subject.MenuSelectedIcon = this.MenuButtonSelectedImage;
            }

            if (this.MenuButtonHoverImage != null)
            {
                subject.MenuHoverIcon = this.MenuButtonHoverImage;
            }

            if (this.MenuButtonLockedImage != null)
            {
                subject.MenuLockedIcon = this.MenuButtonLockedImage;
            }

            if (this.TooltipImage != null)
            {
                subject.TooltipIcon = this.TooltipImage;
            }

            if (this.LadleImage != null)
            {
                subject.LadleIcon = Sprite.Create(this.LadleImage, new Rect(0, 0, this.LadleImage.width, this.LadleImage.height), new Vector2(-0.25f, 0.5f));
            }

            if (this.RecipeStepImage != null)
            {
                subject.RecipeStepIcon = this.RecipeStepImage;
            }

            if (this.MapOriginImage != null)
            {
                subject.MapIcon = this.MapOriginImage;
            }

            if (this.UnlockedOnStart)
            {
                UnlockIdsOnStart.Add(subject.ID);
            }
            else
            {
                UnlockIdsOnStart.Remove(subject.ID);
            }

            if (this.MapEntities != null)
            {
                using var spawner = CrucibleMapEntitySpawner.WithPotionBase(this.Subject);
                spawner.ClearMap();
                this.MapEntities.ForEach(x => x.AddEntityToSpawner(this.PackageMod.Namespace, spawner));
            }
        }
    }
}
