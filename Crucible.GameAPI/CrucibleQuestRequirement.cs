// <copyright file="CrucibleQuestRequirement.cs" company="RoboPhredDev">
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
    using global::PotionCraft.QuestSystem;
    using global::PotionCraft.ScriptableObjects.Ingredient;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="QuestRequirementInQuest"/>s.
    /// </summary>
    public class CrucibleQuestRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleQuestRequirement"/> class based on the provided <see cref="QuestRequirementInQuest"/>.
        /// </summary>
        /// <param name="requirement">The <see cref="QuestRequirementInQuest"/> to use as a base.</param>
        public CrucibleQuestRequirement(QuestRequirementInQuest requirement)
        {
            this.Requirement = requirement;
        }

        /// <summary>
        /// Gets or sets the base <see cref="QuestRequirementInQuest"/>.
        /// </summary>
        public QuestRequirementInQuest Requirement { get; set; }

        /// <summary>
        /// Gets or sets the ingredient used for this requirement.
        /// </summary>
        public string RequirementIngredient 
        {
            get => this.Requirement.ingredient.name;
            set => this.Requirement.ingredient = Ingredient.GetByName(value);
        }

        /// <summary>
        /// Gets the <see cref="CrucibleQuestRequirement"/> instance by name.
        /// </summary>
        /// <param name="requirementName">The name of the requirement.</param>
        /// <returns>The <see cref="CrucibleQuestRequirement"/> matching the provided name.</returns>
        public static CrucibleQuestRequirement GetByName(string requirementName)
        {
            return new CrucibleQuestRequirement(QuestRequirementInQuest.GetByName(requirementName));
        }
    }
}
