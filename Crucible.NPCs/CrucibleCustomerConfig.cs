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
        /// Gets or sets the appearance configuration for this npc.
        /// </summary>
        public CrucibleNpcAppearanceConfig Appearance { get; set; }

        /// <summary>
        /// Gets or sets the request this npc makes of the alchemist.
        /// </summary>
        public LocalizedString RequestText { get; set; }

        /// <summary>
        /// Gets or sets the collection of effect names that this npc wants to buy.
        /// </summary>
        public List<string> AcceptedEffects { get; set; }

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
        protected override void OnApplyConfiguration(CrucibleCustomerNpcTemplate subject)
        {
            this.Appearance?.ApplyAppearance(subject);

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
