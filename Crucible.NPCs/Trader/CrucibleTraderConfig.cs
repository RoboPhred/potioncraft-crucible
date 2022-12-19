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
        /// Gets or sets the visual mood of the faction ("Bad", "Normal", "Good").
        /// </summary>
        public string VisualMood { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MinimumDaysOfCooldown { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MaximumDaysOfCooldown { get; set; }

        /// <summary>
        /// Gets or sets the gender of the trader. Current options are "Male" and "Female".
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the haggling themes for each difficulty of haggling
        /// </summary>
        public CrucibleHagglingThemesConfig HagglingThemes { get; set; }

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
                    ?? CrucibleTraderNpcTemplate.CreateTraderNpcTemplate(id, this.CopyFrom);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleTraderNpcTemplate subject)
        {
            base.OnApplyConfiguration(subject);

            if (this.UnlockAtChapter > 0)
            {
                subject.UnlockAtChapter = this.UnlockAtChapter;
            }

            if (!string.IsNullOrEmpty(this.VisualMood))
            {
                subject.VisualMood = this.VisualMood;
            }

            if (this.MinimumDaysOfCooldown > 0 && this.MaximumDaysOfCooldown > 0)
            {
                subject.DaysOfCooldown = (this.MinimumDaysOfCooldown, this.MaximumDaysOfCooldown);
            }

            if (this.MinimumKarmaForSpawn != int.MaxValue && this.MaximumKarmaForSpawn != int.MaxValue)
            {
                subject.KarmaForSpawn = (this.MinimumKarmaForSpawn, this.MaximumKarmaForSpawn);
            }

            if (!string.IsNullOrEmpty(this.Gender))
            {
                subject.Gender = this.Gender;
            }

            this.HagglingThemes?.ApplyConfiguration(subject);

            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    item.OnApplyConfiguration(subject);
                }
            }
        }
    }
}
