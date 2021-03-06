// <copyright file="CruciblePotionBase.cs" company="RoboPhredDev">
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

#if ENABLE_POTION_BASE

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using LocalizationSystem;
    using ObjectBased.RecipeMap;
    using ObjectOptimizationSystem;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="PotionBase"/>s.
    /// </summary>
    public sealed class CruciblePotionBase
    {
        private static readonly HashSet<MapState> CustomMapStates = new();
        private static readonly HashSet<PotionBase> AtlasOverriddenPotionBases = new();
        private static CrucibleSpriteAtlas spriteAtlas;
        private static GameObject blankMapPrefab;

        private readonly MapState mapState;

        static CruciblePotionBase()
        {
            // Custom map states may not have existed when the current save was last
            // saved, so we need to initialize their fog
            CrucibleGameEvents.OnSaveLoaded += (_, __) =>
            {
                foreach (var mapState in CustomMapStates)
                {
                    ClearInitialFog(mapState);
                }
            };
        }

        private CruciblePotionBase(MapState mapState)
        {
            this.mapState = mapState;
        }

        /// <summary>
        /// Gets the game object for the map of this potion base.
        /// </summary>
        public GameObject MapGameObject
        {
            get
            {
                return this.mapState.transform.gameObject;
            }
        }

        /// <summary>
        /// Gets the potion base controlled by this api object.
        /// </summary>
        public PotionBase PotionBase
        {
            get
            {
                return this.mapState.potionBase;
            }
        }

        /// <summary>
        /// Gets the internal ID of this potion base.
        /// </summary>
        public string ID
        {
            get => this.mapState.potionBase.name;
        }

        /// <summary>
        /// Gets or sets the name of this potion base in the potion base menu.
        /// </summary>
        public string Name
        {
            get => new Key(this.LocalizationKey).GetText();
            set
            {
                CrucibleLocalization.SetLocalizationKey(this.LocalizationKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the description of this potion base in the potion base menu.
        /// </summary>
        public string Description
        {
            get => new Key($"{this.LocalizationKey}_description").GetText();
            set
            {
                CrucibleLocalization.SetLocalizationKey($"{this.LocalizationKey}_description", value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the liquid when using this base.
        /// </summary>
        /// <remarks>
        /// This color will be used for the liquid in the potion bottle and the crucible.
        /// </remarks>
        public Color LiquidColor
        {
            get
            {
                return this.mapState.potionBase.baseColor;
            }

            set
            {
                this.mapState.potionBase.baseColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite to use for this potion base in ingredient lists.
        /// </summary>
        public Sprite IngredientListIcon
        {
            get
            {
                return this.mapState.potionBase.smallIconSprite;
            }

            set
            {
                if (!value.texture.isReadable)
                {
                    throw new ArgumentException("IngredientListIcon can only be set to sprites with readable textures.  The texture data must be available to bake into a sprite atlas.");
                }

                if (value.packed)
                {
                    throw new ArgumentException("IngredientListIcon only supports sprites that are not derived from sprite sheets.  The texture data must be available to bake into a sprite atlas.");
                }

                // TODO: Set up the custom atlas for this.
                this.mapState.potionBase.smallIconSprite = value;

                SetPotionBaseIcon(this.mapState.potionBase, value.texture);
            }
        }

        /// <summary>
        /// Gets or sets the icon to use on the potion base selection menu.
        /// </summary>
        public Sprite MenuIcon
        {
            get
            {
                return this.mapState.potionBase.recipeMapIconSprite;
            }

            set
            {
                this.mapState.potionBase.recipeMapIconSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon to use in the potion base menu button when the potion base is selected.
        /// </summary>
        public Sprite MenuSelectedIcon
        {
            get
            {
                return this.mapState.potionBase.markerIconIdleSprite;
            }

            set
            {
                this.mapState.potionBase.markerIconIdleSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon to use when hovering over the potion base in the menu.
        /// </summary>
        public Sprite MenuHoverIcon
        {
            get
            {
                return this.mapState.potionBase.markerIconHoverSprite;
            }

            set
            {
                this.mapState.potionBase.markerIconHoverSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon to use when the potion base is locked in the menu.
        /// </summary>
        public Sprite MenuLockedIcon
        {
            get
            {
                return this.mapState.potionBase.markerIconLockedSprite;
            }

            set
            {
                this.mapState.potionBase.markerIconLockedSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the artwork to use on the potion base tooltip.
        /// </summary>
        public Sprite TooltipIcon
        {
            get
            {
                return this.mapState.potionBase.tooltipIconSprite;
            }

            set
            {
                this.mapState.potionBase.tooltipIconSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon to use for the ladle when this potion base is selected.
        /// </summary>
        public Sprite LadleIcon
        {
            get
            {
                return this.mapState.potionBase.ladleIconSprite;
            }

            set
            {
                this.mapState.potionBase.ladleIconSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon used on the potion base map.
        /// </summary>
        public Sprite MapIcon
        {
            get
            {
                return this.mapState.potionBase.mapItemSprite;
            }

            set
            {
                this.mapState.potionBase.mapItemSprite = value;

                var potionBaseItem = this.MapGameObject.GetComponentInChildren<PotionBaseMapItem>();
                if (potionBaseItem)
                {
                    Traverse.Create(potionBaseItem).Method("UpdateSprites").GetValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon to use for this potion base in the recipe map steps list.
        /// </summary>
        public Sprite RecipeStepIcon
        {
            get
            {
                return this.mapState.potionBase.recipeMarkIcon;
            }

            set
            {
                this.mapState.potionBase.recipeMarkIcon = value;
            }
        }

        private string LocalizationKey
        {
            get
            {
                return $"potion_base_{this.ID.ToLowerInvariant().Replace(" ", "_")}";
            }
        }

        /// <summary>
        /// Creates a new potion base with the given id.
        /// </summary>
        /// <param name="id">The id of the potion base to create.</param>
        /// <returns>The potion base api object for the created potion base.</returns>
        public static CruciblePotionBase CreatePotionBase(string id)
        {
            if (GetPotionBaseById(id) != null)
            {
                throw new ArgumentException($"A base with id \"{id}\" already exists.", nameof(id));
            }

            var waterBase = Array.Find(Managers.RecipeMap.potionBasesSettings.potionBases, x => x.name == "Water");

            var newBase = ScriptableObject.CreateInstance<PotionBase>();
            Managers.RecipeMap.potionBasesSettings.potionBases = Managers.RecipeMap.potionBasesSettings.potionBases.Concat(new[] { newBase }).ToArray();

            newBase.name = id;
            newBase.mapPrefab = GetBlankMapPrefab();

            newBase.baseColor = waterBase.baseColor;
            newBase.smallIconSprite = waterBase.smallIconSprite;
            newBase.markerIconHoverSprite = waterBase.markerIconHoverSprite;
            newBase.markerIconIdleSprite = waterBase.markerIconIdleSprite;
            newBase.markerIconLockedSprite = waterBase.markerIconLockedSprite;
            newBase.tooltipIconSprite = waterBase.tooltipIconSprite;
            newBase.ladleIconSprite = waterBase.ladleIconSprite;
            newBase.recipeMarkIcon = waterBase.recipeMarkIcon;
            newBase.mapItemSprite = waterBase.mapItemSprite;
            newBase.recipeMapIconSprite = waterBase.recipeMapIconSprite;

            var index = MapLoader.loadedMaps.Count;
            var mapState = new MapState
            {
                index = index,
                zoom = Managers.RecipeMap.settings.zoomSettings.defaultZoom,
                potionBase = newBase,
            };
            MapLoader.loadedMaps.Add(mapState);

            var oldCurrentMap = Managers.RecipeMap.currentMap;
            Managers.RecipeMap.currentMap = mapState;
            try
            {
                mapState.transform = Traverse.Create(typeof(MapLoader)).Method("InstantiateMap", new[] { typeof(int) }).GetValue<Transform>(index);
                mapState.transform.gameObject.name = $"Crucible RecipeMap {id}";
                mapState.transform.GetComponent<RecipeMapPrefabController>().mapState = mapState;

                // We keep the prefab inactive to stop logic running on it.
                // Now that we instantiated it, we need to make it active to let all that logic run on the new map.
                mapState.transform.gameObject.SetActive(true);

                mapState.UpdateMapBounds();

                Managers.RecipeMap.fogOfWar.InitializeMap(index);

                RegisterMapStatePhysics(mapState);
            }
            finally
            {
                Managers.RecipeMap.currentMap = oldCurrentMap;
            }

            CustomMapStates.Add(mapState);

            return new CruciblePotionBase(mapState);
        }

        /// <summary>
        /// Gets the potion base by id.
        /// </summary>
        /// <param name="id">The ID of the potion base to fetch.</param>
        /// <returns>The potion base if found, or null if no potion base exists with the given id.</returns>
        public static CruciblePotionBase GetPotionBaseById(string id)
        {
            var mapState = MapLoader.loadedMaps.Find(x => x.potionBase.name == id);
            if (mapState == null)
            {
                return null;
            }

            return new CruciblePotionBase(mapState);
        }

        /// <summary>
        /// Clears the recipe map of all entities.
        /// </summary>
        /// <remarks>
        /// This will remove all entities known to Crucible from the map.  Entities added by other mods may not be removed.
        /// </remarks>
        public void ClearMap()
        {
            RecipeMapGameObjectUtilities.ClearMap(this.MapGameObject);
            this.Reinitialize();
        }

        /// <summary>
        /// Reinitialize entities in the recipe map.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This function scans the map entities and ensures that they are properly linked with the recipe map.
        /// </para>
        /// <para>
        /// This should be called after adding or removing game objects on the recipe map.
        /// </para>
        public void Reinitialize()
        {
            RecipeMapGameObjectUtilities.Reinitialize(this.MapGameObject);
        }

        /// <summary>
        /// Unlocks the potion base and makes it available for use.
        /// </summary>
        public void GiveToPlayer()
        {
            Managers.RecipeMap.potionBaseSubManager.UnlockPotionBase(this.mapState.potionBase);
        }

        private static GameObject GetBlankMapPrefab()
        {
            if (blankMapPrefab == null)
            {
                var prefab = MapLoader.loadedMaps[0].potionBase.mapPrefab;
                var wasActive = prefab.activeSelf;

                // Clone the prefab as inactive, otherwise lots of map logic will try to initialize.
                prefab.SetActive(false);
                var newMap = UnityEngine.Object.Instantiate(prefab, Managers.RecipeMap.recipeMapObject.mapsContainer);
                prefab.SetActive(wasActive);

                newMap.name = "Crucible RecipeMap Prefab";

                RecipeMapGameObjectUtilities.ClearMap(newMap);

                blankMapPrefab = newMap;
            }

            return blankMapPrefab;
        }

        /// <summary>
        /// Initialize the map physics optimizer data.
        /// </summary>
        /// <seealso cref="RecipeMapPhysicsOptimizer.Initialize"/>
        /// <param name="mapState">The map state to initialize physics data for.</param>
        private static void RegisterMapStatePhysics(MapState mapState)
        {
            var physicsOptimizerTraverse = Traverse.Create(typeof(RecipeMapPhysicsOptimizer));

            // Note: If we (or potioncraft itself) ever support varying map sizes, this logic will need to be reset if crucible changes the size.
            Vector2 vector2 = 0.5f * Managers.RecipeMap.currentMap.mapBgRect.size;
            Rect rect = Rect.MinMaxRect(-vector2.x, -vector2.y, vector2.x, vector2.y);

            physicsOptimizerTraverse.Field<Dictionary<MapState, Rect>>("gridRectDictionary").Value.Add(mapState, rect);

            var defaultCellWidth = physicsOptimizerTraverse.Field<float>("defaultCellWidth").Value;
            var defaultCellHeight = physicsOptimizerTraverse.Field<float>("defaultCellHeight").Value;
            var cellsX = Mathf.CeilToInt(rect.width / defaultCellWidth);
            var cellsY = Mathf.CeilToInt(rect.height / defaultCellHeight);
            physicsOptimizerTraverse.Field<Dictionary<MapState, int>>("columnsCountDictionary").Value.Add(mapState, cellsX);
            physicsOptimizerTraverse.Field<Dictionary<MapState, int>>("rowsCountDictionary").Value.Add(mapState, cellsY);
            physicsOptimizerTraverse.Field<Dictionary<MapState, float>>("cellWidthDictionary").Value.Add(mapState, rect.width / cellsX);
            physicsOptimizerTraverse.Field<Dictionary<MapState, float>>("cellHeightDictionary").Value.Add(mapState, rect.height / cellsY);

            var objectOptimizerTargetSetArray = new HashSet<IRecipeMapObjectOptimizerTarget>[cellsY][];
            for (int cellY = 0; cellY < cellsY; ++cellY)
            {
                objectOptimizerTargetSetArray[cellY] = new HashSet<IRecipeMapObjectOptimizerTarget>[cellsX];
                for (int cellX = 0; cellX < cellsX; ++cellX)
                {
                    objectOptimizerTargetSetArray[cellY][cellX] = new HashSet<IRecipeMapObjectOptimizerTarget>();
                }
            }

            physicsOptimizerTraverse.Field<Dictionary<MapState, HashSet<IRecipeMapObjectOptimizerTarget>[][]>>("gridDictionary").Value.Add(mapState, objectOptimizerTargetSetArray);

            physicsOptimizerTraverse.Field<Dictionary<MapState, HashSet<IRecipeMapObjectOptimizerTarget>>>("optimizerTargetsDictionary").Value.Add(mapState, new HashSet<IRecipeMapObjectOptimizerTarget>());
            physicsOptimizerTraverse.Field<Dictionary<MapState, HashSet<IRecipeMapObjectOptimizerTarget>>>("disabledOptimizerTargetsDictionary").Value.Add(mapState, new HashSet<IRecipeMapObjectOptimizerTarget>());
        }

        private static void SetPotionBaseIcon(PotionBase potionBase, Texture2D texture)
        {
            if (spriteAtlas == null)
            {
                spriteAtlas = new CrucibleSpriteAtlas("CruciblePotionBases");
                IngredientsListResolveAtlasEvent.OnAtlasRequest += (_, e) =>
                {
                    if (AtlasOverriddenPotionBases.Contains(e.Object))
                    {
                        e.AtlasResult = spriteAtlas.AtlasName;
                    }
                };

                CrucibleSpriteAtlasManager.AddAtlas(spriteAtlas);
            }

            spriteAtlas.SetIcon($"PotionBase {potionBase.name} SmallIcon", texture, yOffset: texture.height * 0.66f);

            AtlasOverriddenPotionBases.Add(potionBase);
        }

        private static void ClearInitialFog(MapState mapState)
        {
            var oldCurrentMap = Managers.RecipeMap.currentMap;
            Managers.RecipeMap.currentMap = mapState;
            try
            {
                // ClearFogAroundIndicator can't be used, as the game save might have a potion in progress
                Managers.RecipeMap.fogOfWar.FogShow(
                    Vector2.zero,
                    Managers.RecipeMap.fogOfWar.settings.visibilityRadiusAroundIndicator + Managers.RecipeMap.fogOfWar.exploringRadiusAddendum,
                    false);
            }
            finally
            {
                Managers.RecipeMap.currentMap = oldCurrentMap;
            }
        }
    }
}

#endif
