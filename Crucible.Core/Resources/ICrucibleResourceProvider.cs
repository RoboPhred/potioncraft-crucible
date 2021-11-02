// <copyright file="ICrucibleResourceProvider.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    /// <summary>
    /// Defines an interface for accessing mod resources.
    /// </summary>
    public interface ICrucibleResourceProvider
    {
        /// <summary>
        /// Reads all text from a text resource.
        /// </summary>
        /// /// <param name="resourceName">The name of the resource to read.</param>
        /// <returns>All text contained in the resource.</returns>
        string ReadAllText(string resourceName);

        /// <summary>
        /// Reads all bytes from a binary resource.
        /// </summary>
        /// <param name="resourceName">The resource to read.</param>
        /// <returns>The read bytes.</returns>
        byte[] ReadAllBytes(string resourceName);
    }
}
