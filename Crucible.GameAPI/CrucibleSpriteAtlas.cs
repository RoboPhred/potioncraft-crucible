namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using TMPro;
    using UnityEngine;
    using UnityEngine.TextCore;

    /// <summary>
    /// A sprite atlas is a collection of sprites.  It is primarily used to render icons alongside text.
    /// </summary>
    public class CrucibleSpriteAtlas
    {
        private readonly Dictionary<string, SpriteAtlasItem> items = new();

        private TMP_SpriteAsset asset;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleSpriteAtlas"/> class.
        /// </summary>
        /// <param name="atlasName">The name of the atlas.</param>
        public CrucibleSpriteAtlas(string atlasName)
        {
            this.AtlasName = atlasName;
            this.AtlasHashCode = TMP_TextUtilities.GetSimpleHashCode(atlasName);
        }

        /// <summary>
        /// Gets the name of the atlas.
        /// </summary>
        public string AtlasName
        {
            get;
        }

        /// <summary>
        /// Gets the simple hash code for the atlas name.
        /// </summary>
        public int AtlasHashCode
        {
            get;
        }

        /// <summary>
        /// Gets the TextMeshPro asset for this sprite atlas.
        /// </summary>
        public TMP_SpriteAsset Asset
        {
            get
            {
                this.EnsureAsset();
                return this.asset;
            }
        }

        private static void ResetAssetCache()
        {
            var instance = Traverse.Create<MaterialReferenceManager>().Field<MaterialReferenceManager>("s_Instance");
            if (instance == null)
            {
                return;
            }

            Traverse.Create(instance).Field<Dictionary<int, Material>>("m_FontMaterialReferenceLookup").Value.Clear();
            Traverse.Create(instance).Field<Dictionary<int, TMP_FontAsset>>("m_FontAssetReferenceLookup").Value.Clear();
            Traverse.Create(instance).Field<Dictionary<int, TMP_SpriteAsset>>("m_SpriteAssetReferenceLookup").Value.Clear();
            Traverse.Create(instance).Field<Dictionary<int, TMP_ColorGradient>>("m_ColorGradientReferenceLookup").Value.Clear();
        }

        public void AddIcon(string iconName, Texture2D icon)
        {
            this.AddIcon(iconName, icon, 0, 0, 0);
        }

        public void AddIcon(string iconName, Texture2D icon, float xOffset, float yOffset, float scale)
        {
            if (this.items.ContainsKey(iconName))
            {
                throw new ArgumentException($"An icon with the name \"{iconName}\" already exists.");
            }

            this.items.Add(iconName, new SpriteAtlasItem
            {
                Texture = icon,
                XOffset = xOffset,
                YOffset = yOffset,
                Scale = scale,
            });

            this.InvalidateAsset();
        }

        public bool RemoveIcon(string iconName)
        {
            var result = this.items.Remove(iconName);
            this.InvalidateAsset();
            return result;
        }

        private void InvalidateAsset()
        {
            this.asset = null;
        }

        private void EnsureAsset()
        {
            if (this.asset != null)
            {
                return;
            }

            var asset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            asset.name = this.AtlasName;
            asset.spriteInfoList = new List<TMP_Sprite>();

            var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false);

            var pairs = this.items.ToArray();

            var rects = texture.PackTextures(pairs.Select(x => x.Value.Texture).ToArray(), 0, 4096, false);

            asset.spriteSheet = texture;
            ShaderUtilities.GetShaderPropertyIDs();
            var material = new Material(Shader.Find("TextMeshPro/Sprite"));
            material.SetTexture(ShaderUtilities.ID_MainTex, texture);
            material.hideFlags = HideFlags.HideInHierarchy;
            asset.material = material;

            var scaleW = (float)texture.width;
            var scaleH = (float)texture.height;

            for (var i = 0; i < pairs.Length; i++)
            {
                var rect = rects[i];
                var spriteName = pairs[i].Key;
                var item = pairs[i].Value;

                var pixelRect = new Rect(rect.x * scaleW, rect.y * scaleH, rect.width * scaleW, rect.height * scaleH);
                var sprite = new TMP_Sprite
                {
                    id = i,
                    name = spriteName,
                    hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteName),
                    x = pixelRect.x,
                    y = pixelRect.y,
                    width = pixelRect.width,
                    height = pixelRect.height,
                    xAdvance = pixelRect.width,
                    xOffset = item.XOffset,
                    yOffset = item.YOffset,
                    scale = item.Scale,
                };

                asset.spriteInfoList.Add(sprite);
            }

            // This is almost exactly like asset.UpgradeSpriteAsset, except that function neglects TMP_SpriteCharacter.glyphIndex,
            // breaking TexMeshPro's rendering of atlas sprites.
            Traverse.Create(asset).Field<string>("m_Version").Value = "1.1.0";
            for (int index = 0; index < asset.spriteInfoList.Count; ++index)
            {
                var spriteInfo = asset.spriteInfoList[index];
                var tmpSpriteGlyph = new TMP_SpriteGlyph
                {
                    index = (uint)index,
                    sprite = spriteInfo.sprite,
                    metrics = new GlyphMetrics(spriteInfo.width, spriteInfo.height, spriteInfo.xOffset, spriteInfo.yOffset, spriteInfo.xAdvance),
                    glyphRect = new GlyphRect((int)spriteInfo.x, (int)spriteInfo.y, (int)spriteInfo.width, (int)spriteInfo.height),
                    scale = 1f, // FIXME: Code we copied from hard codes this to 1.  Should we set it to scale?
                    atlasIndex = 0,
                };
                asset.spriteGlyphTable.Add(tmpSpriteGlyph);

                var tmpSpriteCharacter = new TMP_SpriteCharacter
                {
                    glyph = tmpSpriteGlyph,
                    glyphIndex = (uint)index,
                    unicode = 65534U,
                    name = spriteInfo.name,
                    scale = spriteInfo.scale,
                };
                asset.spriteCharacterTable.Add(tmpSpriteCharacter);
            }

            asset.UpdateLookupTables();

            this.asset = asset;

            ResetAssetCache();
        }

        private struct SpriteAtlasItem
        {
            public Texture2D Texture { get; set; }

            public float XOffset { get; set; }

            public float YOffset { get; set; }

            public float Scale { get; set; }
        }
    }
}
