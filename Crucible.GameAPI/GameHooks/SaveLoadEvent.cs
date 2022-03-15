// <copyright file="SaveLoadEvent.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using SaveFileSystem;
    using UnityEngine;

    /// <summary>
    /// Events for when the game saves and loads.
    /// </summary>
    public static class SaveLoadEvent
    {
        private static bool patchApplied = false;

        private static EventHandler<SaveLoadEventArgs> onGameSaved;

        private static EventHandler<SaveLoadEventArgs> onGameLoaded;

        /// <summary>
        /// Raised after the game saves.
        /// </summary>
        public static event EventHandler<SaveLoadEventArgs> OnGameSaved
        {
            add
            {
                EnsurePatches();
                onGameSaved += value;
            }

            remove
            {
                onGameSaved -= value;
            }
        }

        /// <summary>
        /// Raised after the game loads.
        /// </summary>
        public static event EventHandler<SaveLoadEventArgs> OnGameLoaded
        {
            add
            {
                EnsurePatches();
                onGameLoaded += value;
            }

            remove
            {
                onGameLoaded -= value;
            }
        }

        private static void EnsurePatches()
        {
            if (patchApplied)
            {
                return;
            }

            patchApplied = true;

            var loadLastProgressFromPool = AccessTools.Method(typeof(SaveLoadManager), nameof(SaveLoadManager.LoadLastProgressFromPool));
            if (loadLastProgressFromPool == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate game load function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(SaveLoadEvent), nameof(TranspileLoadLastProgressFromPool));
                HarmonyInstance.Instance.Patch(loadLastProgressFromPool, transpiler: new HarmonyMethod(transpiler));
            }

            var saveProgressToPool = AccessTools.Method(typeof(SaveLoadManager), nameof(SaveLoadManager.SaveProgressToPool));
            if (saveProgressToPool == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate game save function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(SaveLoadEvent), nameof(TranspileSaveProgressToPool));
                HarmonyInstance.Instance.Patch(saveProgressToPool, transpiler: new HarmonyMethod(transpiler));
            }
        }

        private static void RaiseGameLoaded(File file)
        {
            try
            {
                var e = new SaveLoadEventArgs(file);
                onGameLoaded?.Invoke(null, e);
            }
            catch (Exception)
            {
                Notification.ShowText("Load Error", "Crucible failed to load data from the save.  Data loss might occur if you continue playing.", Notification.TextType.EventText);
                throw;
            }
        }

        private static void RaiseGameSaved(SavePool pool)
        {
            try
            {
                // We dont get a direct reference to the file, but we can safely assume it was the
                // most recent file in the given pool.
                var file = FileStorage.GetNewestSuitableFromPool(pool);
                var e = new SaveLoadEventArgs(file);
                onGameSaved?.Invoke(null, e);
            }
            catch (Exception)
            {
                Notification.ShowText("Save Error", "Crucible failed to save data.  Data loss may occur.", Notification.TextType.EventText);
                throw;
            }
        }

        private static IEnumerable<CodeInstruction> TranspileLoadLastProgressFromPool(IEnumerable<CodeInstruction> instructions)
        {
            var raiseGameLoadedMethodInfo = AccessTools.Method(typeof(SaveLoadEvent), nameof(RaiseGameLoaded));

            var managersGetMenuMethodInfo = AccessTools.PropertyGetter(typeof(Managers), nameof(Managers.Menu));

            var found = false;
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo methodInfo && methodInfo == managersGetMenuMethodInfo)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0); // file
                    yield return new CodeInstruction(OpCodes.Call, raiseGameLoadedMethodInfo);
                }

                yield return instruction;
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject game loaded interceptor!");
            }
        }

        private static IEnumerable<CodeInstruction> TranspileSaveProgressToPool(IEnumerable<CodeInstruction> instructions)
        {
            var createNewFromStateMethodInfo = AccessTools.Method(typeof(File), nameof(File.CreateNewFromState));
            var raiseGameSavedMethodInfo = AccessTools.Method(typeof(SaveLoadEvent), nameof(RaiseGameSaved));
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo methodInfo && methodInfo == createNewFromStateMethodInfo)
                {
                    found = true;

                    // yield the instruction to call the function that saves the game.
                    yield return instruction;

                    // After saving the file, call our handler.
                    // We dont have a direct reference to the file, so we will infer it from the pool.
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // pool
                    yield return new CodeInstruction(OpCodes.Call, raiseGameSavedMethodInfo);
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject game save interceptor!");
            }
        }
    }
}
