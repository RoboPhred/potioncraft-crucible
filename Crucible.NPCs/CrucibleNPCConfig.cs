// <copyright file="CrucibleNPCConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines the configuration for an npc.
    /// </summary>
    /// <typeparam name="T">The type of npc template to populate from this config.</typeparam>
    public abstract class CrucibleNPCConfig<T> : CruciblePackageConfigSubjectNode<T>
        where T : CrucibleNpcTemplate
    {
        /// <summary>
        /// Gets or sets the ID of this NPC.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the template to copy this NPC from.
        /// </summary>
        public string InheritFrom { get; set; } = "Brewer";

        /// <summary>
        /// Gets or sets the maximum level of closeness this customer can gain. This effectivly limits how many visits the customer will make to the shop in total.
        /// </summary>
        public int MaximumCloseness { get; set; } = -1;

        /// <summary>
        /// Gets or sets the minimum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MinimumDaysOfCooldown { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MaximumDaysOfCooldown { get; set; }

        /// <summary>
        /// Gets or sets the appearance configuration for this npc.
        /// </summary>
        public CrucibleNpcAppearanceConfig Appearance { get; set; }

        /// <summary>
        /// Gets or sets the list of closeness quests for this NPC.
        /// </summary>
        public OneOrMany<CrucibleNPCDialogueQuestConfig> Quests { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tags associated with this NPC.
        /// This allows ingredient mods to easily target this NPC if it is a trader without needing to know the template name.
        /// </summary>
        public OneOrMany<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the gender of the trader. Current options are "Male" and "Female".
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the visual mood of the faction ("Bad", "Normal", "Good").
        /// </summary>
        public string VisualMood { get; set; }

        /// <summary>
        /// Gets or sets the haggling themes for each difficulty of haggling.
        /// </summary>
        public CrucibleHagglingThemesConfig HagglingThemes { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(T subject)
        {
            this.Appearance?.ApplyAppearance(subject);

            if (this.MinimumDaysOfCooldown > 0 && this.MaximumDaysOfCooldown > 0)
            {
                if (this.MinimumDaysOfCooldown > this.MaximumDaysOfCooldown)
                {
                    throw new ArgumentException("MinimumDaysOfCooldown must be less than or equal to MaximumDaysOfCooldown!");
                }

                subject.DaysOfCooldown = (this.MinimumDaysOfCooldown, this.MaximumDaysOfCooldown);
            }

            if (!string.IsNullOrEmpty(this.Gender))
            {
                subject.Gender = this.Gender;
            }

            if (!string.IsNullOrEmpty(this.VisualMood))
            {
                subject.VisualMood = this.VisualMood;
            }

            if (this.MaximumCloseness > 0)
            {
                subject.SetMaximumCloseness(this.MaximumCloseness);
            }

            this.HagglingThemes?.ApplyConfiguration(subject);

            // Apply quests
            subject.PrepareClosenessQuestsForNewQuests();
            var targetQuestList = subject.ClosenessQuests;
            var orderedQuests = this.Quests.OrderByDescending(d => d.ClosenessRequirement).ToList();
            var appliedQuests = 0;
            var maxCloseness = subject.MaximumCloseness;
            for (var closeness = 0; closeness < maxCloseness; closeness++)
            {
                var questToApply = orderedQuests.FirstOrDefault(d => d.ClosenessRequirement >= 0 && d.ClosenessRequirement <= closeness);
                if (questToApply == null)
                {
                    continue;
                }

                var applyQuest = subject is CrucibleCustomerNpcTemplate || questToApply.ClosenessRequirement == closeness;
                if ((applyQuest && questToApply.Quest != null) || subject is CrucibleCustomerNpcTemplate)
                {
                    if (questToApply.Quest == null)
                    {
                        throw new ArgumentException("Dialgue without quests is not allowed for customer npcs. Each unique dialogue tree must also define a quest.");
                    }

                    var quest = subject.ClosenessQuests[closeness];

                    // If no id was provided generate a unique quest id using the closeness level
                    if (string.IsNullOrEmpty(questToApply.Quest.ID) || (int.TryParse(questToApply.Quest.ID, out int intId) && intId == closeness - 1))
                    {
                        questToApply.Quest.ID = closeness.ToString();
                    }

                    // Ensure the quest id will be unique across all quests by combining it with the npc id
                    questToApply.Quest.ID = subject.ID + "." + questToApply.Quest.ID;
                    questToApply.Quest.ApplyConfiguration(quest);
                    quest.SetQuestText(questToApply.Dialogue, questToApply.Quest.SubsequentVisitsQuestText);
                    appliedQuests++;
                }

                subject.ApplyDialogueForClosenessLevel(closeness, questToApply.Dialogue, applyQuest);
            }

            if (appliedQuests < this.Quests.Count)
            {
                CrucibleLog.Log($"Some quests were not applied to NPC due to issues with specified closeness requirements. Be sure you have specified a closeness requirement for each quest and that each closeness is less than the maximum closeness level ({subject.MaximumCloseness}).");
            }

            if (this.Tags != null)
            {
                foreach (var tag in this.Tags)
                {
                    subject.AddTag(tag);
                }
            }
        }
    }
}
