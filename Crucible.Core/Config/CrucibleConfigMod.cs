// <copyright file="CrucibleConfigMod.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using RoboPhredDev.PotionCraft.Crucible.Resources;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// The root configuration element.
    /// This is the top level configuration object supplied by config mods.
    /// </summary>
    public sealed class CrucibleConfigMod : DirectoryResourceProvider
    {
        private static List<CrucibleConfigNode> loadingNodes;

        private CrucibleConfigModRoot root;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleConfigMod"/> class.
        /// </summary>
        private CrucibleConfigMod(string directory)
            : base(directory)
        {
            this.ID = new DirectoryInfo(directory).Name;
        }

        /// <summary>
        /// Gets the name of this mod.
        /// </summary>
        public string Name => this.root.Name ?? this.ID;

        /// <summary>
        /// Gets the author of this mod.
        /// </summary>
        public string Author => this.root.Author;

        /// <summary>
        /// Gets the version of this mod.
        /// </summary>
        public string Version => this.root.Version;

        /// <summary>
        /// Gets the ID of this mod.
        /// </summary>
        /// <remarks>
        /// The mod ID is the same as the name of the directory that contains its assets.
        /// </remarks>
        public string ID { get; }

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
                return this.ID.Replace(" ", "_").ToLowerInvariant();
            }
        }

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
                var mod = new CrucibleConfigMod(modFolder);
                var packagePath = Path.Combine(modFolder, "package.yml");
                mod.root = CrucibleResources.WithResourceProvider(mod, () => Deserializer.Deserialize<CrucibleConfigModRoot>(packagePath));
                loadingNodes.ForEach(x => x.SetConfigMod(mod));
                return mod;
            }
            catch (Exception ex)
            {
                // TODO: Collect exceptions for display to the user, return mod anyway.
                throw new CrucibleConfigModException($"Failed to load crucible mod \"{Path.GetDirectoryName(modFolder)}\": " + ex.Message, ex);
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
            // TODO: Collect exceptions for display to the user.
            CrucibleResources.WithResourceProvider(this, () => CrucibleLog.RunInLogScope(this.Namespace, () => this.root.ParsedRoots.ForEach(x => x.ApplyConfiguration())));
        }

        /// <summary>
        /// Inform the mod that a config node belonging to it has loaded.
        /// </summary>
        /// <param name="node">The node belonging to this mod that has loaded.</param>
        internal static void OnNodeLoaded(CrucibleConfigNode node)
        {
            if (loadingNodes == null)
            {
                throw new Exception("Cannot instantiate a CrucibleConfigNode when no CrucibleConfigMod is loading.");
            }

            loadingNodes.Add(node);
        }

        private class CrucibleConfigModRoot : IDeserializeExtraData
        {
            public List<CrucibleConfigRoot> ParsedRoots { get; } = new();

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

            /// <inheritdoc/>
            void IDeserializeExtraData.OnDeserializeExtraData(ReplayParser parser)
            {
                this.ParsedRoots.Clear();

                foreach (var rootType in CrucibleConfigElementRegistry.GetConfigRoots())
                {
                    parser.Reset();
                    this.ParsedRoots.Add((CrucibleConfigRoot)Deserializer.DeserializeFromParser(rootType, parser));
                }
            }
        }
    }
}
