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
        public string CopyFrom { get; set; }

        /// <summary>
        /// Gets or sets the appearance configuration for this npc.
        /// </summary>
        public CrucibleNpcAppearanceConfig Appearance { get; set; }

        /// <summary>
        /// Gets or sets the list of dialogues for this NPC.
        /// </summary>
        public OneOrMany<CrucibleNPCDialogueConfig> Dialogue { get; set; } = new ();

        /// <summary>
        /// Gets or sets the list of closeness quests for this NPC.
        /// </summary>
        public OneOrMany<CrucibleNPCClosenessQuestConfig> Quests { get; set; } = new ();

        /// <summary>
        /// Gets or sets the tags associated with this NPC.
        /// This allows ingredient mods to easily target this NPC if it is a trader without needing to know the template name.
        /// </summary>
        public OneOrMany<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the day time for npc to spawn. 0 is at the start of the day and 100 is at the end of the day.
        /// </summary>
        public int DayTimeForSpawn { get; set; } = int.MaxValue;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(T subject)
        {
            if (this.DayTimeForSpawn != int.MaxValue)
            {
                subject.DayTimeForSpawn = this.DayTimeForSpawn;
            }

            this.Appearance?.ApplyAppearance(subject);

            // Apply quests
            subject.PrepareClosenessQuestsForNewQuests();
            var targetQuestList = subject.ClosenessQuests;
            foreach (var quest in this.Quests)
            {
                if (quest.ClosenessLevel < 0)
                {
                    throw new ArgumentException($"Quest ClosenessLevel must be greater than zero!");
                }

                if (quest.ClosenessLevel >= targetQuestList.Count)
                {
                    throw new ArgumentException($"Given quest ClosenessLevel is larger than the maximum closeness for this trader ({targetQuestList.Count})");
                }

                var currentTarget = targetQuestList[quest.ClosenessLevel];
                if (currentTarget.IsNull)
                {
                    currentTarget.GenerateEmptyQuest();
                }

                quest.ApplyConfiguration(currentTarget);
            }

            // Apply dialogues
            var orderedDialogues = this.Dialogue.OrderByDescending(d => d.ClosnessRequirement).ToList();
            var appliedDialogues = 0;
            for (var closeness = 0; closeness < subject.MaximumCloseness; closeness++)
            {
                var dialogueToApply = orderedDialogues.FirstOrDefault(d => d.ClosnessRequirement <= closeness);
                if (dialogueToApply == null)
                {
                    continue;
                }

                subject.ApplyDialogueForClosenessLevel(closeness, dialogueToApply.Dialogue);
                appliedDialogues++;
            }

            if (appliedDialogues < this.Dialogue.Count)
            {
                CrucibleLog.Log("Some dialogues were not applied to NPC due to issues with specified closeness requirements.");
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
