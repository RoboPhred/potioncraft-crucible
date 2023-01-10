// <copyright file="Deserializer.cs" company="RoboPhredDev">
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
    using System.IO;
    using RoboPhredDev.PotionCraft.Crucible.Resources;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Deserialization utilities for yaml files.
    /// </summary>
    public static class Deserializer
    {
        /// <summary>
        /// Gets the naming convention used by this deserializer.
        /// </summary>
        public static readonly INamingConvention NamingConvention = CamelCaseNamingConvention.Instance;

        private static readonly Stack<string> ParsingFiles = new();

        /// <summary>
        /// Gets the path of the current file being processed.
        /// If no file is being processed, returns null.
        /// </summary>
        public static string CurrentFilePath
        {
            get
            {
                if (ParsingFiles.Count == 0)
                {
                    return null;
                }

                return ParsingFiles.Peek();
            }
        }

        /// <summary>
        /// Deserializes the given file into the given type.
        /// </summary>
        /// <param name="resourcePath">The path to the file in the resource to deserialize.</param>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeFromResource<T>(string resourcePath)
        {
            return WithResourceFileParser(resourcePath, parser =>
            {
                var deserializer = BuildDeserializer();
                return deserializer.Deserialize<T>(parser);
            });
        }

        /// <summary>
        /// Deserialize the given file into the given type.
        /// </summary>
        /// <param name="resourcePath">The path to the resource to deserialize.</param>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(string resourcePath, Type type)
        {
            return WithResourceFileParser(resourcePath, parser =>
            {
                var deserializer = BuildDeserializer();
                return deserializer.Deserialize(parser, type);
            });
        }

        /// <summary>
        /// Deserialize an object from the parser.
        /// </summary>
        /// <param name="filePath">The file path the parser is from.</param>
        /// <param name="type">The type to deserialize.</param>
        /// <param name="parser">The parser to deserialize from.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeFromParser(string filePath, Type type, IParser parser)
        {
            ParsingFiles.Push(filePath);
            try
            {
                var deserializer = BuildDeserializer();
                return deserializer.Deserialize(parser, type);
            }
            catch (YamlException ex) when (ex is not YamlFileException)
            {
                throw new YamlFileException(CurrentFilePath, ex.Start, ex.End, ex.Message, ex);
            }
            finally
            {
                ParsingFiles.Pop();
            }
        }

        /// <summary>
        /// Deserialize an object from the parser.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <param name="parser">The parser to deserialize from.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeFromParser(Type type, IParser parser)
        {
            var deserializer = BuildDeserializer();
            return deserializer.Deserialize(parser, type);
        }

        /// <summary>
        /// Obtains a <see cref="IParser"/> from a given yaml file path.
        /// </summary>
        /// <param name="resourcePath">The path of the resource file to obtain a parser for.</param>
        /// <param name="func">The function to parse the object from the parser.</param>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <returns>The parser for the given file.</returns>
        public static T WithResourceFileParser<T>(string resourcePath, Func<IParser, T> func)
        {
            ParsingFiles.Push(resourcePath);
            try
            {
                var fileContents = CrucibleResources.ReadAllText(resourcePath);
                var parser = new MergingParser(new Parser(new StringReader(fileContents)));
                return func(parser);
            }
            catch (YamlException ex) when (ex is not YamlFileException)
            {
                throw new YamlFileException(CurrentFilePath, ex.Start, ex.End, ex.Message, ex);
            }
            finally
            {
                ParsingFiles.Pop();
            }
        }

        private static IDeserializer BuildDeserializer()
        {
            var builder = new DeserializerBuilder();

            builder
                .WithNamingConvention(NamingConvention)
                .IgnoreUnmatchedProperties()
                .WithNodeTypeResolver(new ImportNodeTypeResolver(), s => s.OnTop())
                .WithNodeDeserializer(new ImportDeserializer(), s => s.OnTop())
                .WithNodeDeserializer(new DuckTypeDeserializer(), s => s.OnTop())
                .WithNodeDeserializer(new TypePropertyDeserializer(), s => s.OnTop());

            foreach (var type in CrucibleTypeRegistry.GetTypesByAttribute<YamlDeserializerAttribute>())
            {
                if (Activator.CreateInstance(type) is not INodeDeserializer deserializer)
                {
                    // TODO: Warn wrong type.
                    continue;
                }

                builder.WithNodeDeserializer(deserializer, s => s.OnTop());
            }

            builder.WithNodeDeserializer(objectDeserializer => new ExtendedObjectNodeDeserializer(objectDeserializer), s => s.InsteadOf<ObjectNodeDeserializer>());

            return builder.Build();
        }
    }
}
