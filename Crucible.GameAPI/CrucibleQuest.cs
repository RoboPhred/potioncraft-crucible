// <copyright file="CrucibleQuest.cs" company="RoboPhredDev">
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
    using global::PotionCraft.Core.ValueContainers;
    using global::PotionCraft.QuestSystem;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="Quest"/>s.
    /// </summary>
    public class CrucibleQuest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleQuest"/> class using the provided <see cref="Quest"/>.
        /// </summary>
        /// <param name="quest">The <see cref="Quest"/> to use as a base.</param>
        public CrucibleQuest(Quest quest)
        {
            this.Quest = quest;
        }

        /// <summary>
        /// Gets or sets the base <see cref="Quest"/>.
        /// </summary>
        public Quest Quest { get; set; }

        /// <summary>
        /// Gets a value indicating whether the base quest is null.
        /// </summary>
        public bool IsNull => this.Quest == null;

        /// <summary>
        /// Gets or sets the uniqueId for this quest.
        /// </summary>
        public string ID
        {
            get => this.Quest.name;
            set => this.Quest.name = value;
        }

        /// <summary>
        /// Gets or sets the karma reward for completing this quest.
        /// </summary>
        public int KarmaReward
        {
            get => this.Quest.karmaReward;
            set => this.Quest.karmaReward = value;
        }

        /// <summary>
        /// Gets or sets the desired effects for the quest.
        /// </summary>
        public List<string> DesiredEffects
        {
            get => this.Quest.desiredEffects.Select(e => e.name).ToList();
            set => this.Quest.desiredEffects = value.Select(e => CruciblePotionEffect.GetPotionEffectByID(e))
                                                    .Select(e => e.PotionEffect)
                                                    .ToArray();
        }

        /// <summary>
        /// Gets or sets the minimum and maximum chapter needed to encounter this quest.
        /// </summary>
        public (int, int) MinMaxChapters
        {
            get
            {
                var minMaxValue = Traverse.Create(this.Quest).Field<MinMaxInt>("alchemistPathChapters").Value;
                return (minMaxValue.min, minMaxValue.max);
            }
            set => Traverse.Create(this.Quest).Field<MinMaxInt>("alchemistPathChapters").Value = new MinMaxInt(value.Item1, value.Item2);
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether or not random requirements will be generated for the mandatory requirements list.
        /// </summary>
        public bool GenerateRandomMandatoryRequirements
        {
            get => !Traverse.Create(this.Quest).Field<bool>("useListMandatoryRequirements").Value;
            set => Traverse.Create(this.Quest).Field<bool>("useListMandatoryRequirements").Value = !value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether or not random requirements will be generated for the optional requirements list.
        /// </summary>
        public bool GenerateRandomOptionalRequirements
        {
            get => !Traverse.Create(this.Quest).Field<bool>("useListOptionalRequirements").Value;
            set => Traverse.Create(this.Quest).Field<bool>("useListOptionalRequirements").Value = !value;
        }

        /// <summary>
        /// Clones the provided <see cref="CrucibleQuest"/>.
        /// </summary>
        /// <param name="copyFrom">The <see cref="CrucibleQuest"/> to clone.</param>
        /// <returns>A copy of the provided <see cref="CrucibleQuest"/>.</returns>
        public static CrucibleQuest Clone(CrucibleQuest copyFrom)
        {
            if (copyFrom.IsNull)
            {
                return new CrucibleQuest(null);
            }

            var newQuest = new CrucibleQuest(ScriptableObject.CreateInstance<Quest>())
            {
                KarmaReward = copyFrom.KarmaReward,
                DesiredEffects = copyFrom.DesiredEffects,
                MinMaxChapters = copyFrom.MinMaxChapters,
                GenerateRandomMandatoryRequirements = copyFrom.GenerateRandomMandatoryRequirements,
                GenerateRandomOptionalRequirements = copyFrom.GenerateRandomOptionalRequirements,
            };
            GetQuestRequirementsField(newQuest, true).Value = GetQuestRequirementsField(copyFrom, true).Value;
            GetQuestRequirementsField(newQuest, false).Value = GetQuestRequirementsField(copyFrom, false).Value;
            return newQuest;
        }

        /// <summary>
        /// Generates an empty <see cref="Quest"/> to use as a base.
        /// </summary>
        public void GenerateEmptyQuest()
        {
            this.Quest = ScriptableObject.CreateInstance<Quest>();
        }

        /// <summary>
        /// Adds a requirement to the mandatory requirements list.
        /// </summary>
        /// <param name="requirement">The <see cref="CrucibleQuestRequirement"/> to add.</param>
        public void AddMandatoryRequirement(CrucibleQuestRequirement requirement)
        {
            GetQuestRequirementsField(this, true).Value.Add(requirement.Requirement);
        }

        /// <summary>
        /// Adds a requirement to the optional requirements list.
        /// </summary>
        /// <param name="requirement">The <see cref="CrucibleQuestRequirement"/> to add.</param>
        public void AddOptionalRequirement(CrucibleQuestRequirement requirement)
        {
            GetQuestRequirementsField(this, false).Value.Add(requirement.Requirement);
        }

        /// <summary>
        /// Sets the localized text for the quest.
        /// </summary>
        /// <param name="questText">The main localized text for the quest.</param>
        /// <param name="subsequentQuestText">The localized text for subsequent instances of the quest.</param>
        public void SetQuestText(CrucibleDialogueData.CrucibleDialogueNode dialogue, LocalizedString subsequentQuestText)
        {
            var dialogueQuestNode = GetDialogueQuestNode(dialogue);

            if (dialogueQuestNode == null)
            {
                throw new ArgumentException("No text was provided for quest within dialogue tree!");
            }

            var localizationkey = $"unique_quest_text_{this.ID}_first_visit";
            CrucibleLocalization.SetLocalizationKey(localizationkey, dialogueQuestNode.DialogueText);
            var subsequentLocalizationkey = $"unique_quest_text_{this.ID}_other_visits";
            CrucibleLocalization.SetLocalizationKey(subsequentLocalizationkey, subsequentQuestText);
        }

        private static Traverse<List<QuestRequirementInQuest>> GetQuestRequirementsField(CrucibleQuest quest, bool mandatory)
        {
            return Traverse.Create(quest.Quest).Field<List<QuestRequirementInQuest>>(mandatory ? "mandatoryRequirements" : "optionalRequirements");
        }

        private static CrucibleDialogueData.CrucibleDialogueNode GetDialogueQuestNode(CrucibleDialogueData.CrucibleDialogueNode dialogue)
        {
            if (dialogue == null)
            {
                return null;
            }

            if (dialogue.IsQuestNode)
            {
                return dialogue;
            }

            if (!dialogue.Answers.Any())
            {
                return null;
            }

            return dialogue.Answers.Select(a => GetDialogueQuestNode(a.NextNode))
                                   .FirstOrDefault(n => n != null);
        }
    }
}
