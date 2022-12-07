// <copyright file="OldIngredientIdConvert.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.BackwardsCompatibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides tools for converting old ingredient ids to new ingredient ids.
    /// </summary>
    public static class OldIngredientIdConvert
    {
        /// <summary>
        /// Gets dictionary containing old ingredient ids as keys with new ingredient ids as values.
        /// </summary>
        public static Dictionary<string, string> IngredientConvertDict { get; } = new Dictionary<string, string>
        {
            { "BloodyRoot", "Bloodthorn" },
            { "CliffFungus", "Mudshroom" },
            { "Crystal", "CloudCrystal" },
            { "DryadMushroom", "DryadsSaddle" },
            { "EarthCrystal", "EarthPyrite" },
            { "GreenMushroom", "StinkMushroom" },
            { "GreyChanterelle", "ShadowChanterelle" },
            { "IceDragonfruit", "Icefruit" },
            { "LavaRoot", "Lavaroot" },
            { "Leaf", "Lifeleaf" },
            { "LumpyBeet", "DreamBeet" },
            { "Marshrooms", "Marshroom" },
            { "RedMushroom", "MadMushroom" },
            { "Refruit", "HairyBanana" },
            { "Tangleweeds", "Tangleweed" },
            { "Thistle", "ThunderThistle" },
            { "Wierdshroom", "Weirdshroom" },
        };
    }
}
