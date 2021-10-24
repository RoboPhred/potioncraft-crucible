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
