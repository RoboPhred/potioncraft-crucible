// <copyright file="CrucibleDayManager.cs" company="RoboPhredDev">
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
    using Npc.Parts;

    /// <summary>
    /// Manages the creation and manipulation of calendar days.
    /// </summary>
    public static class CrucibleDayManager
    {
        /// <summary>
        /// Ensures at least <paramref name="dayNumber"/> days exist in the calendar.
        /// If more days are needed, the groundhog day is used as backfill.
        /// </summary>
        /// <param name="dayNumber">The number of days the calendar should contain.</param>
        public static void BackfillDaysTo(int dayNumber)
        {
            if (Managers.Day.calendar.Length > dayNumber)
            {
                return;
            }

            var days = Managers.Day.calendar.ToList();
            for (var i = days.Count; i <= dayNumber; i++)
            {
                var day = UnityEngine.Object.Instantiate(Managers.Day.settings.groundhogDay);
                days.Add(day);
            }

            Managers.Day.calendar = days.ToArray();
        }

        /// <summary>
        /// Gets all npc templates associated with the given day.
        /// </summary>
        /// <param name="dayNumber">The absolute day number to retrieve templates for.</param>
        /// <returns>An enumerable of npc templates contained in the day.</returns>
        public static IEnumerable<CrucibleNpcTemplate> GetNpcTemplatesForDay(int dayNumber)
        {
            if (dayNumber < 0 || dayNumber > Managers.Day.calendar.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dayNumber));
            }

            var day = Managers.Day.calendar[dayNumber];
            return day.templatesToSpawn.OfType<NpcTemplate>().Select(x => new CrucibleNpcTemplate(x));
        }

        /// <summary>
        /// Inserts the given npc template into the specified day at the specified index.
        /// </summary>
        /// <param name="dayNumber">The absolute day number to insert the npc into.</param>
        /// <param name="index">The index in the npc template list to insert the npc at.</param>
        /// <param name="npcTemplate">The template of the npc to insert.</param>
        public static void InsertNpcTemplateToDay(int dayNumber, int index, CrucibleNpcTemplate npcTemplate)
        {
            if (dayNumber < 0 || dayNumber > Managers.Day.calendar.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dayNumber));
            }

            var day = Managers.Day.calendar[dayNumber];
            var template = npcTemplate.NpcTemplate;
            day.templatesToSpawn.Insert(index, template);
        }
    }
}
