// <copyright file="BepInExPluginUtilities.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BepInEx;

    /// <summary>
    /// Utilities for inspecting BepInEx plugins.
    /// </summary>
    public static class BepInExPluginUtilities
    {
        private static bool isInitialized;
        private static BepInPlugin[] cachedPlugins;

        /// <summary>
        /// Gets all BepInEx plugins.
        /// </summary>
        /// <returns>An enumerable of all loaded BepInEx plugins.</returns>
        public static IEnumerable<BepInPlugin> GetAllPlugins()
        {
            EnsureInitialized();

            if (cachedPlugins == null)
            {
                var plugins = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assembly.GetTypes()
                              let attribute = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin))
                              where attribute != null
                              select attribute;
                cachedPlugins = plugins.ToArray();
            }

            return cachedPlugins;
        }

        /// <summary>
        /// Gets the BepInEx mod GUID for the mod implemented in the given assembly.
        /// </summary>
        /// <remarks>
        /// If more than one mod is located in the assembly, the returned GUID may be unpredictable.
        /// </remarks>
        /// <param name="assembly">The assembly to identify the mod GUID from.</param>
        /// <returns>The mod GUID of the BepInExPlugin contained in the assembly, or <c>null</c> if none was found.</returns>
        public static string GetModGuidFromAssembly(Assembly assembly)
        {
            var pluginAttributes = from type in assembly.GetTypes()
                                   let attribute = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin))
                                   where attribute != null
                                   select attribute;
            var bepAttr = pluginAttributes.FirstOrDefault();
            if (bepAttr != null)
            {
                return bepAttr.GUID;
            }

            return null;
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            AppDomain.CurrentDomain.AssemblyLoad += (_, __) => cachedPlugins = null;
        }
    }
}
