// <copyright file="CrucibleModConfig.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// The root config node for a crucible mod.
    /// </summary>
    /// <remarks>
    /// This class should not be deserialized directly.  Instead, <see cref="CrucibleMod"/> should be used.
    /// </remarks>
    /// <seealso cref="CrucibleMod"/>
    public class CrucibleModConfig : IDeserializeExtraData
    {
        /// <summary>
        /// Gets the list of config roots parsed from this mod config.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the dependencies of this mod.
        /// </summary>
        public List<CrucibleDependencyConfig> Dependencies { get; set; } = new();

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
