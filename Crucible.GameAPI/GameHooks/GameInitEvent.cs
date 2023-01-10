// <copyright file="GameInitEvent.cs" company="RoboPhredDev">
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
    using global::PotionCraft.ManagersSystem.Game;
    using global::PotionCraft.SceneLoader;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides an event to be notified when the game has finished initialization.
    /// </summary>
    public static class GameInitEvent
    {
        private static bool patchApplied = false;
        private static bool loaded = false;
        private static EventHandler onLoadEvent;

        /// <summary>
        /// Raised when <see cref="RecipeMapObject"/> has finished awakening.
        /// </summary>
        public static event EventHandler OnGameInitialized
        {
            add
            {
                EnsurePatch();
                onLoadEvent += value;
                if (loaded)
                {
                    value.Invoke(null, EventArgs.Empty);
                }
            }

            remove
            {
                onLoadEvent -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (patchApplied)
            {
                return;
            }

            var gameManagerStartMethodInfo = AccessTools.Method(typeof(GameManager), "Start");
            if (gameManagerStartMethodInfo == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find GameManager Start function!");
            }
            else
            {
                var postfix = AccessTools.Method(typeof(GameInitEvent), nameof(Postfix));
                HarmonyInstance.Instance.Patch(gameManagerStartMethodInfo, postfix: new HarmonyMethod(postfix));
            }

            patchApplied = true;
        }

        private static void Postfix()
        {
            // We want init to run after the new game state has been stored.
            ObjectsLoader.AddLast("SaveLoadManager.SaveNewGameState", () => GameInitEvent.OnLoaded());
            Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Registered game load event.");
        }

        private static void OnLoaded()
        {
            Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Game load event fired.");
            loaded = true;
            onLoadEvent?.Invoke(null, EventArgs.Empty);
        }
    }
}
