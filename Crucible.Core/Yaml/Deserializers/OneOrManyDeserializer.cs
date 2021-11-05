// <copyright file="OneOrManyDeserializer.cs" company="RoboPhredDev">
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
    using System.Collections;
    using System.Collections.Generic;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="OneOrMany"/> objects.
    /// </summary>
    [YamlDeserializer]
    public class OneOrManyDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (!expectedType.IsGenericType || expectedType.GetGenericTypeDefinition() != typeof(OneOrMany<>))
            {
                value = null;
                return false;
            }

            var subjectType = expectedType.GetGenericArguments()[0];

            IEnumerable items;
            if (reader.Accept<SequenceStart>(out var _))
            {
                items = (IEnumerable)nestedObjectDeserializer(reader, typeof(List<>).MakeGenericType(subjectType));
            }
            else
            {
                var item = nestedObjectDeserializer(reader, subjectType);
                var array = Array.CreateInstance(subjectType, 1);
                array.SetValue(item, 0);
                items = array;
            }

            // Pass the ctor args as an array, so the array of a single item is not unwrapped.
            value = Activator.CreateInstance(expectedType, new[] { items });
            return true;
        }
    }
}
