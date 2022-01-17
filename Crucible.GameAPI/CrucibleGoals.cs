// <copyright file="CrucibleGoals.cs" company="RoboPhredDev">
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

#if CRUCIBLE_GOALS
namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using Books.GoalsBook;
    using HarmonyLib;
    using UnityEngine;

    public sealed class CrucibleGoals
    {
        public static void TestGoal()
        {
            // NOTE: Only works after the game has initialized its data.  See CrucibleGameEvents.OnGameInitialized

            var dummyChapter = Managers.Goals.settings.chaptersGroups[0].chapters[0];

            Debug.Log("Adding group");
            var group = new ChaptersGroup
            {
                name = "test_group",
            };
            Managers.Goals.settings.chaptersGroups.Add(group);
            CrucibleLocalization.SetLocalizationKey(group.GetLocalizationPrefix() + "_goals_book_chapter_title", "Test Group");
            CrucibleLocalization.SetLocalizationKey("finish_current_chapter_to_complete_" + group.GetLocalizationPrefix(), "Finish current chapter to complete Test Group");

            Debug.Log("Adding chapter");
            var chapter = new Chapter
            {
                name = "test_chapter",
                goals = new(),
                chapterCompletedSprite = dummyChapter.chapterCompletedSprite,
                chapterMarkerActiveIcon = dummyChapter.chapterMarkerActiveIcon,
                chapterMarkerIdleIcon = dummyChapter.chapterMarkerIdleIcon,
            };
            group.chapters.Add(chapter);

            Debug.Log("Building first goal");

            var goal = ScriptableObject.CreateInstance<Goal>();
            goal.name = "test_goal";
            CrucibleLocalization.SetLocalizationKey("goal_" + goal.name, "Test Goal");
            goal.targetValue = 3;
            goal.inBook = true;
            goal.showProgress = true;

            Debug.Log("Adding first goal");
            chapter.goals.Add(goal);
            Traverse.Create(typeof(GoalsLoader)).Field("AllGoals").GetValue<Dictionary<string, Goal>>().Add(goal.name, goal);

            // Overall goal for the chapter.  Required.
            Debug.Log("Building chapter goal");
            var chapterGoal = ScriptableObject.CreateInstance<Goal>();
            chapterGoal.name = "chapter_goal";
            CrucibleLocalization.SetLocalizationKey("goal_" + chapterGoal.name, "Chapter Goal");
            chapterGoal.targetValue = 1;
            chapterGoal.inBook = true;
            chapterGoal.showProgress = true;
            chapter.chapterGoal = chapterGoal;

            Debug.Log("Adding chapter goal");
            Traverse.Create(typeof(GoalsLoader)).Field("AllGoals").GetValue<Dictionary<string, Goal>>().Add(chapterGoal.name, goal);

            // This needs to be done after the game loads, as the game will deserialize the bookmarks from the save file.
            CrucibleGameEvents.OnSaveLoaded += (_, __) =>
            {
                Debug.Log("Adding bookmark");

                // Reproduced from GoalsBook.AddPagesOnBookStart, adapted to add missing pages instead of adding all.
                var groupController = Managers.Goals.goalsBook.bookmarkControllersGroupController;
                var chaptersGroups = Managers.Goals.settings.chaptersGroups;
                var bookmarkController = groupController.controllers[0].bookmarkController;
                var existingBookmarksCount = bookmarkController.GetAllBookmarksList().Count;
                var bookmarkIndex = 0;
                for (int index1 = 0; index1 < chaptersGroups.Count; ++index1)
                {
                    for (int index2 = 0; index2 < chaptersGroups[index1].chapters.Count; ++index2)
                    {
                        // Adapted code: Only add if bookmark does not exist.
                        if (bookmarkIndex >= existingBookmarksCount)
                        {
                            // FIXME: Game wants to find a bookmark controller per index1, but adding that is complicated and seemingly done by the unity editor.
                            bookmarkController.AddNewBookmark();
                        }

                        bookmarkIndex++;
                    }
                }

                Traverse.Create(Managers.Goals.goalsBook).Method("UpdateBookmarksContents").GetValue();
            };

            Debug.Log("Adding xp");
            GoalsLoader.GetGoalByName(goal.name).ProgressIncrement(2);
        }
    }
}
#endif
