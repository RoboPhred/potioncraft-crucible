namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Defines an object element configuration section in a Crucible config file.
    /// This configration element will produce a subject after parsing, and allow
    /// extension config entries to apply their configuration to the subject.
    /// </summary>
    /// <typeparam name="TSubject">The subject object created as a result of this configuration entry.</typeparam>
    public abstract class CrucibleConfigSubjectObject<TSubject> : IDeserializeExtraData
    {
        private readonly List<CrucibleConfigExtension<TSubject>> extensions = new();

        /// <summary>
        /// Applies the configuration node.
        /// </summary>
        public void ApplyConfiguration()
        {
            var subject = this.GetSubject();
            foreach (var extension in this.extensions)
            {
                extension.OnApplyConfiguration(subject);
            }
        }

        /// <summary>
        /// Parse and apply extensions to the subject.
        /// </summary>
        /// <param name="parser">The parser containing this configuration object.</param>
        void IDeserializeExtraData.OnDeserializeExtraData(ReplayParser parser)
        {
            this.extensions.Clear();

            foreach (var extensionType in CrucibleConfigElementRegistry.GetSubjectExtensionTypes<TSubject>())
            {
                parser.Reset();
                var extension = (CrucibleConfigExtension<TSubject>)Deserializer.DeserializeFromParser(extensionType, parser);
                this.extensions.Add(extension);
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
