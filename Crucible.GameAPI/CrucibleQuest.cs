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
            Traverse.Create(this.Quest).Field<List<QuestRequirementInQuest>>("mandatoryRequirements").Value.Add(requirement.Requirement);
        }

        /// <summary>
        /// Adds a requirement to the optional requirements list.
        /// </summary>
        /// <param name="requirement">The <see cref="CrucibleQuestRequirement"/> to add.</param>
        public void AddOptionalRequirement(CrucibleQuestRequirement requirement)
        {
            Traverse.Create(this.Quest).Field<List<QuestRequirementInQuest>>("optionalRequirements").Value.Add(requirement.Requirement);
        }
    }
}
