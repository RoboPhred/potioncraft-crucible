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

#if CRUCIBLE_BASES

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

        /// <summary>
        /// Adds a danger zone entity to the spawn queue.
        /// </summary>
        /// <param name="spawner">The map entity spawner to add the danger zone to.</param>
        /// <param name="prefabType">The prefab type to use for an existing danger zone entity.</param>
        /// <param name="position">The position to spawn the danger zone at.</param>
        /// <param name="angle">The angle to spawn the danger zone at.</param>
        public static void AddDangerZonePart(this CrucibleMapEntitySpawner spawner, string prefabType, Vector2 position, float angle = 0)
        {
            spawner.AddEntityFactory(new CrucibleDangerZonePartEntityFactory(prefabType, position, angle));
        }

        /// <summary>
        /// Adds a vortex entity to the spawn queue.
        /// </summary>
        /// <param name="spawner">The map entity spawner to add the vortex to.</param>
        /// <param name="prefabType">The prefab type to use for an existing vortex entity.</param>
        /// <param name="position">The position to spawn the vortex at.</param>
        public static void AddVortex(this CrucibleMapEntitySpawner spawner, string prefabType, Vector2 position)
        {
            spawner.AddEntityFactory(new CrucibleVortexEntityFactory(prefabType, position));
        }
    }
}

#endif
