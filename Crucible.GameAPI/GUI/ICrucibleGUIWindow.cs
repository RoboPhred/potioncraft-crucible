// <copyright file="ICrucibleGUIWindow.cs" company="RoboPhredDev">
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
    /// <summary>
    /// A GUI window managed by Crucible.
    /// </summary>
    public interface ICrucibleGUIWindow
    {
        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window is visible.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        float Height { get; set; }

        /// <summary>
        /// Gets or sets the X position of the window.
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the window.
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
