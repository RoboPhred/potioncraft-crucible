namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System.Collections.Generic;
    using System.IO;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// The root configuration element.
    /// This is the top level configuration object supplied by config mods.
    /// </summary>
    public sealed class CrucibleConfigMod : IDeserializeExtraData
    {
        private static List<CrucibleConfigNode> loadingNodes;

        private readonly List<CrucibleConfigRoot> parsedRoots = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleConfigMod"/> class.
        /// </summary>
        private CrucibleConfigMod()
        {
        }

        /// <summary>
        /// Gets the file name for this mod.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the namespace for this mod.
        /// </summary>
        /// <remarks>
        /// The namespace should be used to prefix ids of objects created for this mod.
        /// This helps to ensure two mods using similar ids do not conflict with each other.
        /// </remarks>
        public string Namespace
        {
            get
            {
                return this.FileName.Replace(" ", "_").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets or sets the name of this mod.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the author of this mod.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the version of this mod.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Loads the config mod from the specified folder.
        /// </summary>
        /// <param name="modFolder">The folder of the mod to load.</param>
        /// <returns>The crucible mod loaded from the folder.</returns>
        public static CrucibleConfigMod Load(string modFolder)
        {
            loadingNodes = new List<CrucibleConfigNode>();
            try
            {
                var mod = Deserializer.Deserialize<CrucibleConfigMod>(Path.Combine(modFolder, "package.yml"));
                mod.FileName = Path.GetFileName(modFolder);
                loadingNodes.ForEach(x => x.SetConfigMod(mod));
                return mod;
            }
            finally
            {
                loadingNodes = null;
            }
        }

        /// <summary>
        /// Apply this mod's configuration to the game.
        /// </summary>
        public void ApplyConfiguration()
        {
            this.parsedRoots.ForEach(x => x.OnApplyConfiguration());
        }

        /// <inheritdoc/>
        void IDeserializeExtraData.OnDeserializeExtraData(ReplayParser parser)
        {
            this.parsedRoots.Clear();

            foreach (var rootType in CrucibleConfigElementRegistry.GetConfigRoots())
            {
                parser.Reset();
                this.parsedRoots.Add((CrucibleConfigRoot)Deserializer.DeserializeFromParser(rootType, parser));
            }
        }

        /// <summary>
        /// Inform the mod that a config node belonging to it has loaded.
        /// </summary>
        /// <param name="node">The node belonging to this mod that has loaded.</param>
        internal static void OnNodeLoaded(CrucibleConfigNode node)
        {
            if (loadingNodes == null)
            {
                throw new System.Exception("Cannot instantiate a CrucibleConfigNode when no CrucibleConfigMod is loading.");
            }

            loadingNodes.Add(node);
        }
    }
}
