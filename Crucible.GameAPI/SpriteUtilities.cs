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
    }
}
