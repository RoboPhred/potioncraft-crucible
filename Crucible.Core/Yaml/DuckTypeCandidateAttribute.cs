// <copyright file="DuckTypeCandidateAttribute.cs" company="RoboPhredDev">
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

    /// <summary>
    /// Describes a candidate type for duck type instantiation of the decorated class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
    public class DuckTypeCandidateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuckTypeCandidateAttribute"/> class.
        /// </summary>
        /// <param name="candidateType">The type of the candidate to deserialize as.</param>
        public DuckTypeCandidateAttribute(Type candidateType)
        {
            this.CandidateType = candidateType;
        }

        /// <summary>
        /// Gets the candidate type for duck type deserialization.
        /// </summary>
        public Type CandidateType { get; }

        /// <summary>
        /// Get all candidate duck types for the given types.
        /// </summary>
        /// <param name="type">The type to find candidate duck types from.</param>
        /// <returns>A list of candidate duck types for the given type.</returns>
        public static ICollection<Type> GetDuckCandidates(Type type)
        {
            var candidateAttributes = (DuckTypeCandidateAttribute[])type.GetCustomAttributes(typeof(DuckTypeCandidateAttribute), false);
            return candidateAttributes.Select(attr => attr.CandidateType).ToArray();
        }
    }
}
