// <copyright file="CrucibleInventoryItemTraderExtensionConfig.cs" company="RoboPhredDev">
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

    /// <summary>
    /// A configuration extention allowing the specification of trader npcs selling the extended inventory item.
    /// </summary>
    [CruciblePackageConfigExtension(typeof(ICrucibleInventoryItemProvider))]
    public class CrucibleInventoryItemTraderExtensionConfig : CruciblePackageConfigNode, ICruciblePackageConfigExtension<ICrucibleInventoryItemProvider>
    {
        /// <summary>
        /// Gets or sets the collection of configs denotating who sells this inventory item.
        /// </summary>
        public OneOrMany<CrucibleInventoryItemSoldByConfig> SoldBy { get; set; }

        /// <inheritdoc/>
        public void OnApplyConfiguration(ICrucibleInventoryItemProvider subject)
        {
            if (this.SoldBy != null)
            {
                var item = subject.GetInventoryItem();
                foreach (var soldBy in this.SoldBy)
                {
                    soldBy.OnApplyConfiguration(item);
                }
            }
        }
    }
}
