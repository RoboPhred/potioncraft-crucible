namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Event arguments for when localization data is resolved from a key.
    /// </summary>
    public class ScriptableObjectAtlasRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableObjectAtlasRequestEventArgs"/> class.
        /// </summary>
        /// <param name="obj">The scriptable object whose atlas is being resolved.</param>
        public ScriptableObjectAtlasRequestEventArgs(ScriptableObject obj)
        {
            this.Object = obj;
        }

        /// <summary>
        /// Gets the scriptable object whose atlas is being resolved.
        /// </summary>
        public ScriptableObject Object { get; }

        /// <summary>
        /// Gets or sets the resolved atlas name.
        /// </summary>
        public string AtlasResult { get; set; }
    }
}
