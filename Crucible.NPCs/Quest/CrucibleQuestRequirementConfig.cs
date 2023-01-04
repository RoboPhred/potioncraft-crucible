// <copyright file="CrucibleQuestRequirementConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Configuration specifying an addional quest requirement.
    /// </summary>
    [DuckTypeCandidate(typeof(CrucibleAdditionalEffectsQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleMainIngredientQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleMaxIngredientsQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleNeedIngredientQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleNoIngredientQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleStrongPotionQuestRequirementConfig))]
    [DuckTypeCandidate(typeof(CrucibleWeakPotionQuestRequirementConfig))]
    public abstract class CrucibleQuestRequirementConfig : CruciblePackageConfigNode
    {
        /// <summary>
        /// Gets the subject corresponding to the type of requirement.
        /// </summary>
        /// <returns>The subject corresponding to the type of requirement.</returns>
        public abstract CrucibleQuestRequirement GetSubject();

        /// <summary>
        /// Applies configuration to the subject.
        /// </summary>
        /// <param name="reqSubject">The subject to apply configuration to.</param>
        public virtual void ApplyConfiguration(CrucibleQuestRequirement reqSubject)
        {
        }
    }
}
