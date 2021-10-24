namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;

    /// <summary>
    /// An attribute marking a class as being a configuration extension.
    /// Configuration extension classes will be parsed alongside the root configuration nodes that create instances
    /// of the given subject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CrucibleConfigExtensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleConfigExtensionAttribute"/> class.
        /// </summary>
        /// <param name="subject">The subject this configuration class will apply to.</param>
        public CrucibleConfigExtensionAttribute(Type subject)
        {
            this.SubjectType = subject;
        }

        /// <summary>
        /// Gets the subject this configuration extension applies to.
        /// </summary>
        public Type SubjectType { get; }
    }
}
