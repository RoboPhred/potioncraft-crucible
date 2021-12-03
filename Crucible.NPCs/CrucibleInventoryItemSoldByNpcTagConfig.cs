// <copyright file="CrucibleInventoryItemSoldByNpcTagConfig.cs" company="RoboPhredDev">
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
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// A config entry for specifying a specific npc template to sell an item.
    /// </summary>
    public class CrucibleInventoryItemSoldByNpcTagConfig : CrucibleInventoryItemSoldByConfig
    {
        /// <summary>
        /// Gets or sets the collection of npc template names that will stock this item.
        /// </summary>
        public OneOrMany<string> NpcTag { get; set; }

        /// <inheritdoc/>
        protected override IEnumerable<CrucibleTraderNpcTemplate> GetTraders()
        {
            if (this.NpcTag == null)
            {
                return new CrucibleTraderNpcTemplate[0];
            }

            var traders = from template in CrucibleNpcTemplate.GetAllNpcTemplates()
                          where template.IsTrader
                          where this.NpcTag.All(x => template.HasTag(x))
                          select template.AsTrader();
            return traders.Distinct();
        }
    }
}
