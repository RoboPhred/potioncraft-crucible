// <copyright file="GetTooltipContentEvent.cs" company="RoboPhredDev">
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
    using global::PotionCraft.ObjectBased.UIElements.Tooltip;
    using global::PotionCraft.ScriptableObjects.Ingredient;
    using HarmonyLib;
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
