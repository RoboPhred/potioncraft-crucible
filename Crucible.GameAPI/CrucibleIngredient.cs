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

        // public void SetIconsFromTexture(Texture2D ingredientImage)
        // {
        //     // TODO: Automatically set up icons with appropriate size and padding.
        //     throw new NotImplementedException("Auto generate icons");
        // }

        /// <summary>
        /// Creates a new ingredient with the given id.
        /// </summary>
        /// <remarks>
        /// The ingredient will be created with placeholder data.  It is strongly recommended that
        /// you set the other properties of the ingredient to customize it after its creation.
        /// </remarks>
        /// <param name="id">The id to create the ingredient with.</param>
        /// <returns>An object to configure the new ingredient.</returns>
        public static CrucibleIngredient Create(string id)
        {
            if (Get(id) != null)
            {
                throw new ArgumentException("An ingredient with the given id already exists.");
            }

            var ingredient = ScriptableObject.CreateInstance<Ingredient>();
            ingredient.name = id;

            var crucibleIngredient = new CrucibleIngredient(ingredient)
            {
                InventoryIcon = SpriteUtilities.Placeholder,
                RecipeStepIcon = SpriteUtilities.Placeholder,
                IngredientListIcon = SpriteUtilities.Placeholder,
                Price = 1f,
                CanBeDamaged = true,
                IsTeleportationIngredient = false,
            };
            crucibleIngredient.SetPath(new[] { CrucibleIngredientPathSegment.LineTo(1f, 0) });

            var ingredientBase = Managers.Ingredient.ingredients.Find(x => x.name == "Waterbloom");

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

        public static CrucibleIngredient GetOrCreate(string id)
        {
            return Get(id) ?? Create(id);
        }

        /// <summary>
        /// Gets an existing ingredient by its id.
        /// </summary>
        /// <param name="id">The internal name of the ingredient to get.</param>
        /// <returns>The ingredient if found, or null if no ingredient exists by the given id.</returns>
        public static CrucibleIngredient Get(string id)
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
        public static IEnumerable<CrucibleIngredient> GetAll()
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
