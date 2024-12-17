// <copyright file="CruciblePotionBasesPlugin.cs" company="RoboPhredDev">
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

#if CRUCIBLE_BASES

namespace RoboPhredDev.PotionCraft.Crucible.PotionBases
{
    using BepInEx;

    /// <summary>
    /// BepInEx plugin for Crucible Config mods.
    /// </summary>
    [BepInPlugin("net.RoboPhredDev.PotionCraft.Crucible.PotionBases", "Potion Base support for Crucible Modding Framework", "2.0.1.0")]
    [BepInDependency("net.RoboPhredDev.PotionCraft.Crucible")]
    public class CruciblePotionBasesPlugin : BaseUnityPlugin
    {
    }
}

#endif
