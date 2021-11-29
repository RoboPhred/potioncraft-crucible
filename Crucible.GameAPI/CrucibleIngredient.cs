// <copyright file="CrucibleIngredient.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using LocalizationSystem;
    using ObjectBased.Stack;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;
    using SoundSystem.SoundControllers;
    using UnityEngine;
    using Utils.BezierCurves;
    using Utils.SortingOrderSetter;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="Ingredient"/>s.
    /// </summary>
    public sealed class CrucibleIngredient : CrucibleInventoryItem
    {
        private static readonly HashSet<Ingredient> AtlasOverriddenIngredients = new();
        private static readonly HashSet<Ingredient> StackOverriddenIngredients = new();

        private static bool overridesInitialized = false;

        private static CrucibleSpriteAtlas spriteAtlas;

        private CrucibleIngredient(Ingredient ingredient)
        : base(ingredient)
        {
        }

        /// <summary>
        /// Gets the raw object used by PotionCraft for this ingredient.
        /// </summary>
        public Ingredient Ingredient
        {
            get
            {
                return (Ingredient)this.InventoryItem;
            }
        }

        /// <summary>
        /// Gets or sets the name of this ingredient in the user's current language.
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
        /// Gets or sets the description of this item in the user's current language.
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
        /// Gets or sets the sprite to use for this ingredient in the recipe steps list.
        /// </summary>
        public Sprite RecipeStepIcon
        {
            get
            {
                return this.Ingredient.recipeMarkIcon;
            }

            set
            {
                this.Ingredient.recipeMarkIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the image to use when displaying this ingredient in the ingredients list.
        /// </summary>
        /// <remarks>
        /// This sprite will appear in:
        /// - The list of ingredients while brewing a potion.
        /// - The list of ingredients on the left page of a saved recipe.
        /// - The ingredients list when hovering over a potion.
        /// </remarks>
        public Sprite IngredientListIcon
        {
            get
            {
                return this.Ingredient.smallIcon;
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

                this.Ingredient.smallIcon = value;

                SetIngredientIcon(this.Ingredient, value.texture);
            }
        }

        /// <summary>
        /// Gets or sets the color of this ingredient when ground in the mortar.
        /// </summary>
        public Color GroundColor
        {
            get
            {
                return this.Ingredient.grindedSubstanceColor;
            }

            set
            {
                this.Ingredient.grindedSubstanceColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this ingredient can be damaged
        /// by smashing it against hard surfaces.
        /// </summary>
        public bool CanBeDamaged
        {
            get
            {
                return this.Ingredient.canBeDamaged;
            }

            set
            {
                this.Ingredient.canBeDamaged = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this ingredient causes the potion
        /// to teleport, as seen with most crystal ingredients.
        /// </summary>
        public bool IsTeleportationIngredient
        {
            get
            {
                return this.Ingredient.isTeleportationIngredient;
            }

            set
            {
                this.Ingredient.isTeleportationIngredient = value;
            }
        }

        /// <summary>
        /// Gets the length of this ingredient's path.
        /// </summary>
        public float PathLength
        {
            get
            {
                return this.Ingredient.path.Length;
            }
        }

        /// <summary>
        /// Gets or sets the percent of the path that is available by default without grinding.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value controls the amount of the path that is accessible if the ingredient is dropped into the cauldron without any grinding.
        /// Grinding this ingredient will unlock more of the path.
        /// </para>
        /// The value is a percentage.  A value of 0.5 will make 50% of the path be pre-ground, with the remaining 50% of the path unlocked by grinding.
        /// </remarks>
        public float PathPregrindPercentage
        {
            get
            {
                return this.Ingredient.path.grindedPathStartsFrom;
            }

            set
            {
                if (value < 0 || value >= 1)
                {
                    throw new ArgumentOutOfRangeException("value", "Grind percentage must be at least 0 and less than 1.");
                }

                this.Ingredient.path.grindedPathStartsFrom = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the stack items spawned act solid in the mortar.
        /// </summary>
        public bool IsStackItemSolid
        {
            get
            {
                return this.Ingredient.isSolid;
            }

            set
            {
                this.Ingredient.isSolid = value;
            }
        }

        private string LocalizationKey
        {
            get
            {
                return $"ingredient_{this.InventoryItem.name}";
            }
        }

        /// <summary>
        /// Creates a new ingredient with the given id.
        /// </summary>
        /// <remarks>
        /// The ingredient will be created by cloning the data from the ingredient specified by <paramref cref="copyFromId"/>.
        /// You can then use the properties and functions of the resultant <see cref="CrucibleIngredient"/> to customize
        /// its appearance and behavior.
        /// </remarks>
        /// <param name="id">The id to create the ingredient with.</param>
        /// <param name="copyFromId">The id of the ingredient to copy from.</param>
        /// <returns>An object to configure the new ingredient.</returns>
        public static CrucibleIngredient CreateIngredient(string id, string copyFromId = "Waterbloom")
        {
            // TODO: names are namespaced among all inventory items.  Should check other types to make sure the id does not collide.
            if (GetIngredientById(id) != null)
            {
                throw new ArgumentException($"An ingredient with the given id of \"{id}\" already exists.");
            }

            var ingredientBase = Managers.Ingredient.ingredients.Find(x => x.name == copyFromId);
            if (ingredientBase == null)
            {
                throw new ArgumentException($"Cannot find ingredient \"{copyFromId}\" to copy settings from.");
            }

            var ingredient = ScriptableObject.CreateInstance<Ingredient>();
            ingredient.name = id;

            var crucibleIngredient = new CrucibleIngredient(ingredient)
            {
                InventoryIcon = ingredientBase.inventoryIconObject,
                RecipeStepIcon = ingredientBase.recipeMarkIcon,

                // We cannot copy the existing icon because it is a non-readable texture.
                IngredientListIcon = SpriteUtilities.CreateBlankSprite(16, 16, Color.red),

                Price = ingredientBase.GetPrice(),
                CanBeDamaged = ingredientBase.canBeDamaged,
                IsTeleportationIngredient = ingredientBase.isTeleportationIngredient,
            };

            ingredient.path = new IngredientPath
            {
                path = ingredientBase.path.path.ToList(),
                grindedPathStartsFrom = ingredientBase.path.grindedPathStartsFrom,
            };

            ingredient.itemStackPrefab = ingredientBase.itemStackPrefab;

            ingredient.grindedSubstanceColor = ingredientBase.grindedSubstanceColor;

            // TODO: Get access to these from the crucible api.
            ingredient.grindedSubstance = ingredientBase.grindedSubstance;
            ingredient.physicalParticleType = ingredientBase.physicalParticleType;
            ingredient.effectMovement = ingredientBase.effectMovement;
            ingredient.effectCollision = ingredientBase.effectCollision;
            ingredient.effectPlantGathering = ingredientBase.effectPlantGathering;
            ingredient.viscosityDown = ingredientBase.viscosityDown;
            ingredient.viscosityUp = ingredientBase.viscosityUp;
            ingredient.isSolid = ingredientBase.isSolid;
            ingredient.spotPlantPrefab = ingredientBase.spotPlantPrefab;
            ingredient.spotPlantSpawnTypes = new List<GrowingSpotType>();
            ingredient.soundPreset = ingredientBase.soundPreset;

            // These are calculated on awake, or with our ReinitializeIngredient function.
            ingredient.grindStatusByLeafGrindingCurve = ingredientBase.grindStatusByLeafGrindingCurve;
            ingredient.substanceGrindingSettings = ingredientBase.substanceGrindingSettings;
            ingredient.grindedSubstanceMaxAmount = ingredientBase.grindedSubstanceMaxAmount;

            ingredient.OnAwake();

            Managers.Ingredient.ingredients.Add(ingredient);

            return crucibleIngredient;
        }

        /// <summary>
        /// Gets an existing ingredient by its id.
        /// </summary>
        /// <param name="id">The internal name of the ingredient to get.</param>
        /// <returns>The ingredient if found, or null if no ingredient exists by the given id.</returns>
        public static CrucibleIngredient GetIngredientById(string id)
        {
            var ingredient = Managers.Ingredient.ingredients.Find(x => x.name == id);
            if (ingredient == null)
            {
                return null;
            }

            return new CrucibleIngredient(ingredient);
        }

        /// <summary>
        /// Gets all ingredients present in the game.
        /// </summary>
        /// <returns>An enumerable of all ingredients.</returns>
        public static IEnumerable<CrucibleIngredient> GetAllIngredients()
        {
            return Managers.Ingredient.ingredients.Select(x => new CrucibleIngredient(x));
        }

        /// <summary>
        /// Sets the localized name of this ingredient.
        /// </summary>
        /// <param name="name">The localized name to use for this ingredient.</param>
        public void SetLocalizedName(LocalizedString name)
        {
            CrucibleLocalization.SetLocalizationKey(this.LocalizationKey, name);
        }

        /// <summary>
        /// Sets the localized description of this ingredient.
        /// </summary>
        /// <param name="description">The localized description to use for this ingredient.</param>
        public void SetLocalizedDescription(LocalizedString description)
        {
            CrucibleLocalization.SetLocalizationKey($"{this.LocalizationKey}_description", description);
        }

        /// <summary>
        /// Sets the stack for this ingredient.
        /// </summary>
        /// <remarks>
        /// Stack items are the visual representation of the ingredient when it is being dragged or ground.
        /// </remarks>
        /// <param name="rootItem">The root item to produce the stack from.</param>
        public void SetStack(CrucibleIngredientStackItem rootItem)
        {
            this.SetStack(new[] { rootItem });
        }

        /// <summary>
        /// Sets the stack for this ingredient.
        /// </summary>
        /// <remarks>
        /// Stack items are the visual representation of the ingredient when it is being dragged or ground.
        /// </remarks>
        /// <param name="rootItems">An enumerable of root items to produce the stack from.</param>
        public void SetStack(IEnumerable<CrucibleIngredientStackItem> rootItems)
        {
            EnsureOverrides();

            var prefab = new GameObject
            {
                name = $"{this.ID} Stack",
            };
            prefab.SetActive(false);

            var stack = prefab.AddComponent<Stack>();
            stack.inventoryItem = this.InventoryItem;
            var stackTraverse = Traverse.Create(stack);
            stackTraverse.Property<ItemFromInventoryController>("SoundController").Value = new SoundController(stack, this.Ingredient.soundPreset);
            stackTraverse.Field<float>("assemblingSpeed").Value = 3;

            var visualEffects = prefab.AddComponent<StackVisualEffects>();
            visualEffects.stackScript = stack;

            prefab.AddComponent<Rigidbody2D>();

            // Not sure what settings this wants.
            // It seems to remove itself automatically, as this component does not exist when inspecting the game object later.
            prefab.AddComponent<SortingOrderSetter>();

            foreach (var rootItem in rootItems)
            {
                var stackItemGO = this.CreateStackItem(rootItem);
                stackItemGO.transform.parent = prefab.transform;
            }

            StackOverriddenIngredients.Add(this.Ingredient);

            this.Ingredient.itemStackPrefab = prefab;

            // Perform OnAwake initialization logic
            this.ReinitializeIngredient();
        }

        /// <summary>
        /// Gets an enumerable enumerating the potion path for this ingredient.
        /// </summary>
        /// <returns>An enumerable of the potion path for this ingredient.</returns>
        public IEnumerable<CrucibleIngredientPathSegment> GetPath()
        {
            foreach (var part in this.Ingredient.path.path)
            {
                yield return CrucibleIngredientPathSegment.FromPotioncraftCurve(part);
            }
        }

        /// <summary>
        /// Sets the ingredient's potion path to the given path.
        /// </summary>
        /// <param name="path">The path to set the ingredient's path to.</param>
        public void SetPath(IEnumerable<CrucibleIngredientPathSegment> path)
        {
            var list = new List<CubicBezierCurve>();

            var start = Vector2.zero;
            foreach (var part in path)
            {
                var curve = part.ToPotioncraftCurve(start);
                list.Add(curve);
                start = curve.PLast;
            }

            this.Ingredient.path.path = list;
        }

        private static void SetIngredientIcon(Ingredient ingredient, Texture2D texture)
        {
            EnsureOverrides();

            if (spriteAtlas == null)
            {
                spriteAtlas = new CrucibleSpriteAtlas("CrucibleIngredients");
                CrucibleSpriteAtlasManager.AddAtlas(spriteAtlas);
            }

            spriteAtlas.SetIcon($"{ingredient.name.Replace(" ", string.Empty)} SmallIcon", texture, yOffset: texture.height - 15);

            AtlasOverriddenIngredients.Add(ingredient);
        }

        private static void EnsureOverrides()
        {
            if (overridesInitialized)
            {
                return;
            }

            overridesInitialized = true;

            IngredientsListResolveAtlasEvent.OnAtlasRequest += (_, e) =>
            {
                // Check the type so we can call the efficient HashSet.Contains(Ingredient), rather than the slow
                // Linq.Contains(object)
                if (e.Object is not Ingredient ingredient)
                {
                    return;
                }

                if (AtlasOverriddenIngredients.Contains(ingredient))
                {
                    e.AtlasResult = spriteAtlas.AtlasName;
                }
            };

            // Stack items expect activated perfabs.  Prefabs made by the unity editor can be 'active' while not actually existing as true game objects.
            // Since we cannot pull this off at runtime, we instead need to set our template game objects as inactive.  This stops them running
            // their initialization code before they are actually spawned and hooked up to real items.
            // However, we need to activate them on spawn to let their logic wake up and work properly.
            StackSpawnNewItemEvent.OnSpawnNewItemPreInititialize += (object _, StackSpawnNewItemEventArgs e) =>
            {
                if (e.Stack.inventoryItem is not Ingredient ingredient)
                {
                    return;
                }

                if (StackOverriddenIngredients.Contains(ingredient))
                {
                    e.Stack.gameObject.SetActive(true);
                }
            };
        }

        private void ReinitializeIngredient()
        {
            // Perform the work of OnAwait to reinitialize this ingredient's data.
            var ingredientTraverse = Traverse.Create(this.Ingredient);
            ingredientTraverse.Method("CalculateStatesAndPrefabs").GetValue();
            ingredientTraverse.Property<int>("MaxItems").Value = this.Ingredient.itemStackPrefab.transform.childCount;
            ingredientTraverse.Method("CalculateTotalCurveValue").GetValue();
            Traverse.Create(this.Ingredient.substanceGrindingSettings).Method("CalculateTotalCurveValue").GetValue();
        }

        private GameObject CreateStackItem(CrucibleIngredientStackItem crucibleStackItem, int depth = 0)
        {
            var stackItem = new GameObject
            {
                name = $"{this.ID} Stack Item {depth}",
            };
            stackItem.transform.parent = GameObjectUtilities.DisabledRoot.transform;
            stackItem.transform.localPosition = crucibleStackItem.PositionInStack;
            stackItem.transform.localRotation = Quaternion.Euler(0, 0, crucibleStackItem.AngleInStack);

            var goOuter = new GameObject
            {
                name = "Collider Outer",
                layer = LayerMask.NameToLayer("IngredientsOuter"),
            };
            goOuter.transform.parent = stackItem.transform;
            goOuter.transform.localPosition = Vector3.zero;
            goOuter.transform.localRotation = Quaternion.identity;
            var colliderOuter = goOuter.AddComponent<PolygonCollider2D>();

            var goInner = new GameObject
            {
                name = "Collider Inner",
                layer = LayerMask.NameToLayer("IngredientsInner"),
            };
            goInner.transform.parent = stackItem.transform;
            goInner.transform.localPosition = Vector3.zero;
            goOuter.transform.localRotation = Quaternion.identity;
            var colliderInner = goInner.AddComponent<PolygonCollider2D>();

            var positionCorrectedCollider = crucibleStackItem.ColliderPolygon;
            var positionCorrectedInnerCollider = crucibleStackItem.InnerColliderPolygon ?? crucibleStackItem.ColliderPolygon;

            if (positionCorrectedCollider?.Count > 0)
            {
                colliderOuter.pathCount = 1;

                colliderOuter.SetPath(0, positionCorrectedCollider);
            }
            else
            {
                var dummyCollider = new List<Vector2>
                {
                    new Vector2(-.3f, -.3f),
                    new Vector2(.3f, -.3f),
                    new Vector2(.3f, .3f),
                    new Vector2(-.3f, .3f),
                };
                colliderOuter.SetPath(0, dummyCollider);
            }

            if (positionCorrectedInnerCollider?.Count > 0)
            {
                colliderInner.pathCount = 1;

                colliderInner.SetPath(0, positionCorrectedInnerCollider);
            }
            else
            {
                var dummyCollider = new List<Vector2>
                {
                    new Vector2(-.3f, -.3f),
                    new Vector2(.3f, -.3f),
                    new Vector2(.3f, .3f),
                    new Vector2(-.3f, .3f),
                };
                colliderInner.SetPath(0, dummyCollider);
            }

            var spriteRenderer = stackItem.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = crucibleStackItem.Sprite;

            var ifs = stackItem.AddComponent<IngredientFromStack>();
            ifs.spriteRenderers = new[] { spriteRenderer };
            ifs.colliderOuter = colliderOuter;
            ifs.colliderInner = colliderInner;
            ifs.NextStagePrefabs = crucibleStackItem.GrindChildren.Select(x => this.CreateStackItem(x, depth + 1)).ToArray();

            return stackItem;
        }
    }
}
