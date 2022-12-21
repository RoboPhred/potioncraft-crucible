// <copyright file="KeyGetTextEvent.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using global::PotionCraft.LocalizationSystem;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides an event to intercept and override the return value of <see cref="Key.GetText"/>.
    /// </summary>
    public static class KeyGetTextEvent
    {
        private static bool patchApplied = false;
        private static EventHandler<KeyGetTextEventArgs> onGetText;

        /// <summary>
        /// Raised when the text for a localization key is resolving.
        /// </summary>
        public static event EventHandler<KeyGetTextEventArgs> OnGetText
        {
            add
            {
                EnsurePatch();
                onGetText += value;
            }

            remove
            {
                onGetText -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (patchApplied)
            {
                return;
            }

            var getTextMethod = AccessTools.Method(typeof(LocalizationManager), "GetText", new[] { typeof(string), typeof(LocalizationManager.Locale) });
            if (getTextMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find LocalizationManager GetText function!");
            }
            else
            {
                var prefixMethod = AccessTools.Method(typeof(KeyGetTextEvent), nameof(GetTextPrefix));
                HarmonyInstance.Instance.Patch(getTextMethod, prefix: new HarmonyMethod(prefixMethod));
            }

            var containsKeyMethod = AccessTools.Method(typeof(LocalizationManager), "ContainsKey", new[] { typeof(string) });
            if (containsKeyMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find LocalizationManager ContainsKey function!");
            }
            else
            {
                var prefixMethod = AccessTools.Method(typeof(KeyGetTextEvent), nameof(ContainsKeyPrefix));
                HarmonyInstance.Instance.Patch(containsKeyMethod, prefix: new HarmonyMethod(prefixMethod));
            }

            patchApplied = true;
        }

#pragma warning disable SA1313
        private static bool GetTextPrefix(LocalizationManager __instance, string key, ref string __result)
#pragma warning restore SA1313
        {
            var e = new KeyGetTextEventArgs(key);
            onGetText?.Invoke(__instance, e);
            if (e.Result != null)
            {
                __result = e.Result;
                return false;
            }

            return true;
        }

#pragma warning disable SA1313
        private static bool ContainsKeyPrefix(LocalizationManager __instance, string key, ref bool __result)
#pragma warning restore SA1313
        {
            var e = new KeyGetTextEventArgs(key);
            onGetText?.Invoke(__instance, e);
            if (e.Result != null)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
