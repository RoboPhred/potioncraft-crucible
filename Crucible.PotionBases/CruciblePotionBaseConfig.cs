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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// Configuration subject for a PotionCraft ingredient.
    /// </summary>
    public class CruciblePotionBaseConfig : CrucibleConfigSubjectObject<CruciblePotionBase>
    {
        private static readonly HashSet<string> UnlockIdsOnStart = new();

        private string id;

        static CruciblePotionBaseConfig()
        {
            CrucibleGameEvents.OnSaveLoaded += (_, __) =>
            {
                foreach (var potionBaseId in UnlockIdsOnStart)
                {
                    var potionBase = CruciblePotionBase.GetPotionBaseById(potionBaseId);
                    potionBase?.GiveToPlayer();
                }
            };
        }

        /// <summary>
        /// Gets or sets the ID of this ingredient.
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.Replace(" ", string.Empty);
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of this potion base.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this potion base.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this potion base is available from the start of the game.
        /// </summary>
        public bool UnlockedOnStart { get; set; }

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

        /// <inheritdoc/>
        protected override CruciblePotionBase GetSubject()
        {
            var id = this.Mod.Namespace + "." + this.ID;
            return CruciblePotionBase.GetPotionBaseById(id) ?? CruciblePotionBase.CreatePotionBase(id);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CruciblePotionBase subject)
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                subject.Name = this.Name;
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                subject.Description = this.Description;
            }

            // TODO: From config.  Make a deserializer for the Color class.  Use hex code string or rgba object
            subject.LiquidColor = Color.red;

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

            // FIXME: This isnt working.  Image is missing from tooltip.
            if (this.TooltipImage != null)
            {
                subject.TooltipIcon = this.TooltipImage;
            }

            if (this.LadleImage != null)
            {
                subject.LadleIcon = Sprite.Create(this.LadleImage, new Rect(0, 0, this.LadleImage.width, this.LadleImage.height), new Vector2(0f, 0.5f));
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
        }
    }
}
