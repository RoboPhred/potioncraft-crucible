// <copyright file="IngredientsListResolveAtlasEvent.cs" company="RoboPhredDev">
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
    using System.Reflection;
    using System.Reflection.Emit;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.ManagersSystem.Potion;
    using global::PotionCraft.ManagersSystem.Potion.Entities;
    using global::PotionCraft.ManagersSystem.TMP;
    using global::PotionCraft.ObjectBased.UIElements.Books.RecipeBook;
    using global::PotionCraft.ObjectBased.UIElements.PotionCraftPanel;
    using global::PotionCraft.ScriptableObjects.Potion;
    using global::PotionCraft.Settings;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides an event to override the atlas name of items in the ingredients list.
    /// </summary>
    public static class IngredientsListResolveAtlasEvent
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

            var panelUpdateIngredientsMethod = AccessTools.Method(typeof(PotionCraftPanel), "UpdateIngredientsList");
            if (panelUpdateIngredientsMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate potion craft panel ingredients list update function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(IngredientsListResolveAtlasEvent), nameof(TranspilePotionCraftPanelUpdateIngredientsList));
                HarmonyInstance.Instance.Patch(panelUpdateIngredientsMethod, transpiler: new HarmonyMethod(transpiler));
            }

            var recipeUpdateIngredientsMethod = AccessTools.Method(typeof(PotionManager), "GetLocalizedReagentsList");
            if (recipeUpdateIngredientsMethod == null)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to locate potion manager get localized reagents list function!");
            }
            else
            {
                var transpiler = AccessTools.Method(typeof(IngredientsListResolveAtlasEvent), nameof(TranspilePotionManagerGetLocalizedReagentsList));
                HarmonyInstance.Instance.Patch(recipeUpdateIngredientsMethod, transpiler: new HarmonyMethod(transpiler));
            }

            /* TODO: patch Ingredient.SpawnCollectedItemText */
        }

        private static string GetAtlasForCurrentPotionUsedComponentIndex(int usedComponentIndex)
        {
            var component = Managers.Potion.PotionUsedComponents.GetSummaryComponents()[usedComponentIndex];
            return GetAtlasForUsedComponent(component);
        }

        private static string GetAtlasForUsedComponent(AlchemySubstanceComponent component)
        {
            return GetAtlasForScriptableObject(component.Component);
        }

        private static string GetAtlasForPotionUsedComponentIndex(Potion potion, int usedComponentIndex)
        {
            var component = potion.usedComponents.GetSummaryComponents()[usedComponentIndex];
            return GetAtlasForUsedComponent(component);
        }

        private static string GetAtlasForScriptableObject(ScriptableObject scriptableObject)
        {
            var e = new ScriptableObjectAtlasRequestEventArgs(scriptableObject);
            onAtlasRequest?.Invoke(null, e);
            return e.AtlasResult ?? Settings<TMPManagerSettings>.Asset.IngredientsAtlasName;
        }

        private static IEnumerable<CodeInstruction> TranspilePotionCraftPanelUpdateIngredientsList(IEnumerable<CodeInstruction> instructions)
        {
            var getAtlasForUsedComponentIndexMethod = AccessTools.Method(typeof(IngredientsListResolveAtlasEvent), nameof(GetAtlasForCurrentPotionUsedComponentIndex));
            var found = false;
            foreach (var instruction in instructions)
            {
                // TODO: We shouldn't trust that this code always uses loc.0 for this value...
                // Probably should look for ldstr "<voffset=0.1em><size=270%><sprite=\"", then skip over its stelem.ref, the array dup, and lcd.i4.3 which prepares the next array index.
                if (!found && instruction.opcode == OpCodes.Ldloc_0)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4); // index
                    yield return new CodeInstruction(OpCodes.Call, getAtlasForUsedComponentIndexMethod);
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject atlas replacement for PotionCraftPanel!");
            }
        }

        private static IEnumerable<CodeInstruction> TranspilePotionManagerGetLocalizedReagentsList(IEnumerable<CodeInstruction> instructions)
        {
            // IL_01b4: ldfld   string PotionCraft.ManagersSystem.TMP.TMPManagerSettings::IngredientsAtlasName
            var getAtlasForUsedComponentMethod = AccessTools.Method(typeof(IngredientsListResolveAtlasEvent), nameof(GetAtlasForUsedComponent));
            var found = false;
            CodeInstruction lastInstruction = null;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldfld
                    && instruction.operand is FieldInfo fieldInfo
                    && fieldInfo.FieldType == typeof(string)
                    && fieldInfo.Name.Equals("IngredientsAtlasName"))
                {
                    // We should now be right before the assignment of IngredientsAtlasName to str9
                    found = true;

                    // Set the last instruction to null so it isn't returned in the next iteration
                    lastInstruction = null;

                    // Put the current used component on the call stack and call getAtlasForUsedComponent to return the correct atlas for the component
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 16); // substanceComponent1
                    yield return new CodeInstruction(OpCodes.Call, getAtlasForUsedComponentMethod);

                    // Do not yield return instruction.
                    // Leave our result on the call stack to take its place.
                }
                else
                {
                    // Only return the last instruction since we need to modify a set of two instructions
                    if (lastInstruction != null)
                    {
                        yield return lastInstruction;
                    }

                    lastInstruction = instruction;
                }
            }

            // Return the last instruction since we didn't do that as part of the loop
            if (lastInstruction != null)
            {
                yield return lastInstruction;
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject atlas replacement for PotionManager.GetLocalizedReagentsList!");
            }
        }
    }
}
