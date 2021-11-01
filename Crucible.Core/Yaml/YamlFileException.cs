// <copyright file="YamlFileException.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Foobar is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Core;

    /// <summary>
    /// Represents a yaml exception in a particular file.
    /// </summary>
    public class YamlFileException : YamlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlFileException"/> class.
        /// </summary>
        /// <param name="filePath">The path of the file that encountered the error.</param>
        /// <param name="message">The exception message.</param>
        public YamlFileException(string filePath, string message)
            : base(message)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlFileException"/> class.
        /// </summary>
        /// <param name="filePath">The path of the file that encountered the error.</param>
        /// <param name="start">The starting location of the error.</param>
        /// <param name="end">The ending location of the error.</param>
        /// <param name="message">The exception message.</param>
        public YamlFileException(string filePath, Mark start, Mark end, string message)
            : base(start, end, message)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlFileException"/> class.
        /// </summary>
        /// <param name="filePath">The path of the file that encountered the error.</param>
        /// <param name="start">The starting location of the error.</param>
        /// <param name="end">The ending location of the error.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public YamlFileException(string filePath, Mark start, Mark end, string message, Exception innerException)
            : base(start, end, message, innerException)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlFileException"/> class.
        /// </summary>
        /// <param name="filePath">The path of the file that encountered the error.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public YamlFileException(string filePath, string message, Exception inner)
            : base(message, inner)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets the file the yaml exception occurred in.
        /// </summary>
        public string FilePath { get; }
    }
}
