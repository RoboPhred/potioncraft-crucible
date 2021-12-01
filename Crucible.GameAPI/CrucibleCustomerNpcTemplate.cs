// <copyright file="CrucibleCustomerNpcTemplate.cs" company="RoboPhredDev">
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
    using System.Linq;
    using LocalizationSystem;
    using Npc.Parts;
    using QuestSystem;
    using UnityEngine;

    /// <summary>
    /// Represents an NPC Template that contains customer data.
    /// </summary>
    public sealed class CrucibleCustomerNpcTemplate : CrucibleNpcTemplate, IEquatable<CrucibleCustomerNpcTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleCustomerNpcTemplate"/> class.
        /// </summary>
        /// <param name="npcTemplate">The NPC template to wrap.</param>
        internal CrucibleCustomerNpcTemplate(NpcTemplate npcTemplate)
            : base(npcTemplate)
        {
            if (this.Quest == null)
            {
                throw new ArgumentException("NPC Template does not contain a Quest.", nameof(npcTemplate));
            }
        }

        /// <summary>
        /// Gets or sets the request text in the user's current language.
        /// </summary>
        public string RequestText
        {
            get
            {
                return new Key($"#quest_text_{this.Quest.name}").GetText();
            }

            set
            {
                CrucibleLocalization.SetLocalizationKey($"#quest_text_{this.Quest.name}", value);
            }
        }

        /// <summary>
        /// Gets a collection of potion effects that this customer accepts.
        /// </summary>
        public CrucibleQuestEffectsCollection AcceptedEffects
        {
            get
            {
                return new CrucibleQuestEffectsCollection(this.Quest);
            }
        }

        /// <summary>
        /// Gets the base game Quest for this customer.
        /// </summary>
        public Quest Quest => this.NpcTemplate.baseParts.OfType<Quest>().FirstOrDefault();

        /// <summary>
        /// Gets the NPC Template by the given name.
        /// </summary>
        /// <param name="id">The name of the npc template to fetch.</param>
        /// <returns>A <see cref="CrucibleNpcTemplate"/> api object for manipulating the template.</returns>
        public static CrucibleCustomerNpcTemplate GetCustomerNpcTemplateById(string id)
        {
            var template = NpcTemplate.allNpcTemplates.Find(x => x.name == id);
            if (template == null)
            {
                return null;
            }

            if (!template.baseParts.OfType<Quest>().Any())
            {
                return null;
            }

            return new CrucibleCustomerNpcTemplate(template);
        }

        /// <summary>
        /// Creates a new blank NPC template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="copyAppearanceFrom">The NPC template to copy the appearance from.</param>
        /// <returns>A new blank NPC template.</returns>
        public static CrucibleCustomerNpcTemplate CreateCustomerNpcTemplate(string name, string copyAppearanceFrom = null)
        {
            var copyFromTemplate = CrucibleNpcTemplate.GetNpcTemplateById(copyAppearanceFrom);
            if (copyFromTemplate == null)
            {
                throw new ArgumentException($"Could not find NPC template with id \"{copyAppearanceFrom}\" to copy appearance from.", nameof(copyAppearanceFrom));
            }

            var template = ScriptableObject.CreateInstance<NpcTemplate>();

            template.spawnChance = 1f;
            template.baseParts = new[]
            {
                new Quest
                {
                    karmaReward = 0,
                    desiredEffects = PotionEffect.allPotionEffects.ToArray(),
                },
            };
            template.name = name;
            template.groupsOfContainers = new PartContainerGroup<NonAppearancePart>[0];
            template.appearance = new AppearanceContainer();

            NpcTemplate.allNpcTemplates.Add(template);

            CrucibleLocalization.SetLocalizationKey($"quest_text_{name}", "I am a new quest!");

            var crucibleTemplate = new CrucibleCustomerNpcTemplate(template);

            if (!string.IsNullOrEmpty(copyAppearanceFrom))
            {
                crucibleTemplate.CopyAppearanceFrom(copyFromTemplate);
            }

            return crucibleTemplate;
        }

        /// <summary>
        /// Sets the localized request text of this customer.
        /// </summary>
        /// <param name="requestText">The localized request text to use for this customer.</param>
        public void SetLocalizedRequestText(LocalizedString requestText)
        {
            CrucibleLocalization.SetLocalizationKey($"quest_text_{this.Quest.name}", requestText);
        }

        /// <inheritdoc/>
        public bool Equals(CrucibleCustomerNpcTemplate other)
        {
            return this.NpcTemplate == other.NpcTemplate;
        }
    }
}
