// <copyright file="CrucibleTraderNpcTemplate.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Linq;
    using Npc.Parts;
    using Npc.Parts.Settings;
    using ObjectBased.Deliveries;

    /// <summary>
    /// Represents an NPC Template that contains trader data.
    /// </summary>
    public sealed class CrucibleTraderNpcTemplate : CrucibleNpcTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleTraderNpcTemplate"/> class.
        /// </summary>
        /// <param name="npcTemplate">The NPC Template to wrap.</param>
        internal CrucibleTraderNpcTemplate(NpcTemplate npcTemplate)
            : base(npcTemplate)
        {
            if (this.TraderSettings == null)
            {
                throw new ArgumentException("NPC Template does not contain a TraderSettings.", nameof(npcTemplate));
            }
        }

        /// <summary>
        /// Gets the base game Quest for this customer.
        /// </summary>
        public TraderSettings TraderSettings => this.NpcTemplate.baseParts.OfType<TraderSettings>().FirstOrDefault();

        /// <summary>
        /// If this npc is a trader, adds an item to this template's trader inventory.
        /// </summary>
        /// <remarks>
        /// The underlying <see cref="NpcTemplate"/> must have a <see cref="TraderSettings"/> part.
        /// </remarks>
        /// <param name="item">The inventory item to add to the trader's inventory.</param>
        /// <param name="chance">The chance of the trader having the item for any given appearance.</param>
        /// <param name="minCount">The minimum amount of the item to stock.</param>
        /// <param name="maxCount">The maximum amount of the item to stock.</param>
        public void AddTradeItem(CrucibleInventoryItem item, float chance = 1, int minCount = 1, int maxCount = 1)
        {
            var settings = this.TraderSettings;

            var crucibleCategory = settings.deliveriesCategories.Find(x => x.name == "Crucible");
            if (crucibleCategory == null)
            {
                crucibleCategory = new Category
                {
                    name = "Crucible",
                    deliveries = new List<Delivery>(),
                };
                settings.deliveriesCategories.Add(crucibleCategory);
            }

            // We could probably precreate the delivery outside the loop and re-use it,
            // but the game does not reuse them, so let's play it safe.
            crucibleCategory.deliveries.Add(new Delivery
            {
                item = item.InventoryItem,
                appearingChance = chance,
                minCount = minCount,
                maxCount = maxCount,
                applyDiscounts = true,
                applyExtraCharge = true,
            });
        }
    }
}
