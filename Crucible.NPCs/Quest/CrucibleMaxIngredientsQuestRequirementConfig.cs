// <copyright file="CrucibleMaxIngredientsQuestRequirementConfig.cs" company="RoboPhredDev">
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
    /// Configuration specifying an addional quest requirement.
    /// </summary>
    public class CrucibleMaxIngredientsQuestRequirementConfig : CrucibleQuestRequirementConfig
    {
        /// <summary>
        /// Gets or sets the maximum number of ingredients allowed for this quest requirement. This should be a value between 1 and 3.
        /// </summary>
        public int MaximumIngredients { get; set; } = 3;

        /// <inheritdoc/>
        public override CrucibleQuestRequirement GetSubject()
        {
            if (this.MaximumIngredients < 0 || this.MaximumIngredients > 3)
            {
                throw new ArgumentException("Maximum ingredient quest requirement must be greater than 0 and less than 3.");
            }

            return CrucibleQuestRequirement.GetByName($"MaxIngredients{this.MaximumIngredients}");
        }
    }
}
