namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Overrides the keys attributed to this class when resolved with duck typing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DuckTypeKeysAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuckTypeKeysAttribute"/> class.
        /// </summary>
        /// <param name="keys">The keys to present to the duck typing system.</param>
        public DuckTypeKeysAttribute(string[] keys)
        {
            this.Keys = keys.ToList();
        }

        /// <summary>
        /// Gets the keys to provide for this class to the duck typing system.
        /// </summary>
        public IReadOnlyList<string> Keys { get; }
    }
}
