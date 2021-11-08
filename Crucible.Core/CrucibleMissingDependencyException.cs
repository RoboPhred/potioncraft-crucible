// <copyright file="CrucibleMissingDependencyException.cs" company="RoboPhredDev">
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
    /// Indicates that a mod is missing a required dependency.
    /// </summary>
    public class CrucibleMissingDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleMissingDependencyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="modGuid">The GUID of the missing dependency.</param>
        /// <param name="semver">The semver for the required version of the missing dependency.</param>
        public CrucibleMissingDependencyException(string message, string modGuid, string semver)
            : base(message)
        {
            this.ModGuid = modGuid;
            this.RequiredSemver = semver;
        }

        /// <summary>
        /// Gets GUID of the missing dependency.
        /// </summary>
        public string ModGuid { get; }

        /// <summary>
        /// Gets the required version string for the missing dependency.
        /// </summary>
        public string RequiredSemver { get; }

        /// <summary>
        /// Creates an exception indicating a dependency is missing.
        /// </summary>
        /// <param name="sourceGuid">The guid of the mod whose dependency is missing.</param>
        /// <param name="dependencyGuid">The guid of the missing dependency.</param>
        /// <param name="semver">The semantic versioning filter for the missing dependency.</param>
        /// <returns>An exception describing the missing dependency.</returns>
        public static CrucibleMissingDependencyException CreateMissingDependencyException(string sourceGuid, string dependencyGuid, string semver)
        {
            return new CrucibleMissingDependencyException($"Mod \"{sourceGuid}\" is missing required dependency \"{dependencyGuid}\"@{semver}.", dependencyGuid, semver);
        }

        /// <summary>
        /// Creates an exception indicating a dependency is the wrong version.
        /// </summary>
        /// <param name="sourceGuid">The guid of the mod whose dependency is conflicting.</param>
        /// <param name="dependencyGuid">The guid of the conflicting dependency.</param>
        /// <param name="semver">The semantic versioning filter indicating the required version of the dependency.</param>
        /// <returns>An exception describing the missing dependency.</returns>
        public static CrucibleMissingDependencyException CreateBadVersionException(string sourceGuid, string dependencyGuid, string semver)
        {
            return new CrucibleMissingDependencyException($"Mod \"{sourceGuid}\" is incompatible with dependency \"{dependencyGuid}\".  The mod requires a version matching \"{semver}\".", dependencyGuid, semver);
        }
    }
}
