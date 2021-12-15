// <copyright file="CruciblePotionEffectEntityFactory.cs" company="RoboPhredDev">
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
    using HarmonyLib;
    using ObjectBased.RecipeMap;
    using UnityEngine;

    /// <summary>
    /// A map object entity factory that can create potion effects on a recipe map.
    /// </summary>
    public sealed class CruciblePotionEffectEntityFactory : ICrucibleMapEntityFactory
    {
        private static GameObject potionEffectPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="CruciblePotionEffectEntityFactory"/> class.
        /// </summary>
        /// <param name="effect">The potion effect to spawn.</param>
        /// <param name="position">The location on the map to spawn the potion effect.</param>
        /// <param name="angle">The angle to spawn the potion effect at.</param>
        public CruciblePotionEffectEntityFactory(CruciblePotionEffect effect, Vector2 position, float angle = 0)
        {
            this.PotionEffect = effect;
            this.Position = position;
            this.Angle = angle;
        }

        /// <summary>
        /// Gets or sets the potion effect to spawn.
        /// </summary>
        public CruciblePotionEffect PotionEffect { get; set; }

        /// <summary>
        /// Gets or sets the angle of this potion effect.
        /// </summary>
        /// <remarks>
        /// If an angle is specified, brewing this effect may require using sun and moon salts to rotate the potion bottle.
        /// </remarks>
        public float Angle { get; set; }

        /// <summary>
        /// Gets or sets the position of this effect on the map.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"PotionEffect PotionEffect={this.PotionEffect} Position={this.Position} Angle={this.Angle}";
        }

        /// <inheritdoc/>
        public GameObject SpawnEntity(GameObject recipeMap)
        {
            RecipeMapGameObjectUtilities.EnsureRecipeMapObject(recipeMap);

            var settings = this.PotionEffect.PotionEffectSettings;
            if (settings == null)
            {
                throw new Exception($"Unable to resolve effect settings for PotionEffect \"{this.PotionEffect.ID}\".  This may be because it is a base game effect that was deleted by a mod.");
            }

            var prefab = GetPotionEffectPrefab();

            var go = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, recipeMap.transform);
            go.name = $"Crucible PotionEffect {this.PotionEffect.ID}";
            var mapItem = go.GetComponent<PotionEffectMapItem>();

            mapItem.effect = this.PotionEffect.PotionEffect;
            mapItem.Status = PotionEffectStatus.Unknown;

            Traverse.Create(mapItem).Field<PotionEffectSettings>("settings").Value = settings;

            mapItem.SetPositionOnMap(this.Position);
            mapItem.SetAngleOnMap(this.Angle);

            go.SetActive(true);

            return go;
        }

        private static GameObject GetPotionEffectPrefab()
        {
            if (potionEffectPrefab != null)
            {
                return potionEffectPrefab;
            }

            foreach (var mapState in MapLoader.loadedMaps)
            {
                var mapItem = mapState.transform.gameObject.GetComponentInChildren<PotionEffectMapItem>();
                if (mapItem == null)
                {
                    continue;
                }

                var go = mapItem.gameObject;
                var wasActive = go.activeSelf;

                // Clone as inactive so the behaviors do not try to link up with a map.
                go.SetActive(false);
                potionEffectPrefab = UnityEngine.Object.Instantiate(go, Vector3.zero, Quaternion.identity, GameObjectUtilities.DisabledRoot.transform);
                potionEffectPrefab.name = "Crucible RecipeMap Prefab";
                go.SetActive(wasActive);

                return potionEffectPrefab;
            }

            throw new Exception("Unable to find a PotionEffectMapItem on any loaded potion base.  At least one PotionEffectMapItem must exist to spawn additional effects.");
        }
    }
}
