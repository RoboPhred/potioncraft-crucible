namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="SvgPath"/> objects from YAML.
    /// </summary>
    [YamlDeserializer]
    public class SvgPathDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (expectedType != typeof(SvgPath))
            {
                value = null;
                return false;
            }

            var scalar = reader.Consume<Scalar>();
            value = SvgPath.Parse(scalar.Value);
            return true;
        }
    }
}
