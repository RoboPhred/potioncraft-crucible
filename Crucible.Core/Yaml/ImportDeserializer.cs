// <copyright file="ImportDeserializer.cs" company="RoboPhredDev">
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
    using System.IO;
    using RoboPhredDev.PotionCraft.Crucible.Resources;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A deserializer capable of importing additional yaml files based on `!import` statements.
    /// </summary>
    internal class ImportDeserializer : INodeDeserializer
    {
        /// <summary>
        /// Try to consume an import tag node.
        /// </summary>
        /// <param name="reader">The parser to consume from.</param>
        /// <param name="filePath">The resulting file path of the import node, if any.</param>
        /// <returns>True if an import node was consumed, otherwise False.</returns>
        public static bool TryConsumeImport(IParser reader, out string filePath)
        {
            if (reader.Accept<NodeEvent>(out var nodeEvent) && nodeEvent.Tag == "!import")
            {
                reader.Consume<NodeEvent>();

                // Tag can either be on the scalar itself, or sometimes be followed by the scalar.
                // Not sure about the reasoning behind this, but the latter occurs when there is more than one import in a sequence.
                if (nodeEvent is Scalar scalar)
                {
                    filePath = scalar.Value;
                }
                else
                {
                    filePath = reader.Consume<Scalar>().Value;
                }

                filePath = Path.Combine(Path.GetDirectoryName(Deserializer.CurrentFilePath), filePath);
                if (!CrucibleResources.Exists(filePath))
                {
                    throw new YamlException(nodeEvent.Start, nodeEvent.End, $"Cannot import file \"{filePath}\" as the file does not exist in the current package.");
                }

                return true;
            }

            filePath = null;
            return false;
        }

        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (TryConsumeImport(reader, out var filePath))
            {
                value = Deserializer.Deserialize(filePath, expectedType);
                return true;
            }

            value = null;
            return false;
        }
    }
}
