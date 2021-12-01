// <copyright file="CrucibleNpcTemplateChildren.cs" company="RoboPhredDev">
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
    using Npc.Parts;

    /// <summary>
    /// Provides simplified access to the children of a CrucibleNpcTemplate.
    /// </summary>
    public sealed class CrucibleNpcTemplateChildren : IList<CrucibleNpcTemplate>
    {
        private readonly NpcTemplate template;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleNpcTemplateChildren"/> class.
        /// </summary>
        /// <param name="template">The NPC template to manage the children of.</param>
        internal CrucibleNpcTemplateChildren(NpcTemplate template)
        {
            this.template = template;
        }

        /// <inheritdoc/>
        public int Count => this.GetChildTemplates().Count();

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public CrucibleNpcTemplate this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                try
                {
                    return new CrucibleNpcTemplate(this.GetChildTemplates().Skip(index).First());
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                var item = this[index];
                var indexInBase = Array.IndexOf(this.template.baseParts, item);
                this.template.baseParts[indexInBase] = value.NpcTemplate;
            }
        }

        /// <inheritdoc/>
        public void Add(CrucibleNpcTemplate item)
        {
            this.template.baseParts = this.template.baseParts.Concat(new[] { item.NpcTemplate }).ToArray();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.template.baseParts = this.template.baseParts.Where(x => x is not NpcTemplate).ToArray();
        }

        /// <inheritdoc/>
        public bool Contains(CrucibleNpcTemplate item)
        {
            return this.GetChildTemplates().Contains(item.NpcTemplate);
        }

        /// <inheritdoc/>
        public void CopyTo(CrucibleNpcTemplate[] array, int arrayIndex)
        {
            for (var i = 0; i < this.Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        /// <inheritdoc/>
        public IEnumerator<CrucibleNpcTemplate> GetEnumerator()
        {
            foreach (var template in this.GetChildTemplates())
            {
                yield return new CrucibleNpcTemplate(template);
            }
        }

        /// <inheritdoc/>
        public int IndexOf(CrucibleNpcTemplate item)
        {
            var items = this.GetChildTemplates().ToList();
            return items.FindIndex(x => x == item.NpcTemplate);
        }

        /// <inheritdoc/>
        public void Insert(int index, CrucibleNpcTemplate item)
        {
            var itemAtIndex = this[index];
            var indexInBase = Array.IndexOf(this.template.baseParts, itemAtIndex);
            this.template.baseParts = this.template.baseParts.Take(indexInBase).Concat(new[] { item.NpcTemplate }).Concat(this.template.baseParts.Skip(indexInBase)).ToArray();
        }

        /// <inheritdoc/>
        public bool Remove(CrucibleNpcTemplate item)
        {
            var index = Array.FindIndex(this.template.baseParts, x => x is NpcTemplate npcTemplate && npcTemplate == item.NpcTemplate);
            if (index == -1)
            {
                return false;
            }

            this.template.baseParts = this.template.baseParts.Take(index).Concat(this.template.baseParts.Skip(index + 1)).ToArray();
            return true;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            NpcTemplate templateAtIndex;
            try
            {
                templateAtIndex = this.GetChildTemplates().Skip(index).First();
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var indexInBase = Array.IndexOf(this.template.baseParts, templateAtIndex);
            if (indexInBase == -1)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            this.template.baseParts = this.template.baseParts.Take(indexInBase).Concat(this.template.baseParts.Skip(indexInBase + 1)).ToArray();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<NpcTemplate> GetChildTemplates()
        {
            return this.template.baseParts.OfType<NpcTemplate>();
        }
    }
}
