// <copyright file="Vector2Deserializer.cs" company="RoboPhredDev">
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
    using System.Linq;
    using UnityEngine;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="Vector2"/> objects.
    /// </summary>
    [YamlDeserializer]
    public class Vector2Deserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (expectedType != typeof(Vector2) && Nullable.GetUnderlyingType(expectedType) != typeof(Vector2))
            {
                value = null;
                return false;
            }

            if (reader.TryConsume<Scalar>(out var scalar))
            {
                float[] parts;
                try
                {
                    // Make sure to use invariant culture, as some languages flip commas and dots for decimal markers.
                    parts = scalar.Value.Split(',').Select(x => x.Trim()).Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                }
                catch
                {
                    throw new FormatException("Invalid vector value.  Expected two floating point numbers.");
                }

                if (parts.Length != 2)
                {
                    throw new FormatException("Invalid vector value.  Expected two floating point numbers.");
                }

                value = new Vector2(parts[0], parts[1]);
                return true;
            }

            var parseObj = (Vector2ParseObj)nestedObjectDeserializer(reader, typeof(Vector2ParseObj));
            value = new Vector2(parseObj.X, parseObj.Y);
            return true;
        }

        private class Vector2ParseObj
        {
            public float X { get; set; }

            public float Y { get; set; }
        }
    }
}
