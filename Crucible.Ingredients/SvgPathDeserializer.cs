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
        private bool suppressDeserializer = false;

        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (this.suppressDeserializer)
            {
                value = null;
                return false;
            }

            if (expectedType != typeof(SvgPath))
            {
                value = null;
                return false;
            }

            if (reader.TryConsume<Scalar>(out var scalar))
            {
                value = new SvgPath(scalar.Value);
                return true;
            }

            this.suppressDeserializer = true;
            try
            {
                value = nestedObjectDeserializer(reader, typeof(SvgPath));
                return true;
            }
            finally
            {
                this.suppressDeserializer = false;
            }
        }
    }
}
