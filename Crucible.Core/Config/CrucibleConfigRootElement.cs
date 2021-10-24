namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// The root configuration element.
    /// This is the top level configuration object supplied by config mods.
    /// </summary>
    public class CrucibleConfigRootElement : IDeserializeExtraData
    {
        /// <inheritdoc/>
        public void OnDeserializeExtraData(ReplayParser parser)
        {
            var rootTypes = CrucibleConfigElementRegistry.GetConfigRoots();
            foreach (var rootType in rootTypes)
            {
                parser.Reset();
                Deserializer.DeserializeFromParser(rootType, parser);
            }
        }
    }
}
