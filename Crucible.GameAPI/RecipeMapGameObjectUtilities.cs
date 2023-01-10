// <copyright file="RecipeMapGameObjectUtilities.cs" company="RoboPhredDev">
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


#if CRUCIBLE_BASES

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.ObjectBased.RecipeMap;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.DashedLine;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionBaseMapItem;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.Zones;
    using HarmonyLib;
    using RecipeMapZoneEditor;
    using UnityEngine;

    /// <summary>
    /// Utilities for working with recipe map game objects.
    /// </summary>
    public static class RecipeMapGameObjectUtilities
    {
        /// <summary>
        /// Ensures the supplied game object is a recipe map game object.
        /// </summary>
        /// <remarks>
        /// Valid recipe map game objects are objects that have a <see cref="RecipeMapPrefabController"/> component.
        /// </remarks>
        /// <param name="recipeMapGameObject">The game object to validate.</param>
        /// <param name="argumentName">The argument name to throw a <see cref="ArgumentException"/> for if the game object is not a valid recipe map object.</param>
        public static void EnsureRecipeMapObject(GameObject recipeMapGameObject, string argumentName = "recipeMapGameObject")
        {
            if (recipeMapGameObject.GetComponent<RecipeMapPrefabController>() == null)
            {
                throw new ArgumentException("The supplied game object is not a recipe map object.  Valid recipe map game objects have a RecipeMapPrefabController component.", argumentName);
            }
        }

        /// <summary>
        /// Executes the given action within the scope of the given recipe map.
        /// </summary>
        /// <param name="recipeMapGameObject">The recipe map game object within whose scope the action is executed.</param>
        /// <param name="action">The action to execute.</param>
        /// <remarks>
        /// Lots of recipe map item initialization logic requires the global current map variable to be set.  This function ensures
        /// this variable is set, and ensures it is reset back to its old value afterwards, even in the case of thrown exceptions.
        /// </remarks>
        public static void ExecuteInRecipeMapScope(GameObject recipeMapGameObject, Action action)
        {
            EnsureRecipeMapObject(recipeMapGameObject);

            var rmo = recipeMapGameObject.GetComponent<RecipeMapPrefabController>();
            var mapState = rmo.mapState;
            if (mapState.transform != recipeMapGameObject.transform)
            {
                throw new InvalidOperationException("The recipe map game object's transform does not match the map state's transform.");
            }

            var oldMapState = Managers.RecipeMap.currentMap;
            Managers.RecipeMap.currentMap = mapState;
            try
            {
                action();
            }
            finally
            {
                Managers.RecipeMap.currentMap = oldMapState;
            }
        }

        /// <summary>
        /// Clears out the recipe map from all known entities.
        /// </summary>
        /// <remarks>
        /// This will clear out all entities that are known to the base game.
        /// Custom objects added by mods might not be cleared out.
        /// </remarks>
        /// <param name="recipeMapGameObject">The game object to clear.  The game object must follow the base game's recipe map game object design and have a <see cref="RecipeMapObject"/> component.</param>
        public static void ClearMap(GameObject recipeMapGameObject)
        {
            EnsureRecipeMapObject(recipeMapGameObject, nameof(recipeMapGameObject));

            // Clear out map items such as effects, experience, vortecies, etc.
            foreach (var potionObject in recipeMapGameObject.GetComponentsInChildren<RecipeMapItem>())
            {
                // Leave the base.
                if (potionObject is PotionBaseMapItem)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(potionObject.gameObject);
            }

            // Clear out all the zones the game builds by default
            foreach (var zoneObject in recipeMapGameObject.GetComponentsInChildren<RecipeMapZoneContainer>())
            {
                var partsObjectTransform = zoneObject.gameObject.transform.Find("Parts");
                if (partsObjectTransform != null)
                {
                    for (var i = partsObjectTransform.childCount - 1; i >= 0; i--)
                    {
                        var pattern = partsObjectTransform.gameObject.transform.GetChild(i);
                        UnityEngine.Object.DestroyImmediate(pattern.gameObject);
                    }
                }
            }

            // Clear out any existing danger zone parts not in a pattern, such as those added by crucible.
            foreach (var part in recipeMapGameObject.GetComponentsInChildren<ZonePart>())
            {
                UnityEngine.Object.DestroyImmediate(part.gameObject);
            }
        }

        /// <summary>
        /// Reinitialize all data associated with the recipe map.
        /// <para>
        /// It is recommended that you call this function after adding or removing any child objects to the recipe map.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Map object child objects in the base game contain lots of cross-referenced data in order to function.  This function will
        /// scan through the child objects and ensure they are set up to work with the recipe map.
        /// </remarks>
        /// <param name="recipeMapGameObject">The recipe map game object to reinitialize.</param>
        public static void Reinitialize(GameObject recipeMapGameObject)
        {
            EnsureRecipeMapObject(recipeMapGameObject, nameof(recipeMapGameObject));

            ExecuteInRecipeMapScope(recipeMapGameObject, () =>
            {
                var rmo = recipeMapGameObject.GetComponent<RecipeMapPrefabController>();
                var mapState = rmo.mapState;

                var listOfLines = Traverse.Create(typeof(DashedLineMapItem)).Field<List<DashedLineMapItem>>("listOfLines").Value;
                foreach (var line in recipeMapGameObject.GetComponentsInChildren<DashedLineMapItem>())
                {
                    listOfLines.Remove(line);
                    UnityEngine.Object.DestroyImmediate(line.gameObject);
                }

                DashedLineMapItem.CreateLinesOnMap(mapState);
            });
        }
    }
}

#endif
