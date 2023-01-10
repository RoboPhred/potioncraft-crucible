// <copyright file="ZipResourceProvider.cs" company="RoboPhredDev">
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
    using System.IO.Compression;
    using System.Linq;

    /// <summary>
    /// A resource provider that provides resources from a zip file.
    /// </summary>
    public class ZipResourceProvider : ICrucibleResourceProvider
    {
        private readonly ZipArchive zip;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipResourceProvider"/> class.
        /// </summary>
        /// <param name="zipFilePath">The path to the zip file to pull resources from.</param>
        public ZipResourceProvider(string zipFilePath)
        {
            this.zip = ZipFile.OpenRead(zipFilePath);
        }

        /// <inheritdoc/>
        public bool Exists(string resourceName)
        {
            return this.zip.Entries.Any(x => x.FullName == resourceName);
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string resourceName)
        {
            var entry = this.zip.Entries.FirstOrDefault(x => x.FullName == resourceName);
            if (entry == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in zip file.");
            }

            using var stream = new MemoryStream();
            using var entryStream = entry.Open();
            entryStream.CopyTo(stream);
            return stream.ToArray();
        }

        /// <inheritdoc/>
        public string ReadAllText(string resourceName)
        {
            var entry = this.zip.Entries.FirstOrDefault(x => x.FullName == resourceName);
            if (entry == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in zip file.");
            }

            using var entryStream = entry.Open();
            using var textReader = new StreamReader(entryStream);
            return textReader.ReadToEnd();
        }
    }
}
