// <copyright file="BepInPluginRequiredException.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;

    /// <summary>
    /// Thrown when a required BepInPlugin is not found.
    /// </summary>
    public class BepInPluginRequiredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BepInPluginRequiredException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public BepInPluginRequiredException(string message)
            : base(message)
        {
        }
    }
}
