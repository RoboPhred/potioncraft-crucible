namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;

    /// <summary>
    /// Marks a <see cref="INodeDeserializer"/> for registration with the yaml <see cref="Deserializer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [CrucibleRegistryAttribute]
    public class YamlDeserializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlDeserializerAttribute"/> class.
        /// </summary>
        public YamlDeserializerAttribute()
        {
        }
    }
}
