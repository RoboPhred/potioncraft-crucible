namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Utilities for working with Unity textures.
    /// </summary>
    public static class TextureUtilities
    {
        /// <summary>
        /// Loads a texture from the given file path.
        /// </summary>
        /// <param name="filePath">The path to the image to use as a texture.</param>
        /// <returns>A texture created from the given image file path.</returns>
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            var data = File.ReadAllBytes(filePath);

            // Do not create mip levels for this texture, use it as-is.
            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            if (!tex.LoadImage(data))
            {
                throw new Exception($"Failed to load image from file at \"{filePath}\".");
            }

            return tex;
        }

        /// <summary>
        /// Creates an empty texure filled with the given color.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="fill">The fill color of the texture.</param>
        /// <returns>The created texture.</returns>
        public static Texture2D CreateBlankTexture(int width, int height, Color fill)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, fill);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}
