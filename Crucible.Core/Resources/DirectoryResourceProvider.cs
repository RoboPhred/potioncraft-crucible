// <copyright file="DirectoryResourceProvider.cs" company="RoboPhredDev">
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
    /// A resource provider that loads resources from within a directory.
    /// </summary>
    public class DirectoryResourceProvider : ICrucibleResourceProvider
    {
        private readonly string rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryResourceProvider"/> class.
        /// </summary>
        /// <param name="directory">The directory to pull resources from.</param>
        public DirectoryResourceProvider(string directory)
        {
            this.rootPath = Path.GetFullPath(directory);
        }

        /// <inheritdoc/>
        public bool Exists(string resourcePath)
        {
            return File.Exists(Path.Combine(this.rootPath, resourcePath));
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string resource)
        {
            var resourcePath = this.GetResourcePath(resource);
            return File.ReadAllBytes(resourcePath);
        }

        /// <inheritdoc/>
        public string ReadAllText(string resource)
        {
            var resourcePath = this.GetResourcePath(resource);
            return File.ReadAllText(resourcePath);
        }

        private string GetResourcePath(string resource)
        {
            var resourcePath = Path.GetFullPath(Path.Combine(this.rootPath, resource));
            if (!resourcePath.StartsWith(this.rootPath))
            {
                throw new CrucibleResourceException($"The request resource \"{resource}\" is outside of the mod directory \"{this.rootPath}\".")
                {
                    ResourceName = resource,
                };
            }

            return resourcePath;
        }
    }
}
