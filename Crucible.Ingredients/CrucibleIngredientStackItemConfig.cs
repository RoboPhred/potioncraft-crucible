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
    using System.Collections.Generic;
    using System.Linq;
    using RoboPhredDev.PotionCraft.Crucible.Config;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;

    /// <summary>
    /// Represents configuration for creating a stack item.
    /// </summary>
    public class CrucibleIngredientStackItemConfig : CruciblePackageConfigNode
    {
        /// <summary>
        /// Gets or sets the sprite to use for this stack item.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Gets or sets the position of this stack item in the stack when being held.
        /// </summary>
        public Vector2 PositionInStack { get; set; }

        /// <summary>
        /// Gets or sets the angle (in degrees) of this stack item in the stack when being held.
        /// </summary>
        public float AngleInStack { get; set; }

        /// <summary>
        /// Gets or sets the list of points making up the collision polygon.
        /// </summary>
        public List<Vector2> Collision { get; set; }

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
                PositionInStack = this.PositionInStack,
                AngleInStack = this.AngleInStack,
                ColliderPolygon = this.Collision,
            };

            if (this.GrindsInto != null)
            {
                item.GrindChildren = this.GrindsInto.Select(x => x.ToStackItem()).ToList();
            }

            return item;
        }
    }
}
