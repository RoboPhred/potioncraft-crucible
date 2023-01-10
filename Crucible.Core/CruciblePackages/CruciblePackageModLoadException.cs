// <copyright file="CruciblePackageModLoadException.cs" company="RoboPhredDev">
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
    using System;

    /// <summary>
    /// An exception thrown when a failure is encountered while loading a <see cref="CruciblePackageMod"/>.
    /// </summary>
    public class CruciblePackageModLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePackageModLoadException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CruciblePackageModLoadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePackageModLoadException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CruciblePackageModLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets the path to the mod that encountered the exception.
        /// </summary>
        public string ModPath { get; set; }
    }
}
