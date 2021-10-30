namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using TMPro;

    /// <summary>
    /// Arguments for events that request a <see cref="TMP_SpriteAsset"/> by name.
    /// </summary>
    public class SpriteAssetRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAssetRequestEventArgs"/> class.
        /// </summary>
        /// <param name="hashCode">The hash code of the asset name.</param>
        /// <param name="assetName">The asset name</param>
        public SpriteAssetRequestEventArgs(int hashCode, string assetName)
        {
            this.AssetHashCode = hashCode;
            this.AssetName = assetName;
        }

        /// <summary>
        /// Gets the hash code of the asset name.
        /// </summary>
        public int AssetHashCode { get; }

        /// <summary>
        /// Gets the asset name being requested.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        /// Gets or sets the sprite asset used to respond to this request.
        /// </summary>
        public TMP_SpriteAsset SpriteAsset { get; set; }
    }
}
