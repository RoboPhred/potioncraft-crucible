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

            value = null;
            return false;
        }

        private bool TryDeserializeFilename(IParser reader, out object value)
        {
            if (!reader.TryConsume<Scalar>(out var scalar))
            {
                value = null;
                return false;
            }

            var resource = scalar.Value;

            var data = CrucibleResources.CurrentResource.ReadAllBytes(resource);

            // Do not create mip levels for this texture, use it as-is.
            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            if (!tex.LoadImage(data))
            {
                throw new CrucibleResourceException($"Failed to load image from resource at \"{resource}\".");
            }

            value = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            return true;
        }
    }
}
