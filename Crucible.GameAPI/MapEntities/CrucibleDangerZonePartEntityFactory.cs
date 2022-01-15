// <copyright file="CrucibleDangerZonePartEntityFactory.cs" company="RoboPhredDev">
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
    using ObjectBased.RecipeMap.RecipeMapItem.DangerZoneMapItem;
    using UnityEngine;

    /// <summary>
    /// A factory for spawning danger zone (bone) entities.
    /// </summary>
    public class CrucibleDangerZonePartEntityFactory : ICrucibleMapEntityFactory
    {
        private static Dictionary<string, GameObject> capturedPrefabs = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleDangerZonePartEntityFactory"/> class.
        /// </summary>
        /// <param name="prefabType">The prefabricated danger zone variant to spawn.</param>
        /// <param name="position">The position to spawn the bone at.</param>
        /// <param name="angle">The angle to spawn the bone at.</param>
        public CrucibleDangerZonePartEntityFactory(string prefabType, Vector2 position, float angle)
        {
            this.PrefabType = prefabType;
            this.Position = position;
            this.Angle = angle;
        }

        /// <summary>
        /// Gets or sets the bone type to spawn.
        /// </summary>
        public string PrefabType { get; set; }

        /// <summary>
        /// Gets or sets the position to spawn the bone at.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the angle to spawn the bone at.
        /// </summary>
        public float Angle { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"DangerZonePart PrefabType={this.PrefabType} Position={this.Position} Angle={this.Angle}";
        }

        /// <inheritdoc/>
        public GameObject SpawnEntity(GameObject recipeMap)
        {
            // TODO: Danger zones are extremely simple.  We should build our own game objects intead of relying on copying others.
            // We will need to figure out how to grab the artwork for known bone types.
            // TODO: Support custom artwork and collision shapes.
            var dangerZone = recipeMap.GetComponentInChildren<DangerZoneMapItem>();

            if (dangerZone == null)
            {
                throw new Exception($"Could not find danger zone for map {recipeMap.name}.");
            }

            var parts = dangerZone.transform.Find("Parts");
            if (parts == null)
            {
                throw new Exception($"Could not find parts object for map {recipeMap.name}.");
            }

            return this.ProduceGameObject(parts);
        }

        private static GameObject GetPrefabOrFail(string prefabName)
        {
            if (capturedPrefabs == null)
            {
                PopulatePrefabs();
            }

            if (!capturedPrefabs.TryGetValue(prefabName, out var prefab))
            {
                throw new Exception($"Could not locate prefab \"{prefabName}\" for use as a danger zone.");
            }

            return prefab;
        }

        private static void PopulatePrefabs()
        {
            capturedPrefabs = new Dictionary<string, GameObject>();
            var parts = GameObject.FindObjectsOfType<DangerZonePart>();
            CapturePrefab("Fang1", parts);
            CapturePrefab("Fang2", parts);
            CapturePrefab("Bone1", parts);
            CapturePrefab("Bone2", parts);
            CapturePrefab("Skull1", parts);
        }

        private static void CapturePrefab(string name, IEnumerable<DangerZonePart> parts)
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
            prefab.name = $"Crucible Danger Zone Prefab {name}";
            go.SetActive(active);

            capturedPrefabs.Add(name, prefab);
        }

        private GameObject ProduceGameObject(Transform parent)
        {
            if (!string.IsNullOrEmpty(this.PrefabType))
            {
                var prefab = GetPrefabOrFail(this.PrefabType);
                var go = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                go.transform.localPosition = this.Position;
                go.transform.localRotation = Quaternion.Euler(0, 0, this.Angle);
                go.name = $"Crucible DangerZonePart";
                go.SetActive(true);
                return go;
            }

            throw new NotImplementedException("Custom danger zones are not yet supported.");
        }
    }
}
