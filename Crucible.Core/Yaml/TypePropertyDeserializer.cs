// <copyright file="TypePropertyDeserializer.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Reflection;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A node deserializer capable of deserializing multiple types base don duck type property matching.
    /// </summary>
    internal class TypePropertyDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            // Do not check for derived types, as all our candidate types are all derived from the marked base type.
            var typePropertyAttribute = expectedType.GetCustomAttribute<TypePropertyAttribute>(false);

            if (typePropertyAttribute == null || string.IsNullOrEmpty(typePropertyAttribute.TypePropertyName))
            {
                value = null;
                return false;
            }

            var candidates = TypePropertyCandidateAttribute.GetCandidateTypes(expectedType);

            // Things get gnarly here, as we might be parsing an !import tag.
            if (ImportDeserializer.TryConsumeImport(reader, out var resourcePath))
            {
                value = Deserializer.WithResourceFileParser(resourcePath, importParser =>
                {
                    // Expecting a basic, non fragment single document file.
                    importParser.Consume<StreamStart>();
                    importParser.Consume<DocumentStart>();
                    var result = DeserializeType(importParser, expectedType, typePropertyAttribute.TypePropertyName, candidates, nestedObjectDeserializer);
                    importParser.Consume<DocumentEnd>();
                    importParser.Consume<StreamEnd>();
                    return result;
                });
            }
            else
            {
                value = DeserializeType(reader, expectedType, typePropertyAttribute.TypePropertyName, candidates, nestedObjectDeserializer);
            }

            return true;
        }

        private static object DeserializeType(IParser reader, Type expectedType, string typePropertyName, IDictionary<string, Type> candidates, Func<IParser, Type, object> nestedObjectDeserializer)
        {
            // We need to parse this object in its entirity to discover its keys.
            //  We also need to remember what we have parsed so that nestedObjectDeserializer can do its thing.
            // This replay parser will remember everything we parsed, and re-parse it for nestedObjectDeserializer.
            var replayParser = new ReplayParser();

            var mappingStart = reader.Consume<MappingStart>();
            replayParser.Enqueue(mappingStart);

            string receivedTypeProperty = null;

            MappingEnd mappingEnd;
            while (!reader.TryConsume(out mappingEnd))
            {
                var key = reader.Consume<Scalar>();
                replayParser.Enqueue(key);

                if (key.Value == typePropertyName)
                {
                    var scalar = reader.Consume<Scalar>();
                    replayParser.Enqueue(scalar);
                    receivedTypeProperty = scalar.Value;
                    continue;
                }

                // The value might be more complex than just a scaler
                //  This code is cribbed from the SkipThisAndNestedEvents IParser extension method
                var depth = 0;
                do
                {
                    var next = reader.Consume<ParsingEvent>();
                    depth += next.NestingIncrease;

                    // Make sure to save this node for the nestedObjectDeserializer pass
                    replayParser.Enqueue(next);
                }
                while (depth > 0);
            }

            // reader.TryConsume will have obtained a mapping end, queue it up.
            replayParser.Enqueue(mappingEnd);

            if (!candidates.TryGetValue(receivedTypeProperty, out var chosenType))
            {
                // We have candidate types, but we were unable to find a match.
                throw new YamlException(
                    mappingStart.Start,
                    mappingEnd.End,
                    $"Cannot identify instance type for {expectedType.Name} based on its type property {receivedTypeProperty}.  Must be one of: {string.Join(", ", candidates.Keys)}");
            }

            replayParser.Start();
            return nestedObjectDeserializer(replayParser, chosenType);
        }
    }
}
