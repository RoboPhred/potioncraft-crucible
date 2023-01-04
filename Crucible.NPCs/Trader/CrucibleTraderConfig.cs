// <copyright file="CrucibleTraderConfig.cs" company="RoboPhredDev">
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
    using System;
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Defines the configuration for a trader.
    /// </summary>
    public class CrucibleTraderConfig : CrucibleNPCConfig<CrucibleTraderNpcTemplate>
    {
        /// <summary>
        /// Gets or sets the minimum karma at which this trader can appear.
        /// </summary>
        public int MinimumKarmaForSpawn { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the maximum karma at which this trader can appear.
        /// </summary>
        public int MaximumKarmaForSpawn { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the chapter this trader unlocks at.
        /// </summary>
        public int UnlockAtChapter { get; set; }

        /// <summary>
        /// Gets or sets the trader's gold.
        /// </summary>
        public int Gold { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the array of items this trader has access to sell by default.
        /// </summary>
        public OneOrMany<CrucibleInventoryItemSoldByNpcStaticConfig> Items { get; set; }

        /// <summary>
        /// Gets or sets the day time for trader to spawn. 0 is at the start of the day and 100 is at the end of the day.
        /// </summary>
        public int DayTimeForSpawn { get; set; } = int.MaxValue;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override CrucibleTraderNpcTemplate GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;
            return CrucibleTraderNpcTemplate.GetTraderNpcTemplateById(id)
                    ?? CrucibleTraderNpcTemplate.CreateTraderNpcTemplate(id, this.InheritFrom);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleTraderNpcTemplate subject)
        {
            base.OnApplyConfiguration(subject);

            if (this.UnlockAtChapter > 0)
            {
                subject.UnlockAtChapter = this.UnlockAtChapter;
            }

            if (this.MinimumKarmaForSpawn != int.MaxValue && this.MaximumKarmaForSpawn != int.MaxValue)
            {
                subject.KarmaForSpawn = (this.MinimumKarmaForSpawn, this.MaximumKarmaForSpawn);
            }

            if (this.DayTimeForSpawn != int.MaxValue)
            {
                subject.DayTimeForSpawn = this.DayTimeForSpawn;
            }

            if (this.Items != null)
            {
                subject.ClearTradeItems();
                foreach (var item in this.Items)
                {
                    item.OnApplyConfiguration(subject);
                }
            }
        }
    }
}
