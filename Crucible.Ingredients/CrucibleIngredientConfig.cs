// <copyright file="CrucibleIngredientConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System;
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;
    using YamlDotNet.Core;

    /// <summary>
    /// Configuration subject for a PotionCraft ingredient.
    /// </summary>
    public class CrucibleIngredientConfig : CrucibleConfigSubjectObject<CrucibleIngredient>
    {
        /// <summary>
        /// Gets or sets the ID of this ingredient.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this ingredient.
        /// </summary>
        public LocalizedString Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the ingredient to inherit from.
        /// </summary>
        public string InheritFrom { get; set; }

        /// <summary>
        /// Gets or sets the description of this ingredient shown in the tooltip.
        /// </summary>
        public LocalizedString Description { get; set; }

        /// <summary>
        /// Gets or sets the sprite to use for the inventory image of this ingredient.
        /// </summary>
        public Sprite InventoryImage { get; set; }

        /// <summary>
        /// Gets or sets the sprite to use for this ingredient in the recipe book.
        /// </summary>
        public Sprite RecipeStepImage { get; set; }

        /// <summary>
        /// Gets or sets the small icon to display for this ingredient in tooltips and ingredient lists.
        /// </summary>
        public Sprite IngredientListIcon { get; set; }

        /// <summary>
        /// Gets or sets the color to use for the ground substance.
        /// </summary>
        public Color? GroundColor { get; set; }

        /// <summary>
        /// Gets or sets the bace price for this ingredient.
        /// </summary>
        public float? Price { get; set; }

        /// <summary>
        /// Gets or sets the ingredient path for this ingredient.
        /// </summary>
        public SvgPath Path { get; set; }

        /// <summary>
        /// Gets or sets a value specifying what percentage of the ingredient is pre-ground.
        /// </summary>
        public float? GrindStartPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this ingredient teleports during movement.
        /// </summary>
        public bool? IsTeleportationIngredient { get; set; }

        /// <summary>
        /// Gets or sets the stack items for this ingredient.
        /// </summary>
        public OneOrMany<CrucibleIngredientStackItemConfig> StackItems { get; set; }

        /// <inheritdoc/>
        protected override void OnDeserializeCompleted(Mark start, Mark end)
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                throw new Exception($"Ingredient at {start} must have an id.");
            }
        }

        /// <inheritdoc/>
        protected override CrucibleIngredient GetSubject()
        {
            var id = this.Mod.Namespace + "." + this.ID;
            return CrucibleIngredient.GetIngredientById(id) ?? CrucibleIngredient.CreateIngredient(id, this.InheritFrom ?? "Waterbloom");
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleIngredient subject)
        {
            if (this.Name != null)
            {
                subject.SetLocalizedName(this.Name);
            }

            if (this.Description != null)
            {
                subject.SetLocalizedDescription(this.Description);
            }

            if (this.InventoryImage != null)
            {
                subject.InventoryIcon = this.InventoryImage;
            }

            if (this.RecipeStepImage != null)
            {
                subject.RecipeStepIcon = this.RecipeStepImage;
            }

            if (this.IngredientListIcon != null)
            {
                subject.IngredientListIcon = this.IngredientListIcon;
            }

            if (this.Price.HasValue)
            {
                subject.Price = this.Price.Value;
            }

            if (this.IsTeleportationIngredient.HasValue)
            {
                subject.IsTeleportationIngredient = this.IsTeleportationIngredient.Value;
            }

            if (this.GrindStartPercent.HasValue)
            {
                subject.PathPregrindPercentage = this.GrindStartPercent.Value;
            }

            if (this.Path != null)
            {
                subject.SetPath(this.Path.ToPathSegments());
            }

            if (this.GroundColor.HasValue)
            {
                subject.GroundColor = this.GroundColor.Value;
            }

            if (this.StackItems.Count > 0)
            {
                subject.SetStack(this.StackItems.Select(x => x.ToStackItem()));
            }
        }
    }
}
