namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using HarmonyLib;
    using ObjectBased.UIElements.Tooltip;
    using UnityEngine;

    /// <summary>
    /// Provides an event to resolve <see cref="TooltipContent"/>s.
    /// </summary>
    public static class GetTooltipContentEvent
    {
        private static bool patchApplied = false;
        private static EventHandler<GetTooltipContentEventArgs<Ingredient>> onIngredientTooltipRequested;

        /// <summary>
        /// Raised when a tooltip is requested for an ingredient.
        /// </summary>
        public static event EventHandler<GetTooltipContentEventArgs<Ingredient>> OnIngredientTooltipRequested
        {
            add
            {
                EnsurePatch();
                onIngredientTooltipRequested += value;
            }

            remove
            {
                onIngredientTooltipRequested -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (patchApplied)
            {
                return;
            }

            var getTooltipContentMethod = AccessTools.Method(typeof(Ingredient), "GetTooltipContent");
            if (getTooltipContentMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to find Ingredient GetTooltipContent function!");
            }
            else
            {
                var prefixMethod = AccessTools.Method(typeof(GetTooltipContentEvent), nameof(IngredientGetTooltipContentPrefix));
                HarmonyInstance.Instance.Patch(getTooltipContentMethod, prefix: new HarmonyMethod(prefixMethod));
            }

            patchApplied = true;
        }

#pragma warning disable SA1313
        private static bool IngredientGetTooltipContentPrefix(Ingredient __instance, ref TooltipContent __result)
#pragma warning restore SA1313
        {
            var e = new GetTooltipContentEventArgs<Ingredient>(__instance);
            onIngredientTooltipRequested?.Invoke(__instance, e);
            if (e.TooltipContent != null)
            {
                __result = e.TooltipContent;
                return false;
            }

            return true;
        }
    }
}
