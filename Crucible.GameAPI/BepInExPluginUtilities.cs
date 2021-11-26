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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
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
        /// Gets the BepInEx plugin GUID for the mod implemented in the given assembly.
        /// </summary>
        /// <remarks>
        /// If more than one plugin is located in the assembly, the method will return null.
        /// </remarks>
        /// <param name="assembly">The assembly to identify the mod GUID from.</param>
        /// <returns>The plugin GUID of the BepInExPlugin contained in the assembly, or <c>null</c> if none was found.</returns>
        public static string GetPluginGuidFromAssembly(Assembly assembly)
        {
            var pluginAttributes = from type in assembly.GetTypes()
                                   let attribute = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin))
                                   where attribute != null
                                   select attribute;
            var found = pluginAttributes.ToArray();
            if (found.Length != 1)
            {
                return null;
            }

            return found[0].GUID;
        }

        /// <summary>
        /// Gets the BepInPlugin GUID for the assembly, or throw a <see cref="BepInPluginRequiredException"/> if none is found.
        /// </summary>
        /// <param name="assembly">The assembly to fetch the plugin GUID from.</param>
        /// <returns>The plugin GUID of the plugin the given assembly implements.</returns>
        public static string RequirePluginGuidFromAssembly(Assembly assembly)
        {
            var pluginAttributes = from type in assembly.GetTypes()
                                   let attribute = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin))
                                   where attribute != null
                                   select attribute;
            var found = pluginAttributes.ToArray();
            if (found.Length == 0)
            {
                throw new BepInPluginRequiredException("No class in the assembly contains the BepInPlugin attribute.");
            }

            if (found.Length > 1)
            {
                throw new BepInPluginRequiredException("The assembly contains more than one class with the BepInPlugin attribute.");
            }

            return found[0].GUID;
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
