// <copyright file="CruciblePotionBaseEffect.cs" company="RoboPhredDev">
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
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionEffectMapItem;

    /// <summary>
    /// Provides access to a potion effect on a potion base.
    /// </summary>
    public sealed class CruciblePotionBaseEffect : IEquatable<CruciblePotionBaseEffect>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePotionBaseEffect"/> class.
        /// </summary>
        /// <param name="item">The map item representing the potion effect.</param>
        internal CruciblePotionBaseEffect(PotionEffectMapItem item)
        {
            this.MapItem = item;
        }

        /// <summary>
        /// Gets the map item for this potion effect on the potion base.
        /// </summary>
        public PotionEffectMapItem MapItem
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the user has discovered this effect.
        /// </summary>
        public bool IsDiscovered
        {
            get
            {
                return this.MapItem.Status == PotionEffectStatus.Known || this.MapItem.Status == PotionEffectStatus.Collected;
            }
        }

        /// <summary>
        /// Discover the potion effect, if not already discovered.
        /// </summary>
        public void Discover()
        {
            if (this.IsDiscovered)
            {
                return;
            }

            this.MapItem.Status = PotionEffectStatus.Known;
        }

        /// <summary>
        /// Mark the status as undiscovered, hiding its type from the user.
        /// </summary>
        /// <remarks>
        /// This cannot be done if the effect has been collected for the current pending potion brew.
        /// </remarks>
        public void Undiscover()
        {
            if (this.MapItem.Status != PotionEffectStatus.Collected)
            {
                this.MapItem.Status = PotionEffectStatus.Unknown;
            }
        }

        /// <summary>
        /// Corrects the status of this potion effect, if it is set to an invalid value.
        /// </summary>
        /// <remarks>
        /// A potion effect can get an invalid status if a save file is loaded that did not contain the potion effect, or if the potion effect was
        /// relocated since the save was made.
        /// </remarks>
        public void FixInvalidStatus()
        {
            if (this.MapItem.Status == 0)
            {
                this.MapItem.Status = PotionEffectStatus.Unknown;
            }
        }

        /// <inheritdoc/>
        public bool Equals(CruciblePotionBaseEffect other)
        {
            return this.MapItem == other.MapItem;
        }
    }
}
