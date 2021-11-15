// <copyright file="CruciblePackageConfigExtensionAttribute.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;

    /// <summary>
    /// An attribute marking a class as being a configuration extension.
    /// Configuration extension classes will be parsed alongside the root configuration nodes that create instances
    /// of the given subject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [CrucibleRegistryAttribute]
    public class CruciblePackageConfigExtensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePackageConfigExtensionAttribute"/> class.
        /// </summary>
        /// <param name="subject">The subject this configuration class will apply to.</param>
        public CruciblePackageConfigExtensionAttribute(Type subject)
        {
            this.SubjectType = subject;
        }

        /// <summary>
        /// Gets the subject this configuration extension applies to.
        /// </summary>
        public Type SubjectType { get; }
    }
}
