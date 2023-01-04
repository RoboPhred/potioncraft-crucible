// <copyright file="CrucibleHagglingThemesConfig.cs" company="RoboPhredDev">
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

    /// <summary>
    /// A config entry for specifying a specific npc template to sell an item.
    /// </summary>
    public class CrucibleHagglingThemesConfig : CruciblePackageConfigNode
    {
        /// <summary>
        /// Gets or sets the very easy haggle theme name.
        /// </summary>
        public string VeryEasyTheme { get; set; }

        /// <summary>
        /// Gets or sets the easy haggle theme name.
        /// </summary>
        public string EasyTheme { get; set; }

        /// <summary>
        /// Gets or sets the medium haggle theme name.
        /// </summary>
        public string MediumTheme { get; set; }

        /// <summary>
        /// Gets or sets the hard haggle theme name.
        /// </summary>
        public string HardTheme { get; set; }

        /// <summary>
        /// Gets or sets the very hard haggle theme name.
        /// </summary>
        public string VeryHardTheme { get; set; }

        /// <summary>
        /// Apply the configuration to the subject.
        /// </summary>
        /// <param name="subject">The subject to apply configuration to.</param>
        public void ApplyConfiguration(CrucibleNpcTemplate subject)
        {
            subject.SetHagglingThemes(this.VeryEasyTheme, this.EasyTheme, this.MediumTheme, this.HardTheme, this.VeryHardTheme);
        }
    }
}
