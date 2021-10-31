namespace RoboPhredDev.PotionCraft.Crucible.PotionBases
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Config;

    /// <summary>
    /// The configuration root for potion bases.
    /// </summary>
    [CrucibleConfigRoot]
    public class CruciblePotionBasesConfigRoot : CrucibleConfigRoot
    {
        /// <summary>
        /// Gets or sets the list of potion bases.
        /// </summary>
        public List<CruciblePotionBaseConfig> PotionBases { get; set; } = new();

        /// <inheritdoc/>
        public override void ApplyConfiguration()
        {
            this.PotionBases.ForEach(x => x.ApplyConfiguration());
        }
    }
}
