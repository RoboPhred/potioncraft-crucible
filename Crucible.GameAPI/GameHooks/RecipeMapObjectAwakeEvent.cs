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
