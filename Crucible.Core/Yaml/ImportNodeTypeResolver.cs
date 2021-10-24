namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A type resolver determining that the type of !import tags are always the requested type.
    /// </summary>
    internal class ImportNodeTypeResolver : INodeTypeResolver
    {
        /// <inheritdoc/>
        public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
        {
            if (nodeEvent.Tag == "!import")
            {
                // Leave curent type alone.
                return true;
            }

            return false;
        }
    }
}
