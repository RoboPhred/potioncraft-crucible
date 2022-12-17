// <copyright file="CrucibleNPCConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines the configuration for an npc.
    /// </summary>
    /// <typeparam name="T">The type of npc template to populate from this config.</typeparam>
    public abstract class CrucibleNPCConfig<T> : CruciblePackageConfigSubjectNode<T>
        where T : CrucibleNpcTemplate
    {
        /// <summary>
        /// Gets or sets the ID of this ingredient.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the appearance configuration for this npc.
        /// </summary>
        public CrucibleNpcAppearanceConfig Appearance { get; set; }

        /// <summary>
        /// Gets or sets the list of quests for this NPC.
        /// </summary>
        public OneOrMany<CrucibleNPCQuestConfig> Quests { get; set; } = new OneOrMany<CrucibleNPCQuestConfig>();

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(T subject)
        {
            this.Appearance?.ApplyAppearance(subject);
        }
    }
}
