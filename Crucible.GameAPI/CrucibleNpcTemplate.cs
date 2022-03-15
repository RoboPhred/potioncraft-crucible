// <copyright file="CrucibleNpcTemplate.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using System.Linq;
    using Npc.Parts;
    using Npc.Parts.Appearance;
    using Npc.Parts.Settings;
    using QuestSystem;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="NpcTemplate"/>s.
    /// </summary>
    public class CrucibleNpcTemplate : IEquatable<CrucibleNpcTemplate>
    {
        private static readonly Dictionary<string, HashSet<string>> NpcTemplateTagsById = new();

        static CrucibleNpcTemplate()
        {
            var groundhogDayTags = new[] { CrucibleNpcTemplateTags.IsGroundhogDayNpc };

            var herbalistTags = new[] { CrucibleNpcTemplateTags.SellsHerbs, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsHerbalist };
            NpcTemplateTagsById.Add("HerbalistNpc 1", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 2", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 3", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 4", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 5", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 6", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 7", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("HerbalistNpc 8", new HashSet<string>(herbalistTags));
            NpcTemplateTagsById.Add("Demo2GroundHogDayHerbalistNpc", new HashSet<string>(herbalistTags.Concat(groundhogDayTags)));

            var mushroomerTags = new[] { CrucibleNpcTemplateTags.SellsMushrooms, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsMushroomer };
            NpcTemplateTagsById.Add("MushroomerNpc 1", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 2", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 3", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 4", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 5", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 6", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("MushroomerNpc 7", new HashSet<string>(mushroomerTags));
            NpcTemplateTagsById.Add("Demo2GroundHogDayMushroomerNpc", new HashSet<string>(mushroomerTags.Concat(groundhogDayTags)));

            var alchemistTags = new[] { CrucibleNpcTemplateTags.SellsAlchemyMachine, CrucibleNpcTemplateTags.IsAlchemist };
            NpcTemplateTagsById.Add("AlchemistNpc 1", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("AlchemistNpc 2", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("AlchemistNpc 3", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("AlchemistNpc 4", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("AlchemistNpc 5", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("AlchemistNpc 6", new HashSet<string>(alchemistTags));
            NpcTemplateTagsById.Add("Playtest2GroundHogDayAlchemistNpc", new HashSet<string>(alchemistTags.Concat(groundhogDayTags)));

            var dwarfTags = new[] { CrucibleNpcTemplateTags.SellsCrystals, CrucibleNpcTemplateTags.SellsInorganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsDwarfMiner };
            NpcTemplateTagsById.Add("DwarfMinerNpc 1", new HashSet<string>(dwarfTags));
            NpcTemplateTagsById.Add("DwarfMinerNpc 2", new HashSet<string>(dwarfTags));
            NpcTemplateTagsById.Add("DwarfMinerNpc 3", new HashSet<string>(dwarfTags));
            NpcTemplateTagsById.Add("DwarfMinerNpc 4", new HashSet<string>(dwarfTags));
            NpcTemplateTagsById.Add("Playtest2GroundHogDayDwarfMinerNpc", new HashSet<string>(dwarfTags.Concat(groundhogDayTags)));

            var merchantTags = new[] { CrucibleNpcTemplateTags.SellsHerbs, CrucibleNpcTemplateTags.SellsMushrooms, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsInorganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsTravelingMerchant };
            NpcTemplateTagsById.Add("WanderingMerchantNpc 1", new HashSet<string>(merchantTags));
            NpcTemplateTagsById.Add("WanderingMerchantNpc 2", new HashSet<string>(merchantTags));
            NpcTemplateTagsById.Add("Demo2GroundHogDayWanderingMerchantNpc 1", new HashSet<string>(merchantTags.Concat(groundhogDayTags)));
            NpcTemplateTagsById.Add("Demo2GroundHogDayWanderingMerchantNpc 2", new HashSet<string>(merchantTags.Concat(groundhogDayTags)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleNpcTemplate"/> class.
        /// </summary>
        /// <param name="template">The game NpcTemplate to represent.</param>
        internal CrucibleNpcTemplate(NpcTemplate template)
        {
            this.NpcTemplate = template;
        }

        /// <summary>
        /// Gets the <see cref="NpcTemplate"/> represented by this api class.
        /// </summary>
        public NpcTemplate NpcTemplate { get; }

        /// <summary>
        /// Gets the ID of this npc template.
        /// </summary>
        public string ID
        {
            get
            {
                return this.NpcTemplate.name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this template is a trader.
        /// </summary>
        public bool IsTrader => this.NpcTemplate.baseParts.Any(x => x is TraderSettings);

        /// <summary>
        /// Gets a value indicating whether this template is a customer.
        /// </summary>
        public bool IsCustomer => this.NpcTemplate.baseParts.Any(x => x is Quest);

        /// <summary>
        /// Gets the appearance controller for this npc.
        /// </summary>
        public CrucibleNpcAppearance Appearance
        {
            get => new(this.NpcTemplate);
        }

        /// <summary>
        /// Gets the collection of child templates for this npc template.
        /// </summary>
        public CrucibleNpcTemplateChildren Children
        {
            get => new(this.NpcTemplate);
        }

        /// <summary>
        /// Gets the NPC Template by the given name.
        /// </summary>
        /// <param name="name">The name of the npc template to fetch.</param>
        /// <returns>A <see cref="CrucibleNpcTemplate"/> api object for manipulating the template.</returns>
        public static CrucibleNpcTemplate GetNpcTemplateById(string name)
        {
            var template = NpcTemplate.allNpcTemplates.Find(x => x.name == name);
            if (template == null)
            {
                return null;
            }

            return new CrucibleNpcTemplate(template);
        }

        /// <summary>
        /// Gets all npc templates known to the game.
        /// </summary>
        /// <returns>An enumerable of every npc template registered with the game.</returns>
        public static IEnumerable<CrucibleNpcTemplate> GetAllNpcTemplates()
        {
            return NpcTemplate.allNpcTemplates.Select(x => new CrucibleNpcTemplate(x));
        }

        /// <summary>
        /// Gets all npc template tags that have been registered with Crucible.
        /// </summary>
        /// <returns>An enumerable of every tag applied to any npc template.</returns>
        public static IEnumerable<string> GetAllNpcTemplateTags()
        {
            var allTags = from pair in NpcTemplateTagsById
                          from tag in pair.Value
                          select tag;
            return allTags.Distinct();
        }

        /// <inheritdoc/>
        public bool Equals(CrucibleNpcTemplate other)
        {
            return this.NpcTemplate == other.NpcTemplate;
        }

        /// <summary>
        /// Adds a tag to this npc template.
        /// </summary>
        /// <param name="tag">The tag to apply to this template.</param>
        public void AddTag(string tag)
        {
            if (!NpcTemplateTagsById.ContainsKey(this.ID))
            {
                NpcTemplateTagsById.Add(this.ID, new HashSet<string>());
            }

            NpcTemplateTagsById[this.ID].Add(tag);
        }

        /// <summary>
        /// Checks to see if this npc template has the given tag.
        /// </summary>
        /// <param name="tag">The tag to check for.</param>
        /// <returns>True if the npc template is tagged with the given tag, or False otherwise.</returns>
        public bool HasTag(string tag)
        {
            if (!NpcTemplateTagsById.TryGetValue(this.ID, out var tags))
            {
                return false;
            }

            return tags.Contains(tag);
        }

        /// <summary>
        /// Gets all tags associated with this npc template.
        /// </summary>
        /// <returns>The collection of tags associated with this npc template.</returns>
        public IReadOnlyCollection<string> GetTags()
        {
            if (!NpcTemplateTagsById.TryGetValue(this.ID, out var tags))
            {
                return new string[0];
            }

            return tags.ToArray();
        }

        /// <summary>
        /// If this NPC Template is a customer, gets the API object for manipulating its customer data.
        /// </summary>
        /// <returns>The customer npc template, if this NPC template represents a customer.</returns>
        public CrucibleCustomerNpcTemplate AsCustomer()
        {
            if (!this.IsCustomer)
            {
                throw new InvalidOperationException("This NPC template is not a customer.");
            }

            return new CrucibleCustomerNpcTemplate(this.NpcTemplate);
        }

        /// <summary>
        /// If this NPC Template is a trader, gets the API object for manipulating its trader data.
        /// </summary>
        /// <returns>The trader npc template, if this NPC template represents a trader.</returns>
        public CrucibleTraderNpcTemplate AsTrader()
        {
            if (!this.IsTrader)
            {
                throw new InvalidOperationException("This NPC template is not a trader.");
            }

            return new CrucibleTraderNpcTemplate(this.NpcTemplate);
        }

        /// <summary>
        /// Adds this npc template to the queue.
        /// </summary>
        public void AddNpcToQueue()
        {
            Managers.Npc.AddToQueueForSpawn(this.NpcTemplate);
            Managers.Npc.TryToSpawnNpc();
        }

        /// <summary>
        /// Gets the non appearance part of the specified type, or throw.
        /// </summary>
        /// <typeparam name="T">The type of the part to get.</typeparam>
        /// <returns>The requested part.</returns>
        protected T RequirePart<T>()
            where T : NonAppearancePart
        {
            var part = this.NpcTemplate.baseParts.OfType<T>().FirstOrDefault();
            if (part == null)
            {
                throw new InvalidOperationException($"NPC template {this.ID} does not have a {typeof(T).Name} part.");
            }

            return part;
        }
    }
}
