namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    using System;

    /// <summary>
    /// Represents an exception while fetching a resource.
    /// </summary>
    public class CrucibleResourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleResourceException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CrucibleResourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Gets or sets the name of the resource being fetched.
        /// </summary>
        public string ResourceName { get; set; }
    }
}