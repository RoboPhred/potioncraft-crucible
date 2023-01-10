// <copyright file="ExtendedObjectNodeDeserializer.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Wraps <see cref="ObjectNodeDeserializer"/> with our own modifications.
    /// </summary>
    public class ExtendedObjectNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedObjectNodeDeserializer"/> class.
        /// </summary>
        /// <param name="nodeDeserializer">The ancestor deserializer to use.</param>
        public ExtendedObjectNodeDeserializer(INodeDeserializer nodeDeserializer)
        {
            this.nodeDeserializer = nodeDeserializer;
        }

        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            var extraDataParser = ReplayParser.ParseObject(reader);
            reader = extraDataParser;

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

            if (value is IDeserializeExtraData extraDataConsumer)
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
