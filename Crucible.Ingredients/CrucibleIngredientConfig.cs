namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// Configuration subject for a PotionCraft ingredient.
    /// </summary>
    public class CrucibleIngredientConfig : CrucibleConfigSubjectObject<CrucibleIngredient>
    {
        private string id;

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
        /// Gets or sets the name of this ingredient.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the ingredient to inherit from.
        /// </summary>
        public string InheritFrom { get; set; }

        /// <summary>
        /// Gets or sets the description of this ingredient shown in the tooltip.
        /// </summary>
        public string Description { get; set; }

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
        public Sprite IconImage { get; set; }

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
        /// Gets or sets the sprite to use for this ingredient in the recipe book.
        /// </summary>
        // For backwards compatibility with pantry.
        [Obsolete("Use RecipeStepImage instead.")]
        internal Sprite RecipeImage
        {
            get
            {
                return this.RecipeStepImage;
            }

            set
            {
                this.RecipeStepImage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this ingredient is a crystal.
        /// </summary>
        // For backwards compatibility with pantry.
        [Obsolete("Use IsTeleportationIngredient instead.")]
        internal bool? IsCrystal
        {
            get
            {
                return this.IsTeleportationIngredient;
            }

            set
            {
                this.IsTeleportationIngredient = value;
            }
        }

        /// <inheritdoc/>
        protected override CrucibleIngredient GetSubject()
        {
            var id = this.Mod.Namespace + "." + this.ID;
            return CrucibleIngredient.GetIngredient(id) ?? CrucibleIngredient.CreateIngredient(id, this.InheritFrom ?? "Waterbloom");
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleIngredient subject)
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                subject.Name = this.Name;
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                subject.Description = this.Description;
            }

            if (this.InventoryImage != null)
            {
                subject.InventoryIcon = this.InventoryImage;
            }

            if (this.RecipeStepImage != null)
            {
                subject.RecipeStepIcon = this.RecipeStepImage;
            }

            if (this.IconImage != null)
            {
                subject.IngredientListIcon = this.IconImage;
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
        }
    }
}
