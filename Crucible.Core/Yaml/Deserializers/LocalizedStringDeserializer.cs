// <copyright file="LocalizedStringDeserializer.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="LocalizedString"/> objects.
    /// </summary>
    [YamlDeserializer]
    public class LocalizedStringDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            // Need to use IsAssignableFrom to support nullable colors
            if (expectedType != typeof(LocalizedString))
            {
                value = null;
                return false;
            }

            if (reader.TryConsume<Scalar>(out var scalar))
            {
                value = new LocalizedString(scalar.Value);
                return true;
            }

            var parseObj = (Dictionary<string, string>)nestedObjectDeserializer(reader, typeof(Dictionary<string, string>));

            LocalizedString localizedString;
            if (parseObj.TryGetValue("default", out var defaultValueString))
            {
                localizedString = new LocalizedString(defaultValueString);
                parseObj.Remove("default");
            }
            else
            {
                localizedString = new LocalizedString();
            }

            foreach (var pair in parseObj)
            {
                localizedString.SetLocalization(pair.Key, pair.Value);
            }

            value = localizedString;
            return true;
        }
    }
}
