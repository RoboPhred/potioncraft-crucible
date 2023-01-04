// <copyright file="CrucibleInventoryItemSoldByConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.NPCs
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Configuration specifying data on which npc(s) sell a given item.
    /// </summary>
    [DuckTypeCandidate(typeof(CrucibleInventoryItemSoldByNpcTemplateConfig))]
    [DuckTypeCandidate(typeof(CrucibleInventoryItemSoldByNpcTagConfig))]
    [DuckTypeCandidate(typeof(CrucibleInventoryItemSoldByNpcStaticConfig))]
    public abstract class CrucibleInventoryItemSoldByConfig : CruciblePackageConfigNode
    {
        /// <summary>
        /// Gets or sets the chance of this item being sold.
        /// </summary>
        public float ChanceToAppear { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the minimum count of stock the trader will have.
        /// </summary>
        public int MinCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum count of stock the trader will have.
        /// </summary>
        public int MaxCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the closeness requirement for this item to be sold by this trader.
        /// </summary>
        public int? ClosenessRequirement { get; set; }

        /// <summary>
        /// Applies the configuration to the given inventory item.
        /// </summary>
        /// <param name="inventoryItem">The inventory item to apply the configuration to.</param>
        public virtual void OnApplyConfiguration(CrucibleInventoryItem inventoryItem)
        {
            foreach (var trader in this.GetTraders())
            {
                trader.AddTradeItem(inventoryItem, this.ChanceToAppear, this.MinCount, this.MaxCount, this.ClosenessRequirement ?? inventoryItem.DefaultClosenessRequirement);
            }
        }

        /// <summary>
        /// Enumerates the traders this item should be stocked by.
        /// </summary>
        /// <returns>An enumerable of npc templates that this item should be sold by.</returns>
        protected abstract IEnumerable<CrucibleTraderNpcTemplate> GetTraders();
    }
}
