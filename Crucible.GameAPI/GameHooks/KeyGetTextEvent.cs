namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Reflection;
    using HarmonyLib;
    using LocalizationSystem;

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

            var getTextMethod = typeof(Key).GetMethod("GetText", BindingFlags.Public | BindingFlags.Instance);
            var prefixMethod = typeof(KeyGetTextEvent).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
            HarmonyInstance.Instance.Patch(getTextMethod, prefix: new HarmonyMethod(prefixMethod));
            patchApplied = true;
        }

        private static bool Prefix(Key __instance, ref string __result)
        {
            var key = Traverse.Create(__instance).Field<string>("key").Value;
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
