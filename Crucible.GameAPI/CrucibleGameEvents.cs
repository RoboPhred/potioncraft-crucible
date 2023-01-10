// <copyright file="CrucibleGameEvents.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// A static class exposing various static game events.
    /// </summary>
    public static class CrucibleGameEvents
    {
        private static bool isInitialized;

        private static EventHandler<CrucibleSaveEventArgs> onSaveLoaded;
        private static EventHandler<CrucibleSaveEventArgs> onSaveSaved;

        /// <summary>
        /// Raised when the game has finished loading its initial data.
        /// </summary>
        /// <remarks>
        /// This is called once the game has finished loading and the user is at the main menu.
        /// It is generally not safe to try to access the game before this event, as game objects that make up critical game components
        /// might not yet be loaded.
        /// </remarks>
        public static event EventHandler OnGameInitialized
        {
            add
            {
                EnsureInitialized();
                GameInitEvent.OnGameInitialized += value;
            }

            remove
            {
                GameInitEvent.OnGameInitialized -= value;
            }
        }

        /// <summary>
        /// Raised when a save file is loaded.
        /// </summary>
        /// <remarks>
        /// This provides an opportunity for other mods to load data previously saved.
        /// The <see cref="CrucibleSaveEventArgs"/> arguments provides access to saving data to the save file, which may be saved
        /// using <see cref="OnSaveSaved"/>.
        /// </remarks>
        public static event EventHandler<CrucibleSaveEventArgs> OnSaveLoaded
        {
            add
            {
                EnsureInitialized();
                onSaveLoaded += value;
            }

            remove
            {
                onSaveLoaded -= value;
            }
        }

        /// <summary>
        /// Raised when a save file is saved.
        /// </summary>
        /// <remarks>
        /// This provides an opportunity for other mods to store data into the save file.
        /// The <see cref="CrucibleSaveEventArgs"/> arguments provides access to saving data to the save file, which may be loaded
        /// using <see cref="OnSaveLoaded"/>.
        /// </remarks>
        public static event EventHandler<CrucibleSaveEventArgs> OnSaveSaved
        {
            add
            {
                EnsureInitialized();
                onSaveSaved += value;
            }

            remove
            {
                onSaveSaved -= value;
            }
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            SaveLoadEvent.OnGameLoaded += HandleSaveLoaded;
            SaveLoadEvent.OnGameSaved += HandleSaveSaved;
        }

        private static void HandleSaveLoaded(object sender, SaveLoadEventArgs e)
        {
            if (onSaveLoaded == null)
            {
                return;
            }

            using var saveFile = new CrucibleSaveFile(e.File);
            onSaveLoaded.Invoke(null, new CrucibleSaveEventArgs(saveFile));
        }

        private static void HandleSaveSaved(object sender, SaveLoadEventArgs e)
        {
            if (onSaveSaved == null)
            {
                return;
            }

            using var saveFile = new CrucibleSaveFile(e.File);
            onSaveSaved.Invoke(null, new CrucibleSaveEventArgs(saveFile));
        }
    }
}
