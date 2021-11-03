// <copyright file="IncompatibleNpcTemplateException.cs" company="RoboPhredDev">
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

    /// <summary>
    /// An exception thrown when an operation is performed on an NPC Template that is incompatible with that template.
    /// </summary>
    public class IncompatibleNpcTemplateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleNpcTemplateException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public IncompatibleNpcTemplateException(string message)
            : base(message)
        {
        }
    }
}
