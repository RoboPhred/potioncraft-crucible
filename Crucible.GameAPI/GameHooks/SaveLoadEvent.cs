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
        // TODO: An event for saving the game.
        // Decide whether this should be before or after File is saved
        // to disk.  If before, we need a way to attach data to the File that will
        // save.  After might be simpler, as then we can append to the existing file.
        /*
        private static EventHandler<SaveLoadEventArgs> onGameSaved;
        */

        private static EventHandler<SaveLoadEventArgs> onGameLoaded;

        /*
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
        */

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
            var loadLastProgressFromPool = AccessTools.Method(typeof(SaveLoadManager), "LoadLastProgressFromPool");
            if (loadLastProgressFromPool == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate game load function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(SaveLoadEvent), nameof(TranspileLoadLastProgressFromPool));
                HarmonyInstance.Instance.Patch(loadLastProgressFromPool, transpiler: new HarmonyMethod(transpiler));
            }
        }

        private static void RaiseGameLoaded(File file)
        {
            var e = new SaveLoadEventArgs(file);
            onGameLoaded?.Invoke(null, e);
        }

        private static IEnumerable<CodeInstruction> TranspileLoadLastProgressFromPool(IEnumerable<CodeInstruction> instructions)
        {
            var raiseGameLoadedMethodInfo = AccessTools.Method(typeof(SaveLoadEvent), nameof(RaiseGameLoaded));

            var managersGetMenuMethodInfo = AccessTools.PropertyGetter(typeof(Managers), "Menu");

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
    }
}
