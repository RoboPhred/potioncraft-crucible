// <copyright file="CrucibleLocalization.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// Tools for registering keys and values in PotionCraft's localization system.
    /// </summary>
    public static class CrucibleLocalization
    {
        private static readonly Dictionary<string, string> Localization = new();
        private static bool initialized = false;

        /// <summary>
        /// Sets the value of a localization key.
        /// If the key is already registered by the game or another mod, it will be replaced.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to use for all languages.</param>
        public static void SetLocalizationKey(string key, string value)
        {
            TryInitialize();
            Localization[key] = value;
        }

        private static void TryInitialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            KeyGetTextEvent.OnGetText += (_, e) =>
            {
                if (Localization.TryGetValue(e.Key, out var value))
                {
                    e.Result = value;
                }
            };
        }
    }
}
