// <copyright file="RecipeMapObjectAwakeEvent.cs" company="RoboPhredDev">
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
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides an event to be notified when the <see cref="RecipeMapObject"/> is awakened.
    /// </summary>
    public static class RecipeMapObjectAwakeEvent
    {
        private static bool patchApplied = false;
        private static EventHandler onRecipeMapObjectAwake;

        /// <summary>
        /// Raised when <see cref="RecipeMapObject"/> has finished awakening.
        /// </summary>
        public static event EventHandler OnRecipeMapObjectAwake
        {
            add
            {
                EnsurePatch();
                onRecipeMapObjectAwake += value;
            }

            remove
            {
                onRecipeMapObjectAwake -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (patchApplied)
            {
                return;
            }

            var recipeMapObjectAwakeMethodInfo = AccessTools.Method(typeof(RecipeMapObject), "Awake");
            if (recipeMapObjectAwakeMethodInfo == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find RecipeMapObject Awake function!");
            }
            else
            {
                var postfix = AccessTools.Method(typeof(RecipeMapObjectAwakeEvent), nameof(Postfix));
                HarmonyInstance.Instance.Patch(recipeMapObjectAwakeMethodInfo, postfix: new HarmonyMethod(postfix));
            }

            patchApplied = true;
        }

        private static void Postfix()
        {
            onRecipeMapObjectAwake?.Invoke(null, EventArgs.Empty);
        }
    }
}
