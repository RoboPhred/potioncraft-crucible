// <copyright file="CrucibleIngredientStackItem.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Represents an ingredient stack item.
    /// </summary>
    /// <remarks>
    /// This is used to define a stack item and its ground decendants for <see cref="CrucibleIngredient"/>.
    /// </remarks>
    public class CrucibleIngredientStackItem
    {
        /// <summary>
        /// Gets or sets the sprite to use for this stack item.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Gets or sets the position of this item in the stack.
        /// </summary>
        public Vector2 PositionInStack { get; set; }

        /// <summary>
        /// Gets or sets the rotation of this item in the stack.
        /// </summary>
        public float AngleInStack { get; set; }

        /// <summary>
        /// Gets or sets the list of child stack items to create when this ingredient is ground.
        /// </summary>
        public List<CrucibleIngredientStackItem> GrindChildren { get; set; } = new();
    }
}
