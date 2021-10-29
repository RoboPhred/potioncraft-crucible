using System;

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    /// <summary>
    /// Event arguments for when localization data is resolved from a key.
    /// </summary>
    public class KeyGetTextEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the key to resolve.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the resolved text.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGetTextEventArgs"/> class.
        /// </summary>
        /// <param name="key">The key being resolve.</param>
        public KeyGetTextEventArgs(string key)
        {
            this.Key = key;
        }
    }
}
