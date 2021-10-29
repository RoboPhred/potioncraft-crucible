namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// Manages custom sprite atlases added to PotionCraft.
    /// </summary>
    public static class CrucibleSpriteAtlasManager
    {
        private static readonly Dictionary<int, CrucibleSpriteAtlas> AtlasesByHashCode = new();
        private static bool isInitialized = false;

        /// <summary>
        /// Adds the sprite atlas to PotionCraft.
        /// </summary>
        /// <param name="atlas">The sprite atlas to add.</param>
        public static void AddAtlas(CrucibleSpriteAtlas atlas)
        {
            EnsureInitialized();
            AtlasesByHashCode[atlas.AtlasHashCode] = atlas;
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            TMPTextOnSpriteAssetRequestEvent.OnSpriteAssetRequest += (_, e) =>
            {
                if (AtlasesByHashCode.TryGetValue(e.AssetHashCode, out var atlas))
                {
                    e.SpriteAsset = atlas.Asset;
                }
            };
        }
    }
}
