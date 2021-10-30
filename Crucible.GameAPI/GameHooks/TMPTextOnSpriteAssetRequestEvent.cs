namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using HarmonyLib;
    using TMPro;

    /// <summary>
    /// Provides an event to allow multiple mods to share access to the <see cref="TMP_Text.OnSpriteAssetRequest"/> event.
    /// </summary>
    public static class TMPTextOnSpriteAssetRequestEvent
    {
        private static bool isInitialized = false;

        private static EventHandler<SpriteAssetRequestEventArgs> onSpriteAssetRequest;

        /// <summary>
        /// Raised when a sprite asset is requested.
        /// </summary>
        public static event EventHandler<SpriteAssetRequestEventArgs> OnSpriteAssetRequest
        {
            add
            {
                EnsureInitialized();
                onSpriteAssetRequest += value;
            }

            remove
            {
                onSpriteAssetRequest -= value;
            }
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            var oldSpriteAssetRequest = Traverse.Create<TMP_Text>().Field<Func<int, string, TMP_SpriteAsset>>("OnSpriteAssetRequest").Value;

            // Despite being an event, this overrides the previous handler.
            // This code will break if a mod loaded after us tries to add its own event handler.
            TMP_Text.OnSpriteAssetRequest += (hashCode, assetName) =>
            {
                var e = new SpriteAssetRequestEventArgs(hashCode, assetName);
                onSpriteAssetRequest?.Invoke(null, e);
                if (e.SpriteAsset != null)
                {
                    return e.SpriteAsset;
                }

                // Call the old handler, if any, to support other mods loaded before us.
                if (oldSpriteAssetRequest != null)
                {
                    return oldSpriteAssetRequest(hashCode, assetName);
                }

                return null;
            };
        }
    }
}
