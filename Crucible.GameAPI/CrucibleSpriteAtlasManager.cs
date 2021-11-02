// <copyright file="CrucibleSpriteAtlasManager.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// Manages custom sprite atlases added to PotionCraft.
    /// </summary>
    public static class CrucibleSpriteAtlasManager
    {
        private static readonly Dictionary<int, CrucibleSpriteAtlas> AtlasesByHashCode = new();
        private static bool isInitialized = false;

        /// <summary>
        /// Adds the sprite atlas to PotionCraft.
        /// </summary>
        /// <param name="atlas">The sprite atlas to add.</param>
        public static void AddAtlas(CrucibleSpriteAtlas atlas)
        {
            EnsureInitialized();
            AtlasesByHashCode[atlas.AtlasHashCode] = atlas;
        }

        private static void EnsureInitialized()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            TMPTextOnSpriteAssetRequestEvent.OnSpriteAssetRequest += (_, e) =>
            {
                if (AtlasesByHashCode.TryGetValue(e.AssetHashCode, out var atlas))
                {
                    e.SpriteAsset = atlas.Asset;
                }
            };
        }
    }
}
