// <copyright file="CruciblePotionEffectEntityConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.PotionBases.Entities
{
    using System;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.MapEntities;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;
    using YamlDotNet.Core;

    /// <summary>
    /// /// A configuration object defining a potion effect entity.
    /// </summary>
    public class CruciblePotionEffectEntityConfig : CrucibleMapEntityConfig, IAfterYamlDeserialization
    {
        /// <summary>
        /// Gets or sets the name of the potion effect to spawn.
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// Gets or sets the position to spawn the effect at.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the angle to spawn the effect at.
        /// </summary>
        public float Angle { get; set; }

        /// <inheritdoc/>
        public override void AddEntityToSpawner(string packageNamespace, CrucibleMapEntitySpawner spawner)
        {
            var effect = CruciblePotionEffect.GetPotionEffectByID(packageNamespace + "." + this.Effect) ?? CruciblePotionEffect.GetPotionEffectByID(this.Effect);
            if (effect == null)
            {
                throw new Exception($"Could not find potion effect with ID \"{this.Effect}\".");
            }

            spawner.AddPotionEffect(effect, this.Position, this.Angle);
        }

        /// <summary>
        /// Validates the configuration object.
        /// </summary>
        /// <param name="start">The start of the configuration object.</param>
        /// <param name="end">The end of the configuration object.</param>
        void IAfterYamlDeserialization.OnDeserializeCompleted(Mark start, Mark end)
        {
            if (string.IsNullOrWhiteSpace(this.Effect))
            {
                throw new Exception($"Potion effect entity at {start} must have specify an effect.");
            }
        }
    }
}
