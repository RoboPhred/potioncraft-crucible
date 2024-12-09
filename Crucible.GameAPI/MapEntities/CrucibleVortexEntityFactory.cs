// <copyright file="CrucibleVortexEntityFactory.cs" company="RoboPhredDev">
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
    using System.Linq;
    using global::PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.VortexMapItem;
    using UnityEngine;

    /// <summary>
    /// A factory for spawning vortex entities.
    /// </summary>
    public class CrucibleVortexEntityFactory : ICrucibleMapEntityFactory
    {
        private static Dictionary<string, GameObject> capturedPrefabs = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleVortexEntityFactory"/> class.
        /// </summary>
        /// <param name="prefabType">The prefabricated danger zone variant to spawn.</param>
        /// <param name="position">The position to spawn the bone at.</param>
        public CrucibleVortexEntityFactory(string prefabType, Vector2 position)
        {
            this.PrefabType = prefabType;
            this.Position = position;
        }

        /// <summary>
        /// Gets or sets the vortex type to spawn.
        /// </summary>
        public string PrefabType { get; set; }

        /// <summary>
        /// Gets or sets the position to spawn the vortex at.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Vortex PrefabType={this.PrefabType} Position={this.Position}";
        }

        /// <inheritdoc/>
        public GameObject SpawnEntity(GameObject recipeMap)
        {
            if (!string.IsNullOrEmpty(this.PrefabType))
            {
                var prefab = GetPrefabOrFail(this.PrefabType);
                var go = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, recipeMap.transform);
                go.transform.localPosition = this.Position;
                go.transform.localRotation = Quaternion.identity;
                go.name = $"Crucible DangerZonePart";
                go.SetActive(true);
                return go;
            }

            throw new NotImplementedException("Custom vortexes are not yet supported.");
        }

        private static GameObject GetPrefabOrFail(string prefabName)
        {
            if (capturedPrefabs == null)
            {
                PopulatePrefabs();
            }

            if (!capturedPrefabs.TryGetValue("Vortex" + prefabName, out var prefab))
            {
                throw new Exception($"Could not locate prefab \"{prefabName}\" for use as a vortex.");
            }

            return prefab;
        }

        private static void PopulatePrefabs()
        {
            capturedPrefabs = new Dictionary<string, GameObject>();
            var parts = GameObject.FindObjectsByType<VortexMapItem>(FindObjectsSortMode.None);
            CapturePrefab("VortexMedium", parts);
            CapturePrefab("VortexLarge", parts);
        }

        private static void CapturePrefab(string name, IEnumerable<VortexMapItem> parts)
        {
            var part = parts.FirstOrDefault(p => p.name == name);
            if (part == null)
            {
                return;
            }

            var go = part.gameObject;
            var active = go.activeSelf;
            go.SetActive(false);
            var prefab = UnityEngine.Object.Instantiate(go, Vector3.zero, Quaternion.identity, GameObjectUtilities.CruciblePrefabRoot.transform);
            prefab.name = $"Crucible Vortex Prefab {name}";
            go.SetActive(active);

            capturedPrefabs.Add(name, prefab);
        }
    }
}
