// <copyright file="CrucibleDependencyConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// A configuration entry declaring a dependency.
    /// </summary>
    [DuckTypeCandidate(typeof(CrucibleBepInExDependencyConfig))]
    public abstract class CrucibleDependencyConfig : CrucibleConfigNode
    {
        /// <summary>
        /// Ensures that the dependency is met.
        /// If the dependency is not met, this should throw a <see cref="CrucibleMissingDependencyException"/>.
        /// </summary>
        public abstract void EnsureDependencyMet();
    }
}
