// <copyright file="CrucibleCustomerConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.NPCs
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines the configuration for a customer.
    /// </summary>
    public class CrucibleCustomerConfig : CruciblePackageConfigSubjectNode<CrucibleCustomerNpcTemplate>
    {
        /// <summary>
        /// Gets or sets the ID of this ingredient.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the NPC Template to copy the apperance from.
        /// This can be any NPC Template, including customers and traders.
        /// </summary>
        public string CopyAppearanceFrom { get; set; }

        /// <summary>
        /// Gets or sets the request this npc makes of the alchemist.
        /// </summary>
        public LocalizedString RequestText { get; set; }

        /// <summary>
        /// Gets or sets the collection of effect names that this npc wants to buy.
        /// </summary>
        public List<string> AcceptedEffects { get; set; }

        public Texture2D HeadBackground { get; set; }

        public Sprite BodyBackground { get; set; }

        public Sprite ArmRightBackground { get; set; }

        public Texture2D Face { get; set; }

        public Sprite EyeLeft { get; set; }
        public Sprite EyeRight { get; set; }

        public Texture2D HairFrontRight { get; set; }

        /// <summary>
        /// Gets or sets the collection of appearances that this npc will make.
        /// </summary>
        public OneOrMany<CrucibleNpcCalendarVisitConfig> Visits { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override CrucibleCustomerNpcTemplate GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;
            return CrucibleCustomerNpcTemplate.GetCustomerNpcTemplateById(id) ?? CrucibleCustomerNpcTemplate.CreateCustomerNpcTemplate(id);
        }

        /// <inheritdoc/>
        protected override async void OnApplyConfiguration(CrucibleCustomerNpcTemplate subject)
        {
            if (!string.IsNullOrEmpty(this.CopyAppearanceFrom))
            {
                var template = CrucibleNpcTemplate.GetNpcTemplateById(this.CopyAppearanceFrom);
                if (template == null)
                {
                    CrucibleLog.Log($"Could not apply \"copyAppearanceFrom\" for customer ID \"{this.ID}\" because no NPC template with an ID of \"{this.CopyAppearanceFrom}\" was found.");
                }
                else
                {
                    subject.Appearance.CopyFrom(template);
                }
            }

            if (this.RequestText != null)
            {
                subject.SetLocalizedRequestText(this.RequestText);
            }

            if (this.AcceptedEffects != null)
            {
                foreach (var effectId in this.AcceptedEffects)
                {
                    var effect = CruciblePotionEffect.GetPotionEffectByID(this.PackageMod.Namespace + "." + effectId) ?? CruciblePotionEffect.GetPotionEffectByID(effectId);
                    if (effect == null)
                    {
                        CrucibleLog.Log($"Could not add potion effect \"{effectId}\" to customer ID \"{this.ID}\" because no effect with an ID of \"{effectId}\" was found.");
                    }
                    else
                    {
                        subject.AcceptedEffects.Add(effect);
                    }
                }
            }

            if (this.HeadBackground != null)
            {
                // TODO: Use a more sensible pivot.  Should let the artist specify where the pivot is.
                subject.Appearance.HeadBackground = SpriteUtilities.FromTexture(this.HeadBackground, new Vector2(0.35f, .3f));
            }

            if (this.BodyBackground != null)
            {
                subject.Appearance.BodyBackground = this.BodyBackground;
            }

            if (this.ArmRightBackground != null)
            {
                subject.Appearance.ArmRightBackground = this.ArmRightBackground;
            }

            if (this.Face != null)
            {
                // TODO: Use a more sensible pivot.  Should let the artist specify where the pivot is.
                subject.Appearance.FaceContour = SpriteUtilities.FromTexture(this.Face, new Vector2(0.1f, .3f));
            }

            if (this.EyeLeft != null)
            {
                subject.Appearance.EyeLeft = this.EyeLeft;
            }

            if (this.EyeRight != null)
            {
                subject.Appearance.EyeRight = this.EyeRight;
            }

            if (this.HairFrontRight != null)
            {
                subject.Appearance.HairFrontRight = SpriteUtilities.FromTexture(this.HairFrontRight, new Vector2(0.4f, 0.1f));
            }

            if (this.Visits != null)
            {
                foreach (var visit in this.Visits)
                {
                    visit.AddToDay(subject);
                }
            }
        }
    }
}
