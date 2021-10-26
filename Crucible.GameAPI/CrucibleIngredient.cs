namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using LocalizationSystem;
    using UnityEngine;

    /// <summary>
    /// API object providing access to and manipulations of PotionCraft ingredients.
    /// </summary>
    public sealed class CrucibleIngredient
    {
        private CrucibleIngredient(Ingredient ingredient)
        {
            this.RawObject = ingredient;
        }

        /// <summary>
        /// Gets the raw object used by PotionCraft for this ingredient.
        /// </summary>
        public Ingredient RawObject
        {
            get;
        }

        /// <summary>
        /// Gets the ID (internal name) of this ingredient.
        /// </summary>
        public string ID
        {
            get
            {
                return this.RawObject.name;
            }
        }

        /// <summary>
        /// Gets or sets the name of this ingredient in the user's current language.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return new Key(this.RawObject.name).GetText();
            }

            set
            {
                // TODO: Set the ingredient name in the localization tools.
                throw new NotImplementedException("Setting localized name");
            }
        }

        /// <summary>
        /// Gets or sets the sprite to use for this ingredient in the inventory.
        /// </summary>
        public Sprite InventoryIcon
        {
            get
            {
                return this.RawObject.inventoryIconObject;
            }

            set
            {
                this.RawObject.inventoryIconObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite to use for this ingredient in the recipe steps list.
        /// </summary>
        public Sprite RecipeStepIcon
        {
            get
            {
                return this.RawObject.recipeMarkIcon;
            }

            set
            {
                this.RawObject.recipeMarkIcon = value;
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
                return this.RawObject.smallIcon;
            }

            set
            {
                this.RawObject.smallIcon = value;

                // TODO: Set the atlas injection for this ingredient.
                throw new NotImplementedException("Atlas injection");
            }
        }

        /// <summary>
        /// Gets or sets the base price of this ingredient for buying or selling.
        /// </summary>
        /// <value>The price of the ingredient.</value>
        public float Price
        {
            get
            {
                return Traverse.Create(this.RawObject).Field<float>("price").Value;
            }

            set
            {
                Traverse.Create(this.RawObject).Field<float>("price").Value = value;
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
                return this.RawObject.canBeDamaged;
            }

            set
            {
                this.RawObject.canBeDamaged = value;
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
                return this.RawObject.isTeleportationIngredient;
            }

            set
            {
                this.RawObject.isTeleportationIngredient = value;
            }
        }

        // public void SetIconsFromTexture(Texture2D ingredientImage)
        // {
        //     // TODO: Automatically set up icons with appropriate size and padding.
        //     throw new NotImplementedException("Auto generate icons");
        // }

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
    }
}
