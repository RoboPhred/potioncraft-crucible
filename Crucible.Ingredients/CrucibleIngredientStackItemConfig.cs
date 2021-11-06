// <copyright file="CrucibleIngredientStackItemConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;

    /// <summary>
    /// Represents configuration for creating a stack item.
    /// </summary>
    public class CrucibleIngredientStackItemConfig : CrucibleConfigNode
    {
        /// <summary>
        /// Gets or sets the sprite to use for this stack item.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Gets or sets the stack items this item grinds into.
        /// </summary>
        public OneOrMany<CrucibleIngredientStackItemConfig> GrindsInto { get; set; }

        /// <summary>
        /// Produces a <see cref="CrucibleIngredientStackItem"/> from this config node.
        /// </summary>
        /// <returns>The stack item.</returns>
        public CrucibleIngredientStackItem ToStackItem()
        {
            var item = new CrucibleIngredientStackItem
            {
                Sprite = this.Sprite,
            };

            if (this.GrindsInto != null)
            {
                item.GrindChildren = this.GrindsInto.Select(x => x.ToStackItem()).ToList();
            }

            return item;
        }
    }
}
