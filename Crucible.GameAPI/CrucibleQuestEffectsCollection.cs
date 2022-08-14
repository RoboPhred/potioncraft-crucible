// <copyright file="CrucibleQuestEffectsCollection.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using global::PotionCraft.QuestSystem;
    using global::PotionCraft.ScriptableObjects;

    /// <summary>
    /// Wraps a <see cref="Quest"/> and provides access to get or modify its required effects.
    /// </summary>
    public sealed class CrucibleQuestEffectsCollection : ICollection<CruciblePotionEffect>
    {
        private readonly Quest quest;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleQuestEffectsCollection"/> class.
        /// </summary>
        /// <param name="quest">The quest to provide effects control over.</param>
        internal CrucibleQuestEffectsCollection(Quest quest)
        {
            this.quest = quest;
        }

        /// <inheritdoc/>
        public int Count => this.quest.desiredEffects.Length;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(CruciblePotionEffect item)
        {
            if (Array.IndexOf(this.quest.desiredEffects, item) >= 0)
            {
                return;
            }

            this.quest.desiredEffects = this.quest.desiredEffects.Concat(new[] { item.PotionEffect }).ToArray();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.quest.desiredEffects = new PotionEffect[0];
        }

        /// <inheritdoc/>
        public bool Contains(CruciblePotionEffect item)
        {
            return Array.IndexOf(this.quest.desiredEffects, item.PotionEffect) >= 0;
        }

        /// <inheritdoc/>
        public void CopyTo(CruciblePotionEffect[] array, int arrayIndex)
        {
            for (var i = 0; i < this.quest.desiredEffects.Length; i++)
            {
                array[i + arrayIndex] = new CruciblePotionEffect(this.quest.desiredEffects[i]);
            }
        }

        /// <inheritdoc/>
        public IEnumerator<CruciblePotionEffect> GetEnumerator()
        {
            return this.quest.desiredEffects.Select(x => new CruciblePotionEffect(x)).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(CruciblePotionEffect item)
        {
            var index = Array.FindIndex(this.quest.desiredEffects, x => x == item.PotionEffect);
            if (index < 0)
            {
                return false;
            }

            this.quest.desiredEffects = this.quest.desiredEffects.Where((_, i) => i != index).ToArray();
            return true;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
