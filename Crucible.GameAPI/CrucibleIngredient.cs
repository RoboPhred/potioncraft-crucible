namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Utils.BezierCurves;

    /// <summary>
    /// Wraps an <see cref="Ingredient"/> to provide an api for mod use.
    /// </summary>
    public sealed class CrucibleIngredient : CrucibleInventoryItem
    {
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
        /// Gets or sets the sprite to use when displaying this ingredient in the ingredients list.
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
                this.Ingredient.smallIcon = value;

                // TODO: Set the atlas injection for this ingredient.
                throw new NotImplementedException("Atlas injection");
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

        // public void SetIconsFromTexture(Texture2D ingredientImage)
        // {
        //     // TODO: Automatically set up icons with appropriate size and padding.
        //     throw new NotImplementedException("Auto generate icons");
        // }

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

                // FIXME: We want to copy from copyFromId, but base ingredients do not have a readable smallIcon texture.
                // Setting this to ingredientBase.smallIcon will throw an error when building the atlas.
                IngredientListIcon = SpriteUtilities.Placeholder,

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
    }
}
