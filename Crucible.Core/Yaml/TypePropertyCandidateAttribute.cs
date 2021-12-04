// <copyright file="TypePropertyCandidateAttribute.cs" company="RoboPhredDev">
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

    /// <summary>
    /// Indicates that the specified property should be used to determine the type to deserialize.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TypePropertyCandidateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypePropertyCandidateAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The name of the type to match against the type property for this class.</param>
        public TypePropertyCandidateAttribute(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Gets the name of the type to match against the type property for this class.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets all types, paired by their type name, that can be used to deserialize the given base type.
        /// </summary>
        /// <param name="baseType">The base type to find candidate types from.</param>
        /// <returns>An enumerable of pairs mapping type property values to types that can be instantiated for the given base type.</returns>
        public static IDictionary<string, Type> GetCandidateTypes(Type baseType)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where baseType.IsAssignableFrom(type)
                    let candidateAttribute = type.GetCustomAttribute<TypePropertyCandidateAttribute>()
                    where candidateAttribute != null
                    select new KeyValuePair<string, Type>(candidateAttribute.TypeName, type)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
