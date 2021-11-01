// <copyright file="ScriptableObjectAtlasRequestEventArgs.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Foobar is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

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
