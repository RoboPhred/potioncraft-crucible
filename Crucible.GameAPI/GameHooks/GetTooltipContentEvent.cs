namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Reflection;
    using HarmonyLib;
    using ObjectBased.UIElements.Tooltip;

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

            HarmonyInstance.Instance.Patch(typeof(Ingredient).GetMethod("GetTooltipContent", BindingFlags.NonPublic | BindingFlags.Instance), prefix: new HarmonyMethod(typeof(GetTooltipContentEvent).GetMethod("IngredientGetTooltipContentPrefix", BindingFlags.Static | BindingFlags.NonPublic)));
            patchApplied = true;
        }

        private static bool IngredientGetTooltipContentPrefix(Ingredient __instance, ref TooltipContent __result)
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
