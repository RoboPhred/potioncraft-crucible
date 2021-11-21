// <copyright file="RelativeResourceProvider.cs" company="RoboPhredDev">
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
    using System.IO;

    /// <summary>
    /// A resource provider that reads from another resource provider at a relative path.
    /// </summary>
    public class RelativeResourceProvider : ICrucibleResourceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeResourceProvider"/> class.
        /// </summary>
        /// <param name="parent">The parent resource provider to read from.</param>
        /// <param name="relativePath">The relative path to apply to the parent resource provider while reading.</param>
        public RelativeResourceProvider(ICrucibleResourceProvider parent, string relativePath)
        {
            this.Parent = parent;
            this.RelativePath = relativePath;
        }

        /// <summary>
        /// Gets the parent this relative resource provider reads from.
        /// </summary>
        public ICrucibleResourceProvider Parent { get; }

        /// <summary>
        /// Gets the relative path this resource provider reads from.
        /// </summary>
        public string RelativePath { get; }

        /// <inheritdoc/>
        public bool Exists(string resourceName)
        {
            return this.Parent.Exists(Path.Combine(this.RelativePath, resourceName));
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string resourceName)
        {
            resourceName = Path.Combine(this.RelativePath, resourceName);
            return this.Parent.ReadAllBytes(resourceName);
        }

        /// <inheritdoc/>
        public string ReadAllText(string resourceName)
        {
            resourceName = Path.Combine(this.RelativePath, resourceName);
            return this.Parent.ReadAllText(resourceName);
        }
    }
}
