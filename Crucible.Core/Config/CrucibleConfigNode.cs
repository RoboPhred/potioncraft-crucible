namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using YamlDotNet.Core;

    /// <summary>
    /// A configuration node in a CrucibleConfig.
    /// </summary>
    public abstract class CrucibleConfigNode : IAfterYamlDeserialization
    {
        /// <summary>
        /// Gets the config mod this node is a part of.
        /// </summary>
        public CrucibleConfigMod Mod
        {
            get; private set;
        }

        /// <inheritdoc/>
        void IAfterYamlDeserialization.OnDeserializeCompleted(Mark start, Mark end)
        {
            CrucibleConfigMod.OnNodeLoaded(this);
        }

        /// <summary>
        /// Sets the crucible config mod that owns this node.
        /// </summary>
        /// <param name="configMod">The mod that owns this node.</param>
        internal void SetConfigMod(CrucibleConfigMod configMod)
        {
            this.Mod = configMod;
        }
    }
}
