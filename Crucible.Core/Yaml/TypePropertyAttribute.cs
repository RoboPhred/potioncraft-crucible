// <copyright file="TypePropertyAttribute.cs" company="RoboPhredDev">
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
    using System.Reflection;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Indicates that the specified property should be used to determine the type to deserialize.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TypePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypePropertyAttribute"/> class.
        /// </summary>
        /// <param name="typePropertyName">The name of the type property to use when determinig the derived type to deserialize.</param>
        public TypePropertyAttribute(string typePropertyName)
        {
            this.TypePropertyName = typePropertyName;
        }

        /// <summary>
        /// Gets the name of the type property to use when determinig the derived type to deserialize.
        /// </summary>
        public string TypePropertyName { get; }
    }
}
