// <copyright file="CruciblePotionSticker.cs" company="RoboPhredDev">
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
    using global::PotionCraft.ObjectBased.UIElements.ElementChangerWindow;
    using global::PotionCraft.ObjectBased.UIElements.ElementChangerWindow.AlchemySubstanceCustomizationWindow;
    using global::PotionCraft.ScriptableObjects;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// /// Provides an API for the creation and modification of potion bottle stickers.
    /// </summary>
    public sealed class CruciblePotionSticker
    {
        private readonly Sticker sticker;

        private CruciblePotionSticker(Sticker sticker) => this.sticker = sticker;

        /// <summary>
        /// Gets or sets the foreground sprite for the sticker.
        /// </summary>
        public Sprite Foreground
        {
            get
            {
                return this.sticker.foregroundSprite;
            }

            set
            {
                this.sticker.foregroundSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the background texture for the sticker.
        /// </summary>Sprite
        public Sprite Background
        {
            get
            {
                return this.sticker.backgroundSprite;
            }

            set
            {
                this.sticker.backgroundSprite = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon for this sticker in the sticker choice menu.
        /// </summary>
        public Sprite MenuIcon
        {
            get
            {
                return this.sticker.backgroundSkinElementSprite;
            }

            set
            {
                this.sticker.backgroundSkinElementSprite = value;
            }
        }

        /// <summary>
        /// Creates a new potion bottle sticker.
        /// </summary>
        /// <param name="id">The id of the new sticker.</param>
        /// <returns>A <see cref="CruciblePotionSticker"/> object for working with the new sticker.</returns>
        public static CruciblePotionSticker CreatePotionSticker(string id)
        {
            if (Sticker.allPotionStickers.Find(x => x.name == id))
            {
                throw new ArgumentException($"A potion sticker with the ID '{id}' already exists.", nameof(id));
            }

            var sticker = ScriptableObject.CreateInstance<Sticker>();
            sticker.name = id;

            var blankSprite = SpriteUtilities.CreateBlankSprite(1, 1, Color.clear);
            sticker.backgroundSprite = blankSprite;
            sticker.foregroundSprite = blankSprite;
            sticker.backgroundSkinElementSprite = blankSprite;

            Sticker.allPotionStickers.Add(sticker);
            AddStickerToUI(sticker);

            return new CruciblePotionSticker(sticker);
        }

        /// <summary>
        /// Gets a potion bottle sticker with the given id.
        /// </summary>
        /// <param name="id">The id of the sticker to fetch.</param>
        /// <returns>The sticker, or <c>null</c> if no sticker exists with the given id.</returns>
        public static CruciblePotionSticker GetPotionStickerFromID(string id)
        {
            var sticker = Sticker.allPotionStickers.Find(x => x.name == id);
            if (sticker == null)
            {
                return null;
            }

            return new CruciblePotionSticker(sticker);
        }

        private static void AddStickerToUI(Sticker sticker)
        {
            var skinChangerWindow = GameObject.FindFirstObjectByType<AlchemySubstanceSkinChangerWindow>();
            if (skinChangerWindow == null)
            {
                return;
            }

            var iconSkinChangerField = Traverse.Create(skinChangerWindow).Field<ElementChangerPanelGroup>("stickerSkinChangerPanelGroup");
            if (iconSkinChangerField == null)
            {
                return;
            }

            var panelGroup = iconSkinChangerField.Value;
            if (!(panelGroup.mainPanel is ElementChangerPanelWithElements mainPanel))
            {
                return;
            }

            var indexOfBlank = mainPanel.elements.FindIndex(x => x.name == "PotionSticker None");
            if (indexOfBlank != -1)
            {
                mainPanel.elements.Insert(indexOfBlank, sticker);
            }
            else
            {
                mainPanel.elements.Add(sticker);
            }
        }
    }
}
