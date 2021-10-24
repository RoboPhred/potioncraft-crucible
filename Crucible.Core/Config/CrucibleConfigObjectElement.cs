namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Defines an object element configuration section in a Crucible config file.
    /// This configration element will produce a subject after parsing, and allow
    /// extension config entries to apply their configuration to the subject.
    /// </summary>
    /// <typeparam name="TSubject">The subject object created as a result of this configuration entry.</typeparam>
    public abstract class CrucibleConfigObjectElement<TSubject> : IDeserializeExtraData
    {
        /// <summary>
        /// Gets the subject object created by this configuration element.
        /// </summary>
        public TSubject Subject { get; private set; }

        /// <summary>
        /// Applies this configuration element to the subject.
        /// </summary>
        /// <param name="subject">The subject to apply configuration to.</param>
        public virtual void OnApplyConfiguration(TSubject subject)
        {
        }

        /// <summary>
        /// Parse and apply extensions to the subject.
        /// </summary>
        /// <param name="parser">The parser containing this configuration object.</param>
        void IDeserializeExtraData.OnDeserializeExtraData(ReplayParser parser)
        {
            var subject = this.GetSubject();
            this.Subject = subject;

            this.OnApplyConfiguration(subject);

            var extensionTypes = CrucibleConfigElementRegistry.GetSubjectExtensionTypes<TSubject>();
            foreach (var extensionType in extensionTypes)
            {
                parser.Reset();
                var extension = (ICrucibleConfigExtension<TSubject>)Deserializer.DeserializeFromParser(extensionType, parser);
                extension.OnApplyConfiguration(subject);
            }
        }

        /// <summary>
        /// Gets or creates the subject of this configuration object.
        /// This is called after the element is deserialized, but before extension configurations are applied.
        /// </summary>
        /// <remarks>
        /// Where possible, this function should try to look up subjects by name and return pre-existing matching subjects.
        /// This allows configs to be reloaded without duplicating data.
        /// </remarks>
        /// <returns>The subject to which configuration options should be applied.</returns>
        protected abstract TSubject GetSubject();
    }
}
