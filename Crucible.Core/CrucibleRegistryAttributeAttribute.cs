// <copyright file="CrucibleRegistryAttributeAttribute.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Foobar is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;

    /// <summary>
    /// Specifies that classes tagged by this attribute should be tracked in the Crucible registry.
    /// </summary>
    /// <remarks>
    /// Attributes marked with this attribute will be discoverable through the <see cref="CrucibleTypeRegistry"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class CrucibleRegistryAttributeAttribute : Attribute
    {
    }
}
