// <copyright file="CrucibleMapEntitySpawnerExtensions.cs" company="RoboPhredDev">
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

#if ENABLE_POTION_BASE

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.MapEntities
{
    using UnityEngine;

    /// <summary>
    /// Extensions for adding entities to a <see cref="CrucibleMapEntitySpawner"/>.
    /// </summary>
    public static class CrucibleMapEntitySpawnerExtensions
    {
        /// <summary>
        /// Adds a potion effect entity to the spawn queue.
        /// </summary>
        /// <param name="spawner">The map entity spawner to add the effect to.</param>
        /// <param name="effect">The effect to add.</param>
        /// <param name="position">The position at which to spawn the effect.</param>
        /// <param name="angle">The angle at which to spawn the effect.</param>
        public static void AddPotionEffect(this CrucibleMapEntitySpawner spawner, CruciblePotionEffect effect, Vector2 position, float angle = 0)
        {
            spawner.AddEntityFactory(new CruciblePotionEffectEntityFactory(effect, position, angle));
        }
    }
}

#endif
