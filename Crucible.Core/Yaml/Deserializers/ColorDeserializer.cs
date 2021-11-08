// <copyright file="ColorDeserializer.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Yaml.Deserializers
{
    using System;
    using System.Globalization;
    using UnityEngine;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="Color"/> objects.
    /// </summary>
    [YamlDeserializer]
    public class ColorDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            // Need to use IsAssignableFrom to support nullable colors
            if (expectedType != typeof(Color) && Nullable.GetUnderlyingType(expectedType) != typeof(Color))
            {
                value = null;
                return false;
            }

            if (reader.TryConsume<Scalar>(out var scalar))
            {
                var textValue = scalar.Value;
                if (textValue.StartsWith("#"))
                {
                    textValue = textValue.Substring(1);
                    if (textValue.Length != 6)
                    {
                        // TODO: alpha
                        throw new FormatException("Unknown color format.  Expected a hexadecimal string.");
                    }

                    if (!int.TryParse(textValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorValue))
                    {
                        throw new FormatException("Unknown color format.  Failed to parse hexadecimal value.");
                    }

                    value = new Color
                    {
                        a = 1,
                        r = (byte)((colorValue >> 16) & 0xFF) / 255f,
                        g = (byte)((colorValue >> 8) & 0xFF) / 255f,
                        b = (byte)(colorValue & 0xFF) / 255f,
                    };
                    return true;
                }
                else
                {
                    throw new FormatException("Unknown color format.  Expected a string starting with \"#\".");
                }
            }

            var parseObj = (ColorParseObj)nestedObjectDeserializer(reader, typeof(ColorParseObj));

            value = new Color(parseObj.R, parseObj.G, parseObj.B, parseObj.A);
            return true;
        }

        private class ColorParseObj
        {
            public float A { get; set; } = 1;

            public float R { get; set; }

            public float G { get; set; }

            public float B { get; set; }
        }
    }
}
