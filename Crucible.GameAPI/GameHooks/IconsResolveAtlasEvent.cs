// <copyright file="IconsResolveAtlasEvent.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Books.RecipeBook;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides an event to override request for icons from the font atlas.
    /// </summary>
    public static class IconsResolveAtlasEvent
    {
        private static bool patchApplied = false;
        private static EventHandler<ScriptableObjectAtlasRequestEventArgs> onAtlasRequest;

        /// <summary>
        /// Raised when the atlas of a given item is being resolved.
        /// </summary>
        public static event EventHandler<ScriptableObjectAtlasRequestEventArgs> OnAtlasRequest
        {
            add
            {
                EnsurePatch();
                onAtlasRequest += value;
            }

            remove
            {
                onAtlasRequest -= value;
            }
        }

        private static void EnsurePatch()
        {
            if (patchApplied)
            {
                return;
            }

            patchApplied = true;

            var getLocalizedEffectsListMethod = AccessTools.Method(typeof(Potion), "GetLocalizedEffectsList");
            if (getLocalizedEffectsListMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate potion effects list function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(IconsResolveAtlasEvent), nameof(TranspilePotionGetLocalizedEffectsList));
                HarmonyInstance.Instance.Patch(getLocalizedEffectsListMethod, transpiler: new HarmonyMethod(transpiler));
            }

            var updateEffectsMethod = AccessTools.Method(typeof(RecipeBookLeftPageContent), "UpdateEffects");
            if (updateEffectsMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate potion effects list function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(IconsResolveAtlasEvent), nameof(TranspileRecipeBookLeftPageContentUpdateEffects));
                HarmonyInstance.Instance.Patch(updateEffectsMethod, transpiler: new HarmonyMethod(transpiler));
            }
        }

        private static string GetAtlasForPotionEffect(PotionEffect effect)
        {
            // Try resolving icon.
            var e = new ScriptableObjectAtlasRequestEventArgs(effect.icon);
            onAtlasRequest?.Invoke(null, e);
            if (e.AtlasResult != null)
            {
                return e.AtlasResult;
            }

            // Try resolving effect.
            e = new ScriptableObjectAtlasRequestEventArgs(effect);
            onAtlasRequest?.Invoke(null, e);
            if (e.AtlasResult != null)
            {
                return e.AtlasResult;
            }

            // Use default.
            return Managers.TmpAtlas.settings.IconsAtlasName;
        }

        private static IEnumerable<CodeInstruction> TranspilePotionGetLocalizedEffectsList(IEnumerable<CodeInstruction> instructions)
        {
            var getAtlasForUsedComponentIndexMethod = AccessTools.Method(typeof(IconsResolveAtlasEvent), nameof(GetAtlasForPotionEffect));
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldloc_0)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4); // potionEffectList2
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 12); // index
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<PotionEffect>), "get_Item", new[] { typeof(int) }));
                    yield return new CodeInstruction(OpCodes.Call, getAtlasForUsedComponentIndexMethod);

                    // Do not return ldloc_0, use our method return value instead.
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject atlas replacement for Potion.GetLocalizedEffectsList!");
            }
        }

        private static IEnumerable<CodeInstruction> TranspileRecipeBookLeftPageContentUpdateEffects(IEnumerable<CodeInstruction> instructions)
        {
            var getAtlasForUsedComponentIndexMethod = AccessTools.Method(typeof(IconsResolveAtlasEvent), nameof(GetAtlasForPotionEffect));
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldloc_S && instruction.operand is LocalBuilder builder && builder.LocalIndex == 7)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5); // source
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 12); // index
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<PotionEffect>), "get_Item", new[] { typeof(int) }));
                    yield return new CodeInstruction(OpCodes.Call, getAtlasForUsedComponentIndexMethod);

                    // Do not return ldloc_s&7, use our method return value instead.
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject atlas replacement for Potion.GetLocalizedEffectsList!");
            }
        }
    }
}
