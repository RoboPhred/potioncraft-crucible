namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Reflection;
    using HarmonyLib;

    internal static class RecipeMapObjectAwakeEvent
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

        static void Postfix(RecipeMapObject __instance)
        {
            onRecipeMapObjectAwake?.Invoke(__instance, EventArgs.Empty);
        }
    }
}