namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Config;

    /// <summary>
    /// The configuration root for ingredients.
    /// </summary>
    [CrucibleConfigRoot]
    public class CrucibleIngredientsConfigRoot : CrucibleConfigRoot
    {
        /// <summary>
        /// Gets or sets the list of ingredients.
        /// </summary>
        public List<CrucibleIngredientConfig> Ingredients { get; set; } = new();

        /// <inheritdoc/>
        public override void ApplyConfiguration()
        {
            this.Ingredients.ForEach(x => x.ApplyConfiguration());
        }
    }
}
