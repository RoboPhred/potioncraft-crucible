// <copyright file="DuckTypeDeserializer.cs" company="RoboPhredDev">
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
    using System.Linq;
    using System.Reflection;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A node deserializer capable of deserializing multiple types base don duck type property matching.
    /// </summary>
    internal class DuckTypeDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            var candidates = DuckTypeCandidateAttribute.GetDuckCandidates(expectedType);
            if (candidates.Count == 0)
            {
                // Not duck typable.
                value = null;
                return false;
            }

            // Things get gnarly here, as we might be parsing an !import tag.
            if (ImportDeserializer.TryConsumeImport(reader, out var resourcePath))
            {
                value = Deserializer.WithResourceFileParser(resourcePath, importParser =>
                {
                    // Expecting a basic, non fragment single document file.
                    importParser.Consume<StreamStart>();
                    importParser.Consume<DocumentStart>();
                    var result = DeserializeDuckType(importParser, expectedType, candidates, nestedObjectDeserializer);
                    importParser.Consume<DocumentEnd>();
                    importParser.Consume<StreamEnd>();
                    return result;
                });
            }
            else
            {
                value = DeserializeDuckType(reader, expectedType, candidates, nestedObjectDeserializer);
            }

            return true;
        }

        private static object DeserializeDuckType(IParser reader, Type expectedType, ICollection<Type> candidates, Func<IParser, Type, object> nestedObjectDeserializer)
        {
            // We need to parse this object in its entirity to discover its keys.
            //  We also need to remember what we have parsed so that nestedObjectDeserializer can do its thing.
            // This replay parser will remember everything we parsed, and re-parse it for nestedObjectDeserializer.
            var replayParser = new ReplayParser();

            var mappingStart = reader.Consume<MappingStart>();
            replayParser.Enqueue(mappingStart);

            var keys = new List<string>();
            MappingEnd mappingEnd;
            while (!reader.TryConsume(out mappingEnd))
            {
                var key = reader.Consume<Scalar>();
                keys.Add(key.Value);
                replayParser.Enqueue(key);

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

            // Pick the type based on the keys we have found.
            var chosenType = ChooseType(candidates, keys);
            if (chosenType == null)
            {
                // We have candidate types, but we were unable to find a match.
                throw new YamlException(
                    mappingStart.Start,
                    mappingEnd.End,
                    $"Cannot identify instance type for {expectedType.Name} based on its properties {string.Join(", ", keys)}.  Must be one of: {string.Join(", ", candidates.Select(x => x.Name))}");
            }

            replayParser.Start();
            return nestedObjectDeserializer(replayParser, chosenType);
        }

        // Choose a candidate duck type based on the keys we posess.
        //  The best match is the one that has the most keys in common.
        private static Type ChooseType(ICollection<Type> candidates, IList<string> keys)
        {
            var matches =
                from candidate in candidates
                let yamlProperties = GetTypeYamlProperties(candidate)
                where !keys.Except(yamlProperties).Any()
                let matchCount = yamlProperties.Intersect(keys).Count()
                where matchCount > 0
                orderby matchCount descending
                select candidate;

            return matches.FirstOrDefault();
        }

        private static IReadOnlyList<string> GetTypeYamlProperties(Type type)
        {
            var keysAttribute = (DuckTypeKeysAttribute)type.GetCustomAttribute(typeof(DuckTypeKeysAttribute));
            if (keysAttribute != null)
            {
                // Attribute overrides key discovery
                return keysAttribute.Keys;
            }

            // YamlDotNet handles any public instance properties or fields.
            var properties =
                from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                let yamlAttribute = property.GetCustomAttribute<YamlMemberAttribute>()
                let name = yamlAttribute?.Alias ?? Deserializer.NamingConvention.Apply(property.Name)
                select name;

            var fields =
                from field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                let yamlAttribute = field.GetCustomAttribute<YamlMemberAttribute>()
                let name = yamlAttribute?.Alias ?? Deserializer.NamingConvention.Apply(field.Name)
                select name;

            return properties.Concat(fields).ToList();
        }
    }
}
