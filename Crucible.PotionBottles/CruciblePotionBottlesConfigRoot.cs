// <copyright file="CruciblePotionBottlesConfigRoot.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;

    /// <summary>
    /// The configuration root for potion bottles.
    /// </summary>
    [CruciblePackageConfigRoot]
    public class CruciblePotionBottlesConfigRoot : CruciblePackageConfigRoot
    {
        /// <summary>
        /// Gets or sets a list of potion bottles.
        /// </summary>
        public List<CruciblePotionBottleConfig> PotionBottles { get; set; } = new();

        /// <summary>
        /// Gets or sets a list of potion bottle icons.
        /// </summary>
        public List<CruciblePotionBottleIconConfig> PotionBottleIcons { get; set; } = new();

        /// <summary>
        /// Gets or sets a list of potion bottle stickers.
        /// </summary>
        public List<CruciblePotionBottleStickerConfig> PotionBottleStickers { get; set; } = new();

        /// <inheritdoc/>
        public override void ApplyConfiguration()
        {
            this.PotionBottles.ForEach(x => x.ApplyConfiguration());
            this.PotionBottleIcons.ForEach(x => x.ApplyConfiguration());
            this.PotionBottleStickers.ForEach(x => x.ApplyConfiguration());
        }
    }
}
