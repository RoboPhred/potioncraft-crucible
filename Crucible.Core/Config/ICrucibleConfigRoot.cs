namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    /// <summary>
    /// Defines a configuration object root.
    /// </summary>
    public interface ICrucibleConfigRoot
    {
        /// <summary>
        /// Apply this configuration node.
        /// </summary>
        void OnApplyConfiguration();
    }
}
