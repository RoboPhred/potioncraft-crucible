namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using HarmonyLib;
    using LocalizationSystem;
    using UnityEngine;

    /// <summary>
    /// Wraps a <see cref="InventoryItem"/> to provide an api for mod use.
    /// </summary>
    public abstract class CrucibleInventoryItem
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
        /// Gets or sets the name of this item in the user's current language.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return new Key(this.InventoryItem.name).GetText();
            }

            set
            {
                CrucibleLocalization.SetLocalizationKey(this.InventoryItem.name, value);
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
        /// Gets the game item being controlled by this api wrapper.
        /// </summary>
        internal InventoryItem InventoryItem { get; }

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
