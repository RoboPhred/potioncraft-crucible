// <copyright file="SpriteUtilities.cs" company="RoboPhredDev">
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
    using UnityEngine;

    /// <summary>
    /// Utilities for working with Unity Sprites.
    /// </summary>
    public static class SpriteUtilities
    {
        /// <summary>
        /// Loads a sprite from an image at the given file path.
        /// The sprite will have the same width and height as the image, with an origin point centered on the image.
        /// </summary>
        /// <param name="filePath">File path to the image to use for the sprite.</param>
        /// <returns>A sprite created from the given image.</returns>
        public static Sprite LoadSpriteFromFile(string filePath)
        {
            var tex = TextureUtilities.LoadTextureFromFile(filePath);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Creates a blank sprite filled with the given color.
        /// </summary>
        /// <param name="width">The sprite width.</param>
        /// <param name="height">The sprite height.</param>
        /// <param name="fill">The fill color.</param>
        /// <returns>A sprite of the given dimentions filled with the specified color.</returns>
        public static Sprite CreateBlankSprite(int width, int height, Color fill)
        {
            var tex = TextureUtilities.CreateBlankTexture(width, height, fill);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Creates a sprite from a texture with an optional pivot.
        /// </summary>
        /// <param name="texture">The texture to create the sprite from.</param>
        /// <param name="pivot">The pivot to use.  If unset, the pivot will be centered.</param>
        /// <returns>The sprite.</returns>
        public static Sprite FromTexture(Texture2D texture, Vector2? pivot = null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot ?? new Vector2(0.5f, 0.5f));
        }
    }
}
