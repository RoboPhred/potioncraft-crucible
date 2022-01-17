// <copyright file="CruciblePotionBottle.cs" company="RoboPhredDev">
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
    using ElementChangerWindow;
    using HarmonyLib;
    using PotionCustomizationWindow;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="PotionBottle"/>s.
    /// </summary>
    public sealed class CruciblePotionBottle
    {
        private CruciblePotionBottle(PotionBottle potionBottle)
        {
            this.PotionBottle = potionBottle;
        }

        /// <summary>
        /// Gets the base game <see cref="PotionBottle"/> wrapped by this api object.
        /// </summary>
        public PotionBottle PotionBottle { get; }

        /// <summary>
        /// Gets or sets the icon to use for this bottle in the bottle menu.
        /// </summary>
        public Sprite BottleIcon
        {
            get
            {
                return this.PotionBottle.backgroundSkinElementSprite;
            }

            set
            {
                this.PotionBottle.backgroundSkinElementSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the foreground sprite for the bottle.
        /// </summary>
        public Sprite BottleForeground
        {
            get
            {
                return this.PotionBottle.PrefabPotionItem.visualObject.bottleFgRenderer.sprite;
            }

            set
            {
                this.PotionBottle.PrefabPotionItem.visualObject.bottleFgRenderer.sprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the mask for this potion bottle.
        /// </summary>
        public Sprite BottleMask
        {
            get
            {
                return this.PotionBottle.PrefabPotionItem.visualObject.bottleSpriteMask.sprite;
            }

            set
            {
                this.PotionBottle.PrefabPotionItem.visualObject.bottleSpriteMask.sprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset of the label.
        /// </summary>
        public Vector2 LabelOffset
        {
            get
            {
                return this.PotionBottle.PrefabPotionItem.visualObject.stickerContainer.position;
            }

            set
            {
                this.PotionBottle.PrefabPotionItem.visualObject.stickerContainer.position = value;
            }
        }

        /// <summary>
        /// Gets or sets the primary liquid sprite.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If the bottle only contains one effect, this sprite is used for that effect.
        /// </p>
        /// <p>
        /// If the bottle contains two effects, this sprite is used for the first effect.
        /// </p>
        /// <p>
        /// If the bottle contains three effects, this sprite is used for the second effect.
        /// </p>
        /// <p>
        /// If the bottle contains four effects, this sprite is used for the third effect.
        /// </p>
        /// <p>
        /// If the bottle contains five effects, this sprite is used for the third effect.
        /// </p>
        /// </remarks>
        public Sprite LiquidMain
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColorMain;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColorMain = value;
            }
        }

        /// <summary>
        /// Gets or sets the second liquid sprite to use for two-effect potions.
        /// </summary>
        public Sprite Liquid2Of2
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor2Of2;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor2Of2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for three-effect potions.
        /// </summary>
        public Sprite Liquid1Of3
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of3;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the third liquid sprite to use for three-effect potions.
        /// </summary>
        public Sprite Liquid3Of3
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor3Of3;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor3Of3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid1Of4
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of4;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of4 = value;
            }
        }

        /// <summary>
        /// Gets or sets the third liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid3Of4
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor3Of4;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor3Of4 = value;
            }
        }

        /// <summary>
        /// Gets or sets the fourth liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid4Of4
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor4Of4;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor4Of4 = value;
            }
        }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid1Of5
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of5;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor1Of5 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid2Of5
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor2Of5;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor2Of5 = value;
            }
        }

        /// <summary>
        /// Gets or sets the fourth liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid4Of5
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor4Of5;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor4Of5 = value;
            }
        }

        /// <summary>
        /// Gets or sets the fifth liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid5Of5
        {
            get
            {
                return this.PotionBottle.bunchOfLiquidSprites.backgroundColor5Of5;
            }

            set
            {
                this.PotionBottle.bunchOfLiquidSprites.backgroundColor5Of5 = value;
            }
        }

        /// <summary>
        /// Creates a new potion bottle with the given id.
        /// </summary>
        /// <param name="id">The id of the potion bottle to create.</param>
        /// <returns>The api object for the created potion bottle.</returns>
        public static CruciblePotionBottle CreatePotionBottle(string id)
        {
            if (PotionBottle.allPotionBottles.Any(x => x.name == id))
            {
                throw new ArgumentException($"A potion bottle with the id {id} already exists.", nameof(id));
            }

            var bottle = ScriptableObject.CreateInstance<PotionBottle>();
            var blankSprite = SpriteUtilities.CreateBlankSprite(200, 200, new Color(0, 0, 0, 0));
            bottle.bunchOfLiquidSprites = new PotionBottle.BunchOfLiquidSprites
            {
                backgroundColorMain = blankSprite,

                backgroundColor2Of2 = blankSprite,

                backgroundColor1Of3 = blankSprite,
                backgroundColor3Of3 = blankSprite,

                backgroundColor1Of4 = blankSprite,
                backgroundColor3Of4 = blankSprite,
                backgroundColor4Of4 = blankSprite,

                backgroundColor1Of5 = blankSprite,
                backgroundColor2Of5 = blankSprite,
                backgroundColor4Of5 = blankSprite,
                backgroundColor5Of5 = blankSprite,
            };

            bottle.potionItemPrefab = GameObject.Instantiate(PotionBottle.allPotionBottles[0].potionItemPrefab, Vector3.zero, Quaternion.identity, GameObjectUtilities.CruciblePrefabRoot.transform);
            bottle.PrefabPotionItem.visualObject.corkRenderer.sprite = blankSprite;
            bottle.PrefabPotionItem.visualObject.bottleSpriteMask.sprite = blankSprite;
            bottle.PrefabPotionItem.visualObject.bottleShadowsRenderer.sprite = blankSprite;
            bottle.PrefabPotionItem.visualObject.bottleFgRenderer.sprite = blankSprite;
            bottle.PrefabPotionItem.visualObject.bottleScratchesRenderer.sprite = blankSprite;

            bottle.backgroundSkinElementSprite = SpriteUtilities.CreateBlankSprite(50, 50, Color.red);

            PotionBottle.allPotionBottles.Add(bottle);

            // Add the bottle to the bottle chooser UI.
            var skinChangerWindow = GameObject.FindObjectOfType<PotionSkinChangerWindow>();
            var panelGroup = Traverse.Create(skinChangerWindow).Field<ElementChangerPanelGroup>("bottleSkinChangerPanelGroup").Value;
            var mainPanel = panelGroup.mainPanel as ElementChangerPanelWithElements;
            if (mainPanel != null)
            {
                mainPanel.elements.Add(bottle);
            }

            return new CruciblePotionBottle(bottle);
        }

        /// <summary>
        /// Gets a potion bottle with the given id.
        /// </summary>
        /// <param name="id">The id of the bottle to get.</param>
        /// <returns>The potion bottle with the given id, or <c>null</c> if no matching potion bottle was found.</returns>
        public static CruciblePotionBottle GetPotionBottleById(string id)
        {
            var bottle = PotionBottle.allPotionBottles.Find(x => x.name == id);
            if (bottle == null)
            {
                return null;
            }

            return new CruciblePotionBottle(bottle);
        }

        /// <summary>
        /// Gets the enumerable of points that define the bottle's collision.
        /// </summary>
        /// <returns>An enumerable of points that make up the bottle's collision.</returns>
        public IEnumerable<Vector2> GetColliderPolygon()
        {
            var collider = this.PotionBottle.potionItemPrefab.GetComponent<PolygonCollider2D>();
            if (collider == null)
            {
                return Enumerable.Empty<Vector2>();
            }

            return collider.points;
        }

        /// <summary>
        /// Sets the points that define the bottle's collision.
        /// </summary>
        /// <param name="points">An enumerable of points to define the collision of the bottle.</param>
        public void SetColliderPolygon(IEnumerable<Vector2> points)
        {
            var prefab = this.PotionBottle.potionItemPrefab;
            var collider = prefab.GetComponent<PolygonCollider2D>() ?? prefab.AddComponent<PolygonCollider2D>();

            collider.points = points.ToArray();
        }
    }
}
