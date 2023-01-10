// <copyright file="CruciblePackageMod.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.CruciblePackages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using RoboPhredDev.PotionCraft.Crucible.Resources;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Represents a Crucible configuration-derived mod.
    /// </summary>
    public sealed class CruciblePackageMod : ICrucibleResourceProvider
    {
        private static List<CruciblePackageConfigNode> loadingNodes;

        private ICrucibleResourceProvider resources;
        private CruciblePackageModConfig parsedData;

        private CruciblePackageMod(string id, ICrucibleResourceProvider resourceProvider)
        {
            this.ID = id;
            this.resources = resourceProvider;
        }

        /// <summary>
        /// Gets the name of this mod.
        /// </summary>
        public string Name => this.parsedData.Name ?? this.ID;

        /// <summary>
        /// Gets the author of this mod.
        /// </summary>
        public string Author => this.parsedData.Author;

        /// <summary>
        /// Gets the version of this mod.
        /// </summary>
        public string Version => this.parsedData.Version;

        /// <summary>
        /// Gets or the dependencies of this mod.
        /// </summary>
        public List<CruciblePackageDependencyConfig> Dependencies => this.parsedData.Dependencies;

        /// <summary>
        /// Gets the ID of this mod.
        /// </summary>
        /// <remarks>
        /// The mod ID is the same as the name of the directory that contains its assets.
        /// </remarks>
        public string ID { get; }

        /// <summary>
        /// Gets the BepInEx style GUID for this mod.
        /// </summary>
        public string GUID
        {
            get
            {
                return $"Crucible::${this.ID}";
            }
        }

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
                /* return this.ID.Replace(" ", "_").ToLowerInvariant(); */
                return this.ID;
            }
        }

        /// <summary>
        /// Gets the collection of configuration roots for this mod.
        /// </summary>
        public ICollection<CruciblePackageConfigRoot> Roots => this.parsedData.ParsedRoots;

        /// <summary>
        /// Loads the crucible package from the specified directory.
        /// </summary>
        /// <param name="directory">The package directory to load.</param>
        /// <returns>A <see cref="CruciblePackageMod"/> loaded from the directory.</returns>
        public static CruciblePackageMod LoadFromFolder(string directory)
        {
            loadingNodes = new List<CruciblePackageConfigNode>();
            try
            {
                var mod = new CruciblePackageMod(new DirectoryInfo(directory).Name, new DirectoryResourceProvider(directory));
                mod.parsedData = CrucibleResources.WithResourceProvider(mod, () => Deserializer.DeserializeFromResource<CruciblePackageModConfig>("package.yml"));
                loadingNodes.ForEach(x => x.SetParentMod(mod));
                return mod;
            }
            catch (Exception ex)
            {
                // TODO: Collect exceptions for display to the user, return mod anyway.
                throw new CruciblePackageModLoadException($"Failed to load crucible package from \"{directory}\": " + ex.Message, ex);
            }
            finally
            {
                loadingNodes = null;
            }
        }

        /// <summary>
        /// Loads the crucible package from the specified file.
        /// </summary>
        /// <param name="zipFilePath">The file path to the zip file containing the mod.</param>
        /// <returns>A <see cref="CruciblePackageMod"/> loaded from the file.</returns>
        public static CruciblePackageMod LoadFromZip(string zipFilePath)
        {
            loadingNodes = new List<CruciblePackageConfigNode>();
            try
            {
                var mod = new CruciblePackageMod(Path.GetFileNameWithoutExtension(zipFilePath), new ZipResourceProvider(zipFilePath));
                mod.parsedData = CrucibleResources.WithResourceProvider(mod, () => Deserializer.DeserializeFromResource<CruciblePackageModConfig>("package.yml"));
                loadingNodes.ForEach(x => x.SetParentMod(mod));
                return mod;
            }
            catch (Exception ex)
            {
                // TODO: Collect exceptions for display to the user, return mod anyway.
                throw new CruciblePackageModLoadException($"Failed to load crucible package from \"{zipFilePath}\": " + ex.Message, ex);
            }
            finally
            {
                loadingNodes = null;
            }
        }

        /// <inheritdoc/>
        public bool Exists(string resourceName)
        {
            return this.resources.Exists(resourceName);
        }

        /// <inheritdoc/>
        public string ReadAllText(string resourceName)
        {
            return this.resources.ReadAllText(resourceName);
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string resourceName)
        {
            return this.resources.ReadAllBytes(resourceName);
        }

        /// <summary>
        /// Ensures that this mod's dependencies have been met.
        /// </summary>
        public void EnsureDependenciesMet()
        {
            if (this.Dependencies != null)
            {
                foreach (var dep in this.Dependencies)
                {
                    dep.EnsureDependencyMet();
                }
            }
        }

        /// <summary>
        /// Apply this mod's configuration to the game.
        /// </summary>
        public void ApplyConfiguration()
        {
            // TODO: Collect exceptions for display to the user.
            CrucibleResources.WithResourceProvider(this, () => CrucibleLog.RunInLogScope(this.Namespace, () => this.parsedData.ParsedRoots.ForEach(x => x.ApplyConfiguration())));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.ID} ({this.Namespace})";
        }

        /// <summary>
        /// Inform the mod that a config node belonging to it has loaded.
        /// </summary>
        /// <param name="node">The node belonging to this mod that has loaded.</param>
        internal static void OnNodeLoaded(CruciblePackageConfigNode node)
        {
            if (loadingNodes == null)
            {
                throw new Exception("Cannot instantiate a " + nameof(CruciblePackageConfigNode) + " when no " + nameof(CruciblePackageMod) + " is being loaded.");
            }

            loadingNodes.Add(node);
        }
    }
}
