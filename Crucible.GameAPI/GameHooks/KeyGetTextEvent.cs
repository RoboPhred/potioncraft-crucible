namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using HarmonyLib;
    using LocalizationSystem;
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

            var getTextMethod = AccessTools.Method(typeof(Key), "GetText");
            if (getTextMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find Key GetText function!");
            }
            else
            {
                var prefixMethod = AccessTools.Method(typeof(KeyGetTextEvent), nameof(Prefix));
                HarmonyInstance.Instance.Patch(getTextMethod, prefix: new HarmonyMethod(prefixMethod));
            }

            patchApplied = true;
        }

#pragma warning disable SA1313
        private static bool Prefix(Key __instance, ref string __result)
#pragma warning restore SA1313
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
