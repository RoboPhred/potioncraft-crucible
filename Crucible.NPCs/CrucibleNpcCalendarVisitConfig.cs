// <copyright file="CrucibleNpcCalendarVisitConfig.cs" company="RoboPhredDev">
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
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;

    /// <summary>
    /// Configuration for specifying on what day an npc appears.
    /// </summary>
    public class CrucibleNpcCalendarVisitConfig
    {
        /// <summary>
        /// Gets or sets the day this npc will appear on.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Adds the given npc template to this day.
        /// </summary>
        /// <param name="npcTemplate">The npc template to add to the configured day.</param>
        public void AddToDay(CrucibleNpcTemplate npcTemplate)
        {
            if (this.Day == 0)
            {
                return;
            }

            //CrucibleDayManager.BackfillDaysTo(this.Day);
            //var count = CrucibleDayManager.GetNpcTemplatesForDay(this.Day).Count();
            //CrucibleDayManager.InsertNpcTemplateToDay(this.Day, count, npcTemplate);
        }
    }
}

