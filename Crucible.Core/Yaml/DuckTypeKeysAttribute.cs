// <copyright file="DuckTypeKeysAttribute.cs" company="RoboPhredDev">
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
    /// Overrides the keys attributed to this class when resolved with duck typing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DuckTypeKeysAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuckTypeKeysAttribute"/> class.
        /// </summary>
        /// <param name="keys">The keys to present to the duck typing system.</param>
        public DuckTypeKeysAttribute(string[] keys)
        {
            this.Keys = keys.ToList();
        }

        /// <summary>
        /// Gets the keys to provide for this class to the duck typing system.
        /// </summary>
        public IReadOnlyList<string> Keys { get; }
    }
}
