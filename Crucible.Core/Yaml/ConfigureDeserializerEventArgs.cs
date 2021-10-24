namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Event arguments for the <see cref="Deserializer.OnConfigureDeserializer" /> event.
    /// </summary>
    public class ConfigureDeserializerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureDeserializerEventArgs"/> class.
        /// </summary>
        /// <param name="builder">The deserializer builder being configured.</param>
        public ConfigureDeserializerEventArgs(DeserializerBuilder builder)
        {
            this.Builder = builder;
        }

        /// <summary>
        /// Gets the deserializer builder for the deserializer being created.
        /// </summary>
        public DeserializerBuilder Builder { get; }
    }
}
