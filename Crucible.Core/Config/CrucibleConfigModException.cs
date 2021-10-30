namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;

    /// <summary>
    /// An exception thrown when a failure is encountered while loading a config mod.
    /// </summary>
    public class CrucibleConfigModException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleConfigModException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CrucibleConfigModException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleConfigModException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CrucibleConfigModException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets the path to the mod that encountered the exception.
        /// </summary>
        public string ModPath { get; set; }
    }
}
