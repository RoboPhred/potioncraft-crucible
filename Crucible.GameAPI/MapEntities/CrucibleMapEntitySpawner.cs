// <copyright file="CrucibleMapEntitySpawner.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.MapEntities
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A class for spawning entities on potion base maps.
    /// </summary>
    /// <remarks>
    /// This class handles spawning entities on a potion base map, and ensures the game initializes the map properly.
    /// It is strongly recommended that this class be used over manually adding game objects as children to the recipe map.
    /// </remarks>
    public sealed class CrucibleMapEntitySpawner : IDisposable
    {
        private readonly List<ICrucibleMapEntityFactory> mapEntityFactories = new();
        private readonly GameObject mapGameObject;
        private bool clearMap;

        private bool isDisposed;

        private CrucibleMapEntitySpawner(GameObject recipeMapGameObject)
        {
            RecipeMapGameObjectUtilities.EnsureRecipeMapObject(recipeMapGameObject, nameof(recipeMapGameObject));

            this.mapGameObject = recipeMapGameObject;
        }

        /// <summary>
        /// Creates a spawner for the given potion base.
        /// </summary>
        /// <param name="potionBase">The potion base to spawn entities for.</param>
        /// <returns>An entity spawner used to spawn entities on the given potion base's recipe map.</returns>
        public static CrucibleMapEntitySpawner WithPotionBase(CruciblePotionBase potionBase)
        {
            if (potionBase == null)
            {
                throw new ArgumentNullException(nameof(potionBase));
            }

            return new CrucibleMapEntitySpawner(potionBase.MapGameObject);
        }

        /// <summary>
        /// Queues up the clearing of the recipe map, and clears all entities queued by this spawner.
        /// </summary>
        public void ClearMap()
        {
            this.EnsureNotDisposed();

            this.clearMap = true;
            this.mapEntityFactories.Clear();
        }

        /// <summary>
        /// Adds the given entity factory to the spawner.
        /// </summary>
        /// <param name="factory">The entity factory to queue.</param>
        public void AddEntityFactory(ICrucibleMapEntityFactory factory)
        {
            this.EnsureNotDisposed();

            this.mapEntityFactories.Add(factory);
        }

        /// <summary>
        /// Executes the entity spawner's queued up entity spawns.
        /// This can only be called once.
        /// </summary>
        public void SpawnEntities()
        {
            this.EnsureNotDisposed();

            try
            {
                RecipeMapGameObjectUtilities.ExecuteInRecipeMapScope(this.mapGameObject, () =>
                {
                    if (this.clearMap)
                    {
                        RecipeMapGameObjectUtilities.ClearMap(this.mapGameObject);
                    }

                    foreach (var factory in this.mapEntityFactories)
                    {
                        factory.SpawnEntity(this.mapGameObject);
                    }
                });

                RecipeMapGameObjectUtilities.Reinitialize(this.mapGameObject);
            }
            finally
            {
                this.isDisposed = true;
            }
        }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.SpawnEntities();
        }

        private void EnsureNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("This spawner has already spawned its entities, and cannot be reused.");
            }
        }
    }
}
