namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Books.RecipeBook;
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

            var panelUpdateIngredientsMethod = typeof(PotionCraftPanel.PotionCraftPanel).GetMethod("UpdateIngredientsList", BindingFlags.NonPublic | BindingFlags.Instance);
            var transpilePanelMethod = typeof(RecipeMapObjectAwakeEvent).GetMethod("TranspilePotionCraftPanelUpdateIngredientsList", BindingFlags.Static | BindingFlags.NonPublic);
            HarmonyInstance.Instance.Patch(panelUpdateIngredientsMethod, transpiler: new HarmonyMethod(transpilePanelMethod));

            var recipeUpdateIngredientsMethod = typeof(RecipeBookLeftPageContent).GetMethod("UpdateIngredientsList", BindingFlags.NonPublic | BindingFlags.Instance);
            var transpileRecipeMethod = typeof(RecipeMapObjectAwakeEvent).GetMethod("TranspileRecipeBookLeftPageContentUpdateIngredientsList", BindingFlags.Static | BindingFlags.NonPublic);
            HarmonyInstance.Instance.Patch(recipeUpdateIngredientsMethod, transpiler: new HarmonyMethod(transpileRecipeMethod));

            patchApplied = true;
        }

        private static string GetAtlasForUsedComponentIndex(int usedComponentIndex)
        {
            var component = Managers.Potion.usedComponents[usedComponentIndex];
            return GetAtlasForUsedComponent(component);
        }

        private static string GetAtlasForUsedComponent(Potion.UsedComponent component)
        {
            var e = new ScriptableObjectAtlasRequestEventArgs(component.componentObject);
            onAtlasRequest?.Invoke(null, e);
            return e.AtlasResult ?? Managers.TmpAtlas.settings.IngredientsAtlasName;
        }

        private static IEnumerable<CodeInstruction> TranspilePotionCraftPanelUpdateIngredientsList(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var instruction in instructions)
            {
                // TODO: We shouldn't trust that this code always uses loc.0 for this value...
                // Probably should look for ldstr "<voffset=0.1em><size=270%><sprite=\"", then skip over its stelem.ref, the array dup, and lcd.i4.3 which prepares the next array index.
                if (!found && instruction.opcode == OpCodes.Ldloc_0)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_3); // index
                    yield return new CodeInstruction(OpCodes.Call, typeof(IngredientsListResolveAtlasEvent).GetMethod("GetAtlasForUsedComponentIndex", BindingFlags.Static | BindingFlags.NonPublic));
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

        private static IEnumerable<CodeInstruction> TranspileRecipeBookLeftPageContentUpdateIngredientsList(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldloc_S && instruction.operand is LocalBuilder localBuilder && localBuilder.LocalIndex == 5 && localBuilder.LocalType == typeof(Potion.UsedComponent))
                {
                    found = true;

                    // We should now be right before the if statement checking if the current potion is in stock

                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5); // currentPotion
                    yield return new CodeInstruction(OpCodes.Call, typeof(IngredientsListResolveAtlasEvent).GetMethod("GetAtlasForUsedComponent", BindingFlags.Static | BindingFlags.NonPublic));
                    // Store the result into ingredientsAtlasName so it will be used in one of the two branching string constructions.
                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    // Return the first part of the if-check and continue as normal.
                    yield return instruction;

                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Debug.Log("[RoboPhredDev.PotionCraft.Crucible] Failed to inject atlas replacement for RecipeBookLeftPageContent!");
            }
        }
    }
}
