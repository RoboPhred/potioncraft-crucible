// <copyright file="OneOrMany.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A configuration class that can load either one item, or an array of items.
    /// </summary>
    /// <typeparam name="T">The type of item to load.</typeparam>
    public class OneOrMany<T> : IList<T>
    {
        private readonly List<T> contents;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOrMany{T}"/> class.
        /// </summary>
        public OneOrMany()
        {
            this.contents = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOrMany{T}"/> class.
        /// </summary>
        /// <param name="source">The enumerable to copy items from.</param>
        public OneOrMany(IEnumerable<T> source)
        {
            this.contents = source.ToList();
        }

        /// <inheritdoc/>
        public int Count => this.contents.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public T this[int index] { get => this.contents[index]; set => this.contents[index] = value; }

        /// <inheritdoc/>
        public void Add(T item)
        {
            this.contents.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.contents.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return this.contents.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.contents.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return this.contents.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return this.contents.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            this.contents.Insert(index, item);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            return this.contents.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            this.contents.RemoveAt(index);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.contents.GetEnumerator();
        }
    }
}
