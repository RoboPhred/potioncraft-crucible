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
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// A map object entity factory that can create potion effects on a recipe map.
    /// </summary>
    // TODO: Since we set settings seperately, we can support custom artwork.
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
        public GameObject SpawnEntity(GameObject recipeMap)
        {
            throw new System.NotImplementedException("Potion effect prefab capture not yet implemented.");
            var go = Object.Instantiate(potionEffectPrefab, new Vector3(0, 0, 0), Quaternion.identity, recipeMap.transform);
            var mapItem = go.GetComponent<PotionEffectMapItem>();
            mapItem.effect = this.PotionEffect.PotionEffect;
            Traverse.Create(mapItem).Field<PotionEffectSettings>("settings").Value = this.PotionEffect.PotionEffectSettings;
            mapItem.SetPositionOnMap(this.Position);
            mapItem.SetAngleOnMap(this.Angle);
            go.SetActive(true);
            return go;
        }
    }
}
