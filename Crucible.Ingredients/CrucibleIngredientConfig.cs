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
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.SVG;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration subject for a PotionCraft ingredient.
    /// </summary>
    public class CrucibleIngredientConfig : CruciblePackageConfigSubjectNode<CrucibleIngredient>
    {
        /// <summary>
        /// Gets or sets the ID of this ingredient.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this ingredient.
        /// </summary>
        public LocalizedString Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this ingredient shown in the tooltip.
        /// </summary>
        public LocalizedString Description { get; set; }

        /// <summary>
        /// Gets or sets the id of the ingredient to inherit from.
        /// </summary>
        public string InheritFrom { get; set; }

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
        /// Gets or sets the bace price for this ingredient.
        /// </summary>
        public float? BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the ingredient path for this ingredient.
        /// </summary>
        public SvgPath Path { get; set; }

        /// <summary>
        /// Gets or sets a value specifying what percentage of the ingredient is pre-ground.
        /// </summary>
        public float? GrindStartPercent { get; set; }

        /// <summary>
        /// Gets or sets the color to use for the ground substance.
        /// </summary>
        public Color? GroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this ingredient teleports during movement.
        /// </summary>
        public bool? IsTeleportationIngredient { get; set; }

        /// <summary>
        /// Gets or sets the stack items for this ingredient.
        /// </summary>
        public OneOrMany<CrucibleIngredientStackItemConfig> StackItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stack items behave as solid items in the morter.
        /// </summary>
        public bool? IsStackItemsSolid { get; set; }

        /// <summary>
        /// Gets or sets the default closeness requirement for this ingredient to show up in a trader's inventory.
        /// </summary>
        public int ClosenessRequirement { get; set; } = 0;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

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
            var id = this.PackageMod.Namespace + "." + this.ID;
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

            if (this.BasePrice.HasValue)
            {
                subject.Price = this.BasePrice.Value;
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

            if (this.StackItems != null && this.StackItems.Count > 0)
            {
                subject.SetStack(this.StackItems.Select(x => x.ToStackItem()));
            }

            if (this.IsStackItemsSolid.HasValue)
            {
                subject.IsStackItemSolid = this.IsStackItemsSolid.Value;
            }

            if (this.ClosenessRequirement > 0)
            {
                subject.DefaultClosenessRequirement = this.ClosenessRequirement;
            }
        }
    }
}
