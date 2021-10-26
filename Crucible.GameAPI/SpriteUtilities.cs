namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using UnityEngine;

    /// <summary>
    /// Utilities for working with Unity Sprites.
    /// </summary>
    public static class SpriteUtilities
    {
        private static Sprite placeholder;

        /// <summary>
        /// Gets a sprite to use as a placeholder.
        /// </summary>
        public static Sprite Placeholder => placeholder ??= Sprite.Create(TextureUtilities.Placeholder, new Rect(0, 0, TextureUtilities.Placeholder.width, TextureUtilities.Placeholder.height), new Vector2(0.5f, 0.5f));

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
    }
}