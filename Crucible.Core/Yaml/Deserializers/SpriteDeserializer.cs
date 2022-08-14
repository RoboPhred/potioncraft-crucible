// <copyright file="SpriteDeserializer.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Yaml.Deserializers
{
    using System;
    using RoboPhredDev.PotionCraft.Crucible.Resources;
    using UnityEngine;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Deserializes <see cref="Sprite"/> objects.
    /// </summary>
    [YamlDeserializer]
    public class SpriteDeserializer : INodeDeserializer
    {
        /// <inheritdoc/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (!typeof(Sprite).IsAssignableFrom(expectedType))
            {
                value = null;
                return false;
            }

            if (this.TryDeserializeFilename(reader, out value))
            {
                return true;
            }

            var data = (SpriteData)nestedObjectDeserializer(reader, typeof(SpriteData));
            var tex = data.Texture;

            if (tex == null)
            {
                throw new Exception("Sprite must specify a texture.");
            }

            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), data.Pivot ?? new Vector2(0.5f, 0.5f));
            sprite.name = tex.name;
            value = sprite;
            return true;
        }

        private bool TryDeserializeFilename(IParser reader, out object value)
        {
            if (!reader.TryConsume<Scalar>(out var scalar))
            {
                value = null;
                return false;
            }

            var resource = scalar.Value;

            var data = CrucibleResources.ReadAllBytes(resource);

            // Do not create mip levels for this texture, use it as-is.
            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            if (!tex.LoadImage(data))
            {
                throw new CrucibleResourceException($"Failed to load image from resource at \"{resource}\".");
            }

            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sprite.name = resource;
            value = sprite;
            return true;
        }

        internal class SpriteData
        {
            public Texture2D Texture { get; set; }

            public Vector2? Pivot { get; set; }
        }
    }
}
