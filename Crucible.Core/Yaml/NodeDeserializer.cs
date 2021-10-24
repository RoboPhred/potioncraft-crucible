namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Wraps a <see cref="INodeDeserializer"/> with our own modifications.
    /// </summary>
    public class NodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDeserializer"/> class.
        /// </summary>
        /// <param name="nodeDeserializer">The ancestor deserializer to use.</param>
        public NodeDeserializer(INodeDeserializer nodeDeserializer)
        {
            this.nodeDeserializer = nodeDeserializer;
        }

        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            // Cloning the parser at every level might be time consuming, so for now lets limit it to when the declared type
            // actually wants it.
            // We might need to remove this restriction in the future if duck types want it.
            var wantsExtraData = typeof(IDeserializeExtraData).IsAssignableFrom(expectedType);
            ReplayParser extraDataParser = null;
            if (wantsExtraData)
            {
                extraDataParser = ReplayParser.ParseObject(reader);
                reader = extraDataParser;
            }

            Mark start = null;
            if (reader.Accept<ParsingEvent>(out var parsingEvent))
            {
                start = parsingEvent?.Start;
            }

            if (!this.nodeDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
            {
                return false;
            }

            var end = reader.Current?.End;

            if (wantsExtraData && value is IDeserializeExtraData extraDataConsumer)
            {
                extraDataParser.Reset();
                extraDataConsumer.OnDeserializeExtraData(extraDataParser);
            }

            if (value is IAfterYamlDeserialization afterDeserialized)
            {
                afterDeserialized.OnDeserializeCompleted(start, end);
            }

            return true;
        }
    }
}
