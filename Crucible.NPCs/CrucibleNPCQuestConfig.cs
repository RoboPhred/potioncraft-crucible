// <copyright file="CrucibleNPCQuestConfig.cs" company="RoboPhredDev">
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
    /// Configuration specifying the chance a quest with a specific effect request will be chosen.
    /// </summary>
    public class CrucibleNPCQuestConfig : CruciblePackageConfigNode
    {
        /// <summary>
        /// Gets or sets the id of the quest. This should be unique among all quests.
        /// </summary>
        public string QuestID { get; set; }

        /// <summary>
        /// Gets or sets the main quest text.
        /// </summary>
        public LocalizedString QuestText { get; set; }

        /// <summary>
        /// Gets or sets the karma reward for the quest. This can be negative or positive.
        /// </summary>
        public int KarmaReward { get; set; } = 0;

        /// <summary>
        /// Gets or sets the desired effects for the quest.
        /// </summary>
        public OneOrMany<string> DesiredEffects { get; set; } = new OneOrMany<string>();

        /// <summary>
        /// Gets or sets the minimum chapter needed to encounter this quest.
        /// </summary>
        public int MinimumChapter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum chapter this quest can be encounters on.
        /// </summary>
        public int MaximumChapter { get; set; } = 10;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether or not random requirements will be generated for the mandatory requirements list.
        /// </summary>
        public bool GenerateRandomMandatoryRequirements { get; set; } = false;

        /// <summary>
        /// Gets or sets the mandatory requirements for the quest.
        /// </summary>
        public List<CrucibleQuestRequirementConfig> MandatoryRequirements { get; set; } = new List<CrucibleQuestRequirementConfig>();

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether or not random requirements will be generated for the optional requirements list.
        /// </summary>
        public bool GenerateRandomOptionalRequirements { get; set; } = false;

        /// <summary>
        /// Gets or sets the optional requirements for the quest.
        /// </summary>
        public List<CrucibleQuestRequirementConfig> OptionalRequirements { get; set; } = new List<CrucibleQuestRequirementConfig>();
    }
}
