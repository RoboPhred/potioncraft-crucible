// <copyright file="CrucibleInventoryItem.cs" company="RoboPhredDev">
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
    using System;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.ScriptableObjects;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="InventoryItem"/>s.
    /// </summary>
    public class CrucibleInventoryItem : IEquatable<CrucibleInventoryItem>, ICrucibleInventoryItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleInventoryItem"/> class.
        /// </summary>
        /// <param name="item">The inventory item to wrap.</param>
        internal CrucibleInventoryItem(InventoryItem item)
        {
            this.InventoryItem = item;
        }

        /// <summary>
        /// Gets the ID (internal name) of this inventory item.
        /// </summary>
        public string ID
        {
            get
            {
                return this.InventoryItem.name;
            }
        }

        /// <summary>
        /// Gets or sets the sprite to use for this item in the inventory.
        /// </summary>
        public Sprite InventoryIcon
        {
            get
            {
                return this.InventoryItem.inventoryIconObject;
            }

            set
            {
                this.InventoryItem.inventoryIconObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the base price of this item for buying or selling.
        /// </summary>
        /// <value>The price of the item.</value>
        public float Price
        {
            get
            {
                return Traverse.Create(this.InventoryItem).Field<float>("price").Value;
            }

            set
            {
                Traverse.Create(this.InventoryItem).Field<float>("price").Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the required closeness level for this item to appear in a trader's inventory.
        /// </summary>
        public int ClosenessRequirement { get; set; }

        /// <summary>
        /// Gets the game item being controlled by this api wrapper.
        /// </summary>
        internal InventoryItem InventoryItem { get; }

        /// <inheritdoc/>
        CrucibleInventoryItem ICrucibleInventoryItemProvider.GetInventoryItem()
        {
            return this;
        }

        /// <inheritdoc/>
        public bool Equals(CrucibleInventoryItem other)
        {
            return this.InventoryIcon == other.InventoryIcon;
        }

        /// <summary>
        /// Gives the item to the player.
        /// </summary>
        /// <param name="count">The number of items to give.</param>
        public void GiveToPlayer(int count)
        {
            Managers.Player.inventory.AddItem(this.InventoryItem, count, true, true);
        }
    }
}
