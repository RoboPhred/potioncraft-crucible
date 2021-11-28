// <copyright file="CruciblePotionEffect.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="PotionEffect"/>s.
    /// </summary>
    public sealed class CruciblePotionEffect
    {
        // TODO: Load PotionEffectSettings for all the base potion effects.
        private static readonly SerializableDictionary<string, PotionEffectSettings> RegisteredEffects = new();

        private CruciblePotionEffect(PotionEffect potionEffect)
        {
            this.PotionEffect = potionEffect;
        }

        /// <summary>
        /// Gets the <see cref="PotionEffect"/> this api object is configuring.
        /// </summary>
        public PotionEffect PotionEffect
        {
            get;
        }

        // TODO: Wrap individual settings, do not expose the base game class.
        internal PotionEffectSettings PotionEffectSettings
        {
            get
            {
                if (RegisteredEffects.TryGetValue(this.PotionEffect.name, out var settings))
                {
                    return settings;
                }

                // TODO: Generate and return a blank saettings object.
                throw new Exception($"No settings object was found for potion base \"{this.PotionEffect.name}\".");
            }
        }

        /// <summary>
        /// Gets a potion effect by id.
        /// </summary>
        /// <param name="id">The id of the potion effect to get.</param>
        /// <returns>A <see cref="CruciblePotionEffect"/> object for manipulating the potion effect, or null if no potion effect exists with the given id.</returns>
        public static CruciblePotionEffect GetPotionEffect(string id)
        {
            var effect = PotionEffect.allPotionEffects.Find(x => x.name == id);
            if (effect == null)
            {
                return null;
            }

            return new CruciblePotionEffect(effect);
        }
    }
}

#endif
