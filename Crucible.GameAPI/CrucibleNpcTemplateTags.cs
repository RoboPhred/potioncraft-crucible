// <copyright file="CrucibleNpcTemplateTags.cs" company="RoboPhredDev">
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
    /// <summary>
    /// A collection of common tags for use with npcs.
    /// </summary>
    /// <remarks>
    /// For mod compatibility reasons, it is strongly recommended to apply these tags to your own npc templates when appropriate.
    /// <para>
    /// These tags are applied to the base game npc templates by default.
    /// </para>
    /// </remarks>
    public static class CrucibleNpcTemplateTags
    {
        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, herbs.
        /// </summary>
        public const string SellsHerbs = "SellsHerbs";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, mushrooms.
        /// </summary>
        public const string SellsMushrooms = "SellsMushrooms";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, crystals.
        /// </summary>
        public const string SellsCrystals = "SellsCrystals";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, parts for the alchemy machine.
        /// </summary>
        public const string SellsAlchemyMachine = "SellsAlchemyMachine";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, organic ingredients.
        /// </summary>
        public const string SellsOrganic = "SellsOrganic";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, inorganic ingredients.
        /// </summary>
        public const string SellsInorganic = "SellsInorganic";

        /// <summary>
        /// A tag indicating that the npc template sells, or wants to sell, ingredients of any type.
        /// </summary>
        public const string SellsIngredients = "SellsIngredients";

        /// <summary>
        /// A tag indicating that the npc template is for the base game Herbalist NPC.
        /// </summary>
        public const string IsHerbalist = "IsHerbalist";

        /// <summary>
        /// A tag indicating that the npc template is for the base game Mushroomer NPC.
        /// </summary>
        public const string IsMushroomer = "IsMushroomer";

        /// <summary>
        /// A tag indicating that the npc template is for the base game Dwarven Miner NPC.
        /// </summary>
        public const string IsDwarfMiner = "IsDwarfMiner";

        /// <summary>
        /// A tag indicating that the npc template is for the base game Alchemist NPC.
        /// </summary>
        public const string IsAlchemist = "IsAlchemist";

        /// <summary>
        /// A tag indicating that the npc template is for the base game Traveling Merchant NPC.
        /// </summary>
        public const string IsTravelingMerchant = "IsTravelingMerchant";

        /// <summary>
        /// A tag indicating that the npc template appears during the never-ending groundhog day portion of the game.
        /// </summary>
        public const string IsGroundhogDayNpc = "IsGroundhogDayNpc";
    }
}
