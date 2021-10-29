namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Reflection;
    using HarmonyLib;
    using LocalizationSystem;

    internal static class KeyGetTextEvent
    {
        private static bool patchApplied = false;
        private static EventHandler<KeyGetTextEventArgs> onGetText;

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

            HarmonyInstance.Instance.Patch(typeof(Key).GetMethod("GetText", BindingFlags.NonPublic | BindingFlags.Instance), prefix: new HarmonyMethod(typeof(RecipeMapObjectAwakeEvent).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
            patchApplied = true;
        }

        static bool Prefix(Key __instance, ref string __result)
        {
            var key = new Traverse(__instance).Field<string>("key").Value;
            var e = new KeyGetTextEventArgs(key);
            onGetText?.Invoke(__instance, e);
            if (e.Result != null)
            {
                __result = e.Result;
                return false;
            }

            return true;
        }
    }
}
