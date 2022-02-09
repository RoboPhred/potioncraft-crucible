// <copyright file="CruciblePotionBottleStickerConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.PotionBottles
{
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration subject for a PotionCraft potion bottle sticker.
    /// </summary>
    public class CruciblePotionBottleStickerConfig : CruciblePackageConfigSubjectNode<CruciblePotionSticker>
    {
        /// <summary>
        /// Gets or sets the ID of this icon.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the foreground sprite.
        /// </summary>
        public Sprite Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background sprite.
        /// </summary>
        public Sprite Background { get; set; }

        /// <summary>
        /// Gets or sets the icon for this sticker in the sticker choice menu.
        /// </summary>
        public Sprite MenuIcon { get; set; }

        /// <inheritdoc/>
        protected override CruciblePotionSticker GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;

            return CruciblePotionSticker.CreatePotionSticker(id) ?? CruciblePotionSticker.CreatePotionSticker(id);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CruciblePotionSticker subject)
        {
            if (this.Foreground != null)
            {
                subject.Foreground = this.Foreground;
            }

            if (this.Background != null)
            {
                subject.Background = this.Background;
            }

            if (this.MenuIcon != null)
            {
                subject.MenuIcon = this.MenuIcon;
            }
        }
    }
}
