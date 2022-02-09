// <copyright file="CrucibleGUIManagerBehavior.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GUI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Behavior that manages all Crucible GUI windows.
    /// </summary>
    internal class CrucibleGUIManagerBehavior : MonoBehaviour
    {
        private readonly HashSet<CrucibleGUIWindowImpl> windows = new();

        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <param name="title">The default title for the window.</param>
        /// <param name="render">The window render function.</param>
        /// <returns>The created window.</returns>
        public ICrucibleGUIWindow CreateWindow(string title, Action<ICrucibleGUIWindow> render)
        {
            var window = new CrucibleGUIWindowImpl(title, render);
            window.OnCloseRequested += this.OnWindowCloseRequested;
            this.windows.Add(window);
            return window;
        }

        /// <summary>
        /// Called by Unity to render the GUI.
        /// </summary>
        public void OnGUI()
        {
            foreach (var window in this.windows)
            {
                window.OnGUI();
            }
        }

        private void OnWindowCloseRequested(object sender, EventArgs e)
        {
            var window = sender as CrucibleGUIWindowImpl;
            this.windows.Remove(window);
            window.Destroy();
        }
    }
}
