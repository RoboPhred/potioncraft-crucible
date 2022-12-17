// <copyright file="CrucibleFactionConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.NPCs
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Configuration specifying data on a faction.
    /// </summary>
    public class CrucibleFactionConfig : CruciblePackageConfigNode
    {
        // TODO  factionClasses, spawnChanceOneShotNpc, randomSpawnOneShotNpc, oneShotNpc

        /// <summary>
        /// Gets or sets the spawn chance configuration for this faction.
        /// </summary>
        public List<CrucibleSpawnChanceConfig> SpawnChance { get; set; } = new List<CrucibleSpawnChanceConfig> { new CrucibleRangeSpawnChanceConfig() };

        /// <summary>
        /// Gets or sets the minimum chapter this faction can spawn at.
        /// </summary>
        public int MinChapter { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum chapter this faction can spawn at.
        /// </summary>
        public int MaxChapter { get; set; } = 10;

        /// <summary>
        /// Gets or sets the visual mood of the faction ("Bad", "Normal", "Good").
        /// </summary>
        public string VisualMood { get; set; } = "Normal";

        /// <summary>
        /// Gets or sets the spawn chance configuration for this faction.
        /// </summary>
        public List<CrucibleEffectChanceConfig> QuestEffectChances { get; set; } = new List<CrucibleEffectChanceConfig>();

        /// <summary>
        /// Applies the configuration to the given faction.
        /// </summary>
        /// <param name="faction">The faction to apply the configuration to.</param>
        //public void OnApplyConfiguration(CrucibleFaction faction)
        //{
        //}
    }
}
