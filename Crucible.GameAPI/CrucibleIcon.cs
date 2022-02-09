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
    using ElementChangerWindow;
    using HarmonyLib;
    using PotionCustomizationWindow;
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
        /// Gets the ID of this icon.
        /// </summary>
        public string ID
        {
            get
            {
                return this.Icon.name;
            }
        }

        /// <summary>
        /// Gets the wrapped icon.
        /// </summary>
        public Icon Icon
        {
            get;
        }

        /// <summary>
        /// Gets or sets the icon texture.
        /// </summary>
        public Texture2D IconTexture
        {
            get
            {
                return this.Icon.textures[0];
            }

            set
            {
                this.Icon.textures[0] = value;
            }
        }

        /// <summary>
        /// Gets an icon by its ID.
        /// </summary>
        /// <param name="id">The ID of the icon to fetch.</param>
        /// <returns>A <see cref="CrucibleIcon"/> api wrapper for working with the requested icon, or <c>null</c> if no icon exists with the given ID.</returns>
        public static CrucibleIcon GetIconByID(string id)
        {
            var icon = Icon.allIcons.Find(x => x.name == id);
            return icon != null ? new CrucibleIcon(icon) : null;
        }

        /// <summary>
        /// Creates a new icon from the given texture.
        /// </summary>
        /// <param name="id">The name of this icon.</param>
        /// <param name="texture">The texture to create the icon from.</param>
        /// <returns>A <see cref="CrucibleIcon"/> object for working with the created <see cref="Icon"/>.</returns>
        public static CrucibleIcon FromTexture(string id, Texture2D texture)
        {
            if (Icon.allIcons.Find(x => x.name == id) != null)
            {
                throw new ArgumentException($"Icon with ID \"{id}\" already exists.", nameof(id));
            }

            var blankTexture = TextureUtilities.CreateBlankTexture(1, 1, new Color(0, 0, 0, 0));
            var icon = ScriptableObject.CreateInstance<Icon>();
            icon.name = id;
            icon.textures = new[] { texture };
            icon.contourTexture = blankTexture;
            icon.scratchesTexture = blankTexture;
            icon.defaultIconColors = new Color[0];

            RegisterIcon(icon);

            return new CrucibleIcon(icon);
        }

        /// <summary>
        /// Creates a brand new icon with data copied from this icon.
        /// </summary>
        /// <param name="id">The name of the new icon.</param>
        /// <returns>The new icon.</returns>
        public CrucibleIcon Clone(string id)
        {
            if (Icon.allIcons.Find(x => x.name == id) != null)
            {
                throw new ArgumentException($"Icon with ID \"{id}\" already exists.", nameof(id));
            }

            var icon = this.Icon;
            var clone = ScriptableObject.CreateInstance<Icon>();
            icon.name = id;
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

            // Clear the icon cache so our new sprite will generate on next request.
            this.ClearCache();
        }

        /// <summary>
        /// Clears the render cache for this icon.
        /// </summary>
        public void ClearCache()
        {
            var variants = Traverse.Create(this.Icon).Field("renderedVariants").GetValue() as IList;
            variants.Clear();
        }

        private static void RegisterIcon(Icon icon)
        {
            // This seems to be used for deserializing icon data from the save file.
            Icon.allIcons.Add(icon);

            // FIXME: Does every single icon get displayed here?
            // Should probably make this optional.
            var skinChangerWindow = GameObject.FindObjectOfType<PotionSkinChangerWindow>();
            if (skinChangerWindow == null)
            {
                return;
            }

            var iconSkinChangerField = Traverse.Create(skinChangerWindow).Field<ElementChangerPanelGroup>("iconSkinChangerPanelGroup");
            if (iconSkinChangerField == null)
            {
                return;
            }

            var panelGroup = iconSkinChangerField.Value;
            var mainPanel = panelGroup.mainPanel as ElementChangerPanelWithElements;
            if (mainPanel != null)
            {
                mainPanel.elements.Add(icon);
            }
        }
    }
}
