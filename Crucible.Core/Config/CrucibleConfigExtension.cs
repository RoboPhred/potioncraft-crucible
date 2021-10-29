namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    /// <summary>
    /// Defines a configuration object extension to a core config subject.
    /// </summary>
    /// <typeparam name="TSubject">The type this configuration is for.</typeparam>
    public abstract class CrucibleConfigExtension<TSubject> : CrucibleConfigNode
    {
        /// <summary>
        /// Apply this configuration node to the subject.
        /// </summary>
        /// <param name="subject">The subject created by the root configuration node.</param>
        public abstract void OnApplyConfiguration(TSubject subject);
    }
}
