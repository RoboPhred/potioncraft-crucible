// <copyright file="TypeExtensions.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;

    /// <summary>
    /// Extensions for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks to see if the type implements the given base type.
        /// </summary>
        /// <param name="type">The type to check the base type of.</param>
        /// <param name="baseType">The base type to check if type implements.</param>
        /// <returns>True if type implements the base type, otherwise false.</returns>
        public static bool BaseTypeIncludes(this Type type, Type baseType)
        {
            do
            {
                if (type == baseType)
                {
                    return true;
                }
            }
            while ((type = type.BaseType) != null);

            return false;
        }

        /// <summary>
        /// Returns true if the type implements an instantiation of the generic base type.
        /// </summary>
        /// <param name="type">The type to check the base types of.</param>
        /// <param name="genericType">The generic type definition of the generic type to look for.</param>
        /// <param name="genericInstantiation">If the type implements the given base type, the instantiation of the generic type definition that implements it.</param>
        /// <returns>True if the type implements concretely the given generic type as a base, or false if it does not.</returns>
        public static bool BaseTypeIncludesGeneric(this Type type, Type genericType, out Type genericInstantiation)
        {
            do
            {
                if (type.GetGenericTypeDefinition() == genericType)
                {
                    genericInstantiation = type;
                    return true;
                }
            }
            while ((type = type.BaseType) != null);

            genericInstantiation = null;
            return false;
        }
    }
}
