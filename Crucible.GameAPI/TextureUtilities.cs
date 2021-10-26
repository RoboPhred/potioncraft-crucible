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
        private static Texture2D placeholder;

        /// <summary>
        /// Gets a placeholder texture.
        /// </summary>
        public static Texture2D Placeholder => placeholder ??= new Texture2D(1, 1);

        /// <summary>
        /// Loads a texture from the given file path.
        /// </summary>
        /// <param name="filePath">The path to the image to use as a texture.</param>
        /// <returns>A texture created from the given image file path.</returns>
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            var data = File.ReadAllBytes(filePath);
            var tex = new Texture2D(0, 0);
            if (!tex.LoadImage(data))
            {
                throw new Exception($"Failed to load image from file at \"{filePath}\"");
            }

            return tex;
        }
    }
}
