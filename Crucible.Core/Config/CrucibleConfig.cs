namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// The root configuration element.
    /// This is the top level configuration object supplied by config mods.
    /// </summary>
    public class CrucibleConfig : IDeserializeExtraData, ICrucibleConfigRoot
    {
        private readonly List<ICrucibleConfigRoot> parsedRoots = new();

        /// <inheritdoc/>
        public void OnApplyConfiguration()
        {
            this.parsedRoots.ForEach(x => x.OnApplyConfiguration());
        }

        /// <inheritdoc/>
        public void OnDeserializeExtraData(ReplayParser parser)
        {
            this.parsedRoots.Clear();

            foreach (var rootType in CrucibleConfigElementRegistry.GetConfigRoots())
            {
                parser.Reset();
                this.parsedRoots.Add((ICrucibleConfigRoot)Deserializer.DeserializeFromParser(rootType, parser));
            }
        }
    }
}
