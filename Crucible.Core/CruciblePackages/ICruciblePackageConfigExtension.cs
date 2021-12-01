// <copyright file="ICruciblePackageConfigExtension.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.CruciblePackages
{
    /// <summary>
    /// Defines an extension configuration targeting a <see cref="CrucibleConfigSubjectObject"/>.
    /// </summary>
    /// <typeparam name="TSubject">The type produced by the targeted <see cref="CrucibleConfigSubjectObject"/>.</typeparam>
    /// <remarks>
    /// This interface should be used with <see cref="CruciblePackageConfigExtensionAttribute"/> to produce a class that loads extended
    /// configuration data from an existing <see cref="CrucibleConfigSubjectObject"/>.
    /// </remarks>
    public interface ICruciblePackageConfigExtension<in TSubject>
    {
        /// <summary>
        /// Apply this configuration node to the subject.
        /// </summary>
        /// <param name="subject">The subject created by the root configuration node.</param>
        void OnApplyConfiguration(TSubject subject);
    }
}
