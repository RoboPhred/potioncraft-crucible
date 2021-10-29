namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    /// <summary>
    /// Defines a configuration object root.
    /// </summary>
    public abstract class CrucibleConfigRoot : CrucibleConfigNode
    {
        /// <summary>
        /// Apply this configuration node.
        /// </summary>
        public abstract void OnApplyConfiguration();
    }
}
