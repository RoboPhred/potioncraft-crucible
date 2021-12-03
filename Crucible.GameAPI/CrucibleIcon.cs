// <copyright file="CrucibleIcon.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="Icon"/>s.
    /// </summary>
    public sealed class CrucibleIcon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleIcon"/> class.
        /// </summary>
        /// <param name="icon">The icon to wrap.</param>
        public CrucibleIcon(Icon icon)
        {
            this.Icon = icon;
        }

        /// <summary>
        /// Gets the wrapped icon.
        /// </summary>
        public Icon Icon
        {
            get;
        }

        /// <summary>
        /// Creates a new icon from the given texture.
        /// </summary>
        /// <param name="name">The name of this icon.</param>
        /// <param name="texture">The texture to create the icon from.</param>
        /// <returns>A <see cref="CrucibleIcon"/> object for working with the created <see cref="Icon"/>.</returns>
        public static CrucibleIcon FromTexture(string name, Texture2D texture)
        {
            if (Icon.allIcons.Find(x => x.name == name) != null)
            {
                throw new ArgumentException($"Icon with name \"{name}\" already exists.", nameof(name));
            }

            var blankTexture = TextureUtilities.CreateBlankTexture(1, 1, new Color(0, 0, 0, 0));
            var icon = ScriptableObject.CreateInstance<Icon>();
            icon.name = name;
            icon.textures = new[] { texture };
            icon.contourTexture = blankTexture;
            icon.scratchesTexture = blankTexture;
            icon.defaultIconColors = new Color[0];

            // This seems to be used for deserializing icon data from the save file.
            Icon.allIcons.Add(icon);

            return new CrucibleIcon(icon);
        }

        /// <summary>
        /// Creates a brand new icon with data copied from this icon.
        /// </summary>
        /// <param name="name">The name of the new icon.</param>
        /// <returns>The new icon.</returns>
        public CrucibleIcon Clone(string name)
        {
            if (Icon.allIcons.Find(x => x.name == name) != null)
            {
                throw new ArgumentException($"Icon with name \"{name}\" already exists.", nameof(name));
            }

            var icon = this.Icon;
            var clone = ScriptableObject.CreateInstance<Icon>();
            icon.name = name;
            clone.textures = icon.textures;
            clone.contourTexture = icon.contourTexture;
            clone.scratchesTexture = icon.scratchesTexture;
            clone.defaultIconColors = icon.defaultIconColors;

            // This seems to be used for deserializing icon data from the save file.
            Icon.allIcons.Add(icon);

            return new CrucibleIcon(clone);
        }

        /// <summary>
        /// Sets the visual texture of this icon.
        /// </summary>
        /// <param name="texture">The texture to set the icon to.</param>
        public void SetTexture(Texture2D texture)
        {
            var icon = this.Icon;
            icon.textures = new[] { texture };
            icon.defaultIconColors = new[] { Color.white };
            this.ClearCache();
        }

        /// <summary>
        /// Clears the render cache for this icon.
        /// </summary>
        public void ClearCache()
        {
            // Clear the icon cache so our new sprite is generated.
            // TODO: Should make this into a utility class/func.
            // Do we want to try resetting the icon like this, or is it better to generate a new icon
            // and remove the old icon from Icon.allIcons?
            var variants = Traverse.Create(this.Icon).Field("renderedVariants").GetValue() as IList;
            variants.Clear();
        }
    }
}
