// <copyright file="SvgPathDeserializer.cs" company="RoboPhredDev">
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
