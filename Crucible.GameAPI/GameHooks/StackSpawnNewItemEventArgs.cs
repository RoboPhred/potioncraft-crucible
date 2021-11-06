// <copyright file="StackSpawnNewItemEventArgs.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using ObjectBased.Stack;

    /// <summary>
    /// Event args for when a stack item spawns.
    /// </summary>
    public class StackSpawnNewItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackSpawnNewItemEventArgs"/> class.
        /// </summary>
        /// <param name="stack">The spawning stack.</param>
        public StackSpawnNewItemEventArgs(Stack stack)
        {
            this.Stack = stack;
        }

        /// <summary>
        /// Gets the spawning stack.
        /// </summary>
        public Stack Stack { get; }
    }
}
