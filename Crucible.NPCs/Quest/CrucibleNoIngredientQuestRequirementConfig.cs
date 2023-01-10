// <copyright file="CrucibleNoIngredientQuestRequirementConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;

    /// <summary>
    /// Configuration specifying an addional quest requirement.
    /// </summary>
    public class CrucibleNoIngredientQuestRequirementConfig : CrucibleQuestRequirementConfig
    {
        /// <summary>
        /// Gets or sets the ingredient ID which cannot be included for this quest requirement.
        /// </summary>
        public string IngredientNotAllowed { get; set; }

        /// <inheritdoc/>
        public override CrucibleQuestRequirement GetSubject()
        {
            return CrucibleQuestRequirement.GetByName("NoParticularIngredient");
        }

        /// <inheritdoc/>
        public override void ApplyConfiguration(CrucibleQuestRequirement reqSubject)
        {
            reqSubject.RequirementIngredient = this.IngredientNotAllowed;
        }
    }
}
