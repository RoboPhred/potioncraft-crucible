namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    /// <summary>
    /// Defines a configuration object extension to a core config subject.
    /// </summary>
    /// <typeparam name="TSubject">The type this configuration is for.</typeparam>
    public interface ICrucibleConfigExtension<TSubject>
    {
        /// <summary>
        /// Apply this configuration node to the subject.
        /// </summary>
        /// <param name="subject">The subject created by the root configuration node.</param>
        void OnApplyConfiguration(TSubject subject);
    }
}
