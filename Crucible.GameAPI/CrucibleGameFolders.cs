// <copyright file="CrucibleGameFolders.cs" company="RoboPhredDev">
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
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Provides common game folder locations.
    /// </summary>
    public static class CrucibleGameFolders
    {
        // Untested, and neither the potioncraft executable nor the assembly dll have any company or product name specified.
        /*
        /// <summary>
        /// Gets the path to the folder containing user data, such as logs and saves.
        /// </summary>
        public static string GameUserDataPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", versionInfo.CompanyName, versionInfo.ProductName);
            }
        }
        */
    }
}
