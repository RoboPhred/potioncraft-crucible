// <copyright file="CrucibleCustomerConfig.cs" company="RoboPhredDev">
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

    /// <summary>
    /// Defines the configuration for a customer.
    /// </summary>
    public class CrucibleCustomerConfig : CrucibleNPCConfig<CrucibleCustomerNpcTemplate>
    {
        /// <summary>
        /// Gets or sets the chance of this npc spawning.
        /// </summary>
        public float ChanceToAppear { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of days between NPC visits.
        /// </summary>
        public int MinimumDaysOfCooldown { get; set; } = -1;

        /// <summary>
        /// Gets or sets the minimum number of days between NPC visits.
        /// </summary>
        public int MaximumDaysOfCooldown { get; set; } = -1;

        /// <summary>
        /// Gets or sets the maximum level of closeness this customer can gain. This effectivly limits how many visits the customer will make to the shop in total.
        /// </summary>
        public int MaximumCloseness { get; set; } = -1;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override CrucibleCustomerNpcTemplate GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;
            return CrucibleCustomerNpcTemplate.GetCustomerNpcTemplateById(id)
                   ?? CrucibleCustomerNpcTemplate.CreateCustomerNpcTemplate(id, this.InheritFrom);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleCustomerNpcTemplate subject)
        {
            if (this.MaximumCloseness > 0)
            {
                subject.SetMaximumCloseness(this.MaximumCloseness);
            }

            // Customers must always have a quest node as their starting dialogue
            foreach(var quest in this.Quests)
            {
                if (!quest.Dialogue.HasQuestNode)
                {
                    quest.Dialogue.IsQuestNode = true;
                }
            }

            base.OnApplyConfiguration(subject);

            if (this.ChanceToAppear > 0)
            {
                subject.SpawnChance = this.ChanceToAppear;
            }

            if (this.MinimumDaysOfCooldown > 0 && this.MaximumDaysOfCooldown > 0)
            {
                if (this.MinimumDaysOfCooldown > this.MaximumDaysOfCooldown)
                {
                    throw new ArgumentException("MinimumDaysOfCooldown must be less than or equal to MaximumDaysOfCooldown!");
                }

                subject.DaysOfCooldown = (this.MinimumDaysOfCooldown, this.MaximumDaysOfCooldown);
            }
        }
    }
}
