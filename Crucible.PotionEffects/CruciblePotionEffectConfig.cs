// <copyright file="CruciblePotionEffectConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.PotionEffects
{
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration for creating potion effects.
    /// </summary>
    public class CruciblePotionEffectConfig : CruciblePackageConfigSubjectNode<CruciblePotionEffect>
    {
        /// <summary>
        /// Gets or sets the ID for this potion effect.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this potion effect.
        /// </summary>
        public LocalizedString Name { get; set; }

        /// <summary>
        /// Gets or sets the potion effect icon.
        /// </summary>
        public Texture2D Icon { get; set; }

        /// <inheritdoc/>
        protected override CruciblePotionEffect GetSubject()
        {
            var effectId = this.PackageMod.Namespace + "." + this.ID;
            return CruciblePotionEffect.GetPotionEffectByID(effectId) ?? CruciblePotionEffect.CreatePotionEffect(effectId);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CruciblePotionEffect subject)
        {
            if (this.Name != null)
            {
                subject.SetLocalizedName(this.Name);
            }

            if (this.Icon != null)
            {
                subject.EffectIconTexture = this.Icon;
            }
        }
    }
}
