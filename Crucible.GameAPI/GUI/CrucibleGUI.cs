// <copyright file="CrucibleGUI.cs" company="RoboPhredDev">
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
    using UnityEngine;

    /// <summary>
    /// Utilities for working with themed PotionCraft GUI windows.
    /// </summary>
    public static class CrucibleGUI
    {
        private static readonly Lazy<CrucibleGUIManagerBehavior> Manager = new(CreateWindowManager);

        /// <summary>
        /// Creates a new GUI window.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="render">A function to render the window using <c>GUILayout</c>.</param>
        /// <returns>A reference to the window.</returns>
        public static ICrucibleGUIWindow CreateWindow(string title, Action<ICrucibleGUIWindow> render)
        {
            return CreateWindow(title, 200, 200, render);
        }

        /// <summary>
        /// Creates a new GUI window.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The default width of the window.</param>
        /// <param name="height">The default height of the window.</param>
        /// <param name="render">A function to render the window using <see cref="GUILayout"/>.</param>
        /// <returns>A reference to the window.</returns>
        public static ICrucibleGUIWindow CreateWindow(string title, int width, int height, Action<ICrucibleGUIWindow> render)
        {
            var window = Manager.Value.CreateWindow(title, render);
            UnityEngine.Debug.Log($"Created window {window.Title} at {window.X}, {window.Y} with size {window.Width}, {window.Height}");
            window.Visible = true;
            window.Width = width;
            window.Height = height;
            return window;
        }

        private static CrucibleGUIManagerBehavior CreateWindowManager()
        {
            var manager = new GameObject("Crucible GUI Window Manager");
            UnityEngine.Object.DontDestroyOnLoad(manager);
            return manager.AddComponent<CrucibleGUIManagerBehavior>();
        }
    }
}
