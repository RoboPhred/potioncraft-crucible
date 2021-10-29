namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Reflection;
    using HarmonyLib;

    /// <summary>
    /// Provides an event to be notified when the <see cref="RecipeMapObject"/> is awakened.
    /// </summary>
    public static class RecipeMapObjectAwakeEvent
    {
        private static bool patchApplied = false;
        private static EventHandler onRecipeMapObjectAwake;

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

            HarmonyInstance.Instance.Patch(typeof(RecipeMapObject).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance), postfix: new HarmonyMethod(typeof(RecipeMapObjectAwakeEvent).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
            patchApplied = true;
        }

        private static void Postfix()
        {
            onRecipeMapObjectAwake?.Invoke(null, EventArgs.Empty);
        }
    }
}