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
            if (GetIngredient(id) != null)
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
                IngredientListIcon = SpriteUtilities.CreateBlankSprite(32, 32, Color.red),

                Price = ingredientBase.GetPrice(),
                CanBeDamaged = ingredientBase.canBeDamaged,
                IsTeleportationIngredient = ingredientBase.isTeleportationIngredient,
            };

            ingredient.path = new IngredientPath
            {
                path = ingredientBase.path.path.ToList(),
                grindedPathStartsFrom = ingredientBase.path.grindedPathStartsFrom,
            };

            // TODO: Get access to these from the crucible api.
            ingredient.itemStackPrefab = ingredientBase.itemStackPrefab;
            ingredient.grindedSubstance = ingredientBase.grindedSubstance;
            ingredient.grindedSubstanceColor = ingredientBase.grindedSubstanceColor;
            ingredient.grindStatusByLeafGrindingCurve = ingredientBase.grindStatusByLeafGrindingCurve;
            ingredient.grindedSubstanceMaxAmount = ingredientBase.grindedSubstanceMaxAmount;
            ingredient.physicalParticleType = ingredientBase.physicalParticleType;
            ingredient.substanceGrindingSettings = ingredientBase.substanceGrindingSettings;
            ingredient.effectMovement = ingredientBase.effectMovement;
            ingredient.effectCollision = ingredientBase.effectCollision;
            ingredient.effectPlantGathering = ingredientBase.effectPlantGathering;
            ingredient.viscosityDown = ingredientBase.viscosityDown;
            ingredient.viscosityUp = ingredientBase.viscosityUp;
            ingredient.isSolid = ingredientBase.isSolid;
            ingredient.spotPlantPrefab = ingredientBase.spotPlantPrefab;
            ingredient.spotPlantSpawnTypes = new List<GrowingSpotType>();
            ingredient.soundPreset = ingredientBase.soundPreset;

            ingredient.OnAwake();

            Managers.Ingredient.ingredients.Add(ingredient);

            return crucibleIngredient;
        }

        /// <summary>
        /// Gets an existing ingredient by its id.
        /// </summary>
        /// <param name="id">The internal name of the ingredient to get.</param>
        /// <returns>The ingredient if found, or null if no ingredient exists by the given id.</returns>
        public static CrucibleIngredient GetIngredient(string id)
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

        [Obsolete("Do not use")]
        public void DebugTestStack(Sprite sprite)
        {
            // Cannot use this because the prefabs are created as incompletely initialized objects, and StackItem
            // tries to run custom code that crashes when destroyed.
            /*
            var prefab = GameObject.Instantiate(this.Ingredient.itemStackPrefab);
            for (var i = prefab.transform.GetChildCount() - 1; i >= 0; i++)
            {
                GameObject.DestroyImmediate(prefab.transform.GetChild(i).gameObject);
            }
            */

            var prefab = new GameObject
            {
                name = "Crucible Test stack",
                active = false,
            };

            var stack = prefab.AddComponent<Stack>();
            stack.inventoryItem = this.InventoryItem;
            var stackTraverse = Traverse.Create(stack);
            stackTraverse.Property<ItemFromInventoryController>("SoundController").Value = new SoundController(stack, this.Ingredient.soundPreset);
            stackTraverse.Field<float>("assemblingSpeed").Value = 3;

            var visualEffects = prefab.AddComponent<StackVisualEffects>();
            visualEffects.stackScript = stack;

            var rigidBody = prefab.AddComponent<Rigidbody2D>();

            // How do we set this up?  It does not appear in the component list when in-game, but it is called by
            // ItemFromInventory.ToForeground when the stack is spawned.
            var sortingOrderSetter = prefab.AddComponent<SortingOrderSetter>();

            var stackItem = new GameObject
            {
                name = "Crucible Test Stack Item 1",
            };
            stackItem.transform.parent = prefab.transform;
            stackItem.transform.localScale = Vector3.one;
            stackItem.transform.localPosition = Vector3.zero;
            stackItem.transform.localRotation = Quaternion.identity;

            var goOuter = new GameObject
            {
                name = "Collider Outer",
                layer = LayerMask.NameToLayer("IngredientsOuter"),
            };
            goOuter.transform.parent = stackItem.transform;
            var colliderOuter = goOuter.AddComponent<PolygonCollider2D>();
            colliderOuter.SetPath(0, new[] { new Vector2(0, 0), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0) });

            var goInner = new GameObject
            {
                name = "Collider Inner",
                layer = LayerMask.NameToLayer("IngredientsInner"),
            };
            goInner.transform.parent = stackItem.transform;
            var colliderInner = goInner.AddComponent<PolygonCollider2D>();
            colliderInner.SetPath(0, new[] { new Vector2(0, 0), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0) });

            // int shapeCount = sprite.GetPhysicsShapeCount();
            // colliderOuter.pathCount = shapeCount;
            // var points = new List<Vector2>(64);
            // for (int i = 0; i < shapeCount; i++)
            // {
            //     sprite.GetPhysicsShape(i, points);
            //     colliderOuter.SetPath(i, points);
            // }

            var ifs = stackItem.AddComponent<IngredientFromStack>();

            // TODO: What is the purpose of each of these?
            ifs.colliderOuter = colliderOuter;
            ifs.colliderInner = colliderInner;

            var spriteRenderer = stackItem.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            // FIXME:
            // Making this inactive causes it to clone as inactive, breaking things.
            // Making this active without Initialize called makes it crash every frame on Update and OnGUI
            // Making this active and calling Initialize makes it work, but then on instantiate it calls Initialize
            //  again and creates two GraphicStateMachine components.
            // prefab.active = true;
            // ifs.Initialize(stack);

            // FIXME: Hacky and sloppy way of doing this.  Causes memory leak
            // Only do this once, and do it from a single static event handler that handles all ingredients.
            StackSpawnNewItemEvent.OnSpawnNewItemPreInititialize += (object _, StackSpawnNewItemEventArgs e) =>
            {
                if (e.Stack.inventoryItem == this.InventoryItem)
                {
                    Debug.Log("Initializing custom stack object");
                    e.GameObject.SetActive(true);
                }
            };

            this.Ingredient.itemStackPrefab = prefab;
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
            if (spriteAtlas == null)
            {
                spriteAtlas = new CrucibleSpriteAtlas("CrucibleIngredients");
                IngredientsListResolveAtlasEvent.OnAtlasRequest += (_, e) =>
                {
                    if (AtlasOverriddenIngredients.Contains(e.Object))
                    {
                        e.AtlasResult = spriteAtlas.AtlasName;
                    }
                };

                CrucibleSpriteAtlasManager.AddAtlas(spriteAtlas);
            }

            spriteAtlas.SetIcon($"{ingredient.name} SmallIcon", texture, 0, texture.height * 0.66f, 1.5f);

            AtlasOverriddenIngredients.Add(ingredient);
        }
    }
}
