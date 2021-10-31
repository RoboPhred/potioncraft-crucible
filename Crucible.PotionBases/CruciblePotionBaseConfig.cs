namespace RoboPhredDev.PotionCraft.Crucible.PotionBases
{
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// Configuration subject for a PotionCraft ingredient.
    /// </summary>
    public class CruciblePotionBaseConfig : CrucibleConfigSubjectObject<CruciblePotionBase>
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
        /// Gets or sets the name of this potion base.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this potion base.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the small icon to display for this potion base in tooltips and ingredient lists.
        /// </summary>
        public Sprite IconImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu.
        /// </summary>
        public Sprite MenuImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base button when this base is selected.
        /// </summary>
        public Sprite MenuSelectedImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu when hovering over the menu.
        /// </summary>
        public Sprite MenuHoverImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for the potion base in the potion base menu when the potion base is locked.
        /// </summary>
        public Sprite MenuLockedImage { get; set; }

        /// <summary>
        /// Gets or sets the tooltip image to use for this potion base when hovering over the base in the potion base menu.
        /// </summary>
        public Sprite MenuTooltipImage { get; set; }

        /// <summary>
        /// Gets or sets the image to place on the ladle when this potion base is selected.
        /// </summary>
        public Texture2D LadleImage { get; set; }

        /// <summary>
        /// Gets or sets the image to use for this potion base in the recipe book.
        /// </summary>
        public Sprite RecipeStepImage { get; set; }

        /// <summary>
        /// Gets or sets the image to display on the potion map.
        /// </summary>
        public Sprite MapItemImage { get; set; }

        /// <inheritdoc/>
        protected override CruciblePotionBase GetSubject()
        {
            var id = this.Mod.Namespace + "." + this.ID;
            return CruciblePotionBase.GetPotionBase(id) ?? CruciblePotionBase.CreatePotionBase(id);
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

            if (this.IconImage != null)
            {
                subject.IngredientListIcon = this.IconImage;
            }

            if (this.MenuImage != null)
            {
                subject.MenuIcon = this.MenuImage;
            }

            if (this.MenuSelectedImage != null)
            {
                subject.MenuSelectedIcon = this.MenuSelectedImage;
            }

            if (this.MenuHoverImage != null)
            {
                subject.MenuHoverIcon = this.MenuHoverImage;
            }

            if (this.MenuLockedImage != null)
            {
                subject.MenuLockedIcon = this.MenuLockedImage;
            }

            // FIXME: This isnt working.  Image is missing from tooltip.
            if (this.MenuTooltipImage != null)
            {
                subject.TooltipIcon = this.MenuTooltipImage;
            }

            if (this.LadleImage != null)
            {
                subject.LadleIcon = Sprite.Create(this.LadleImage, new Rect(0, 0, this.LadleImage.width, this.LadleImage.height), new Vector2(0f, 0.5f));
            }

            if (this.RecipeStepImage != null)
            {
                subject.RecipeStepIcon = this.RecipeStepImage;
            }

            if (this.MapItemImage != null)
            {
                subject.MapIcon = this.MapItemImage;
            }
        }
    }
}
