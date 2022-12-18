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
    using global::PotionCraft.DialogueSystem.Dialogue;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.Npc.Parts;
    using global::PotionCraft.Npc.Parts.Settings;
    using global::PotionCraft.QuestSystem;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="NpcTemplate"/>s.
    /// </summary>
    public class CrucibleNpcTemplate : IEquatable<CrucibleNpcTemplate>
    {
        private static readonly Dictionary<string, HashSet<string>> NpcTemplateTagsById = new();

        static CrucibleNpcTemplate()
        {
            // TODO: Groundhog day templates no longer exist in the templates list. All Groundhog day traders in Potion Craft v0.5.0 do not use the new closeness system and appear to be non-funtional.
            var herbalistTags = new[] { CrucibleNpcTemplateTags.SellsHerbs, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsHerbalist };
            NpcTemplateTagsById.Add("Herbalist", new HashSet<string>(herbalistTags));

            var mushroomerTags = new[] { CrucibleNpcTemplateTags.SellsMushrooms, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsMushroomer };
            NpcTemplateTagsById.Add("Mushroomer", new HashSet<string>(mushroomerTags));

            var alchemistTags = new[] { CrucibleNpcTemplateTags.SellsAlchemyMachine, CrucibleNpcTemplateTags.IsAlchemist };
            NpcTemplateTagsById.Add("Alchemist", new HashSet<string>(alchemistTags));

            var dwarfTags = new[] { CrucibleNpcTemplateTags.SellsCrystals, CrucibleNpcTemplateTags.SellsInorganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsDwarfMiner };
            NpcTemplateTagsById.Add("Dwarf", new HashSet<string>(dwarfTags));

            var merchantTags = new[] { CrucibleNpcTemplateTags.SellsHerbs, CrucibleNpcTemplateTags.SellsMushrooms, CrucibleNpcTemplateTags.SellsOrganic, CrucibleNpcTemplateTags.SellsInorganic, CrucibleNpcTemplateTags.SellsIngredients, CrucibleNpcTemplateTags.IsTravelingMerchant };
            NpcTemplateTagsById.Add("WMerchant", new HashSet<string>(merchantTags));
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
        public bool IsTrader => this.NpcTemplate.closenessParts.Any(c => c.parts.Any(x => x is TraderSettings));

        /// <summary>
        /// Gets a value indicating whether this template is a customer.
        /// </summary>
        public bool IsCustomer => this.NpcTemplate.baseParts.Any(x => x is Quest);

        /// <summary>
        /// Gets the closeness parts list for this NPC.
        /// </summary>
        public List<NonAppearanceClosenessPartsList> ClosenessParts => this.NpcTemplate.closenessParts;

        /// <summary>
        /// Gets the maximum level of closeness for this NPC.
        /// </summary>
        public int MaximumCloseness => this.NpcTemplate.closenessParts.Count;

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
            var template = NpcTemplate.allNpcTemplates.templates.Find(x => x.name == name);
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
            return NpcTemplate.allNpcTemplates.templates.Select(x => new CrucibleNpcTemplate(x));
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
        public override string ToString()
        {
            return $"[CrucibleNpcTemplate {this.ID}]";
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
        /// Applies a given CrucibleDialogueNode to this npc for a given closeness level.
        /// </summary>
        /// <param name="closenessLevel">The closeness level the given dialogue should appear at.</param>
        /// <param name="startingDialogue">The dialogue which should appear at the given closeness level.</param>
        public void ApplyDialogueForClosenessLevel(int closenessLevel, CrucibleDialogueData.CrucibleDialogueNode startingDialogue)
        {
            if (this.ClosenessParts.Count <= closenessLevel)
            {
                throw new ArgumentException($"Given closenessLevel is too large. Maximum closeness for this NPC is: {this.ClosenessParts.Count}");
            }

            var closenessPart = this.ClosenessParts[closenessLevel];
            var dialogueData = CrucibleDialogueData.CreateDialogueData(startingDialogue);
            var dialogueIndex = closenessPart.parts.FindIndex(p => p is DialogueData);
            if (dialogueIndex == -1)
            {
                closenessPart.parts.Add(dialogueData.DialogueData);
                return;
            }

            closenessPart.parts[dialogueIndex] = dialogueData.DialogueData;
        }

        /// <summary>
        /// Gets the non appearance part of the specified type, or throw.
        /// </summary>
        /// <typeparam name="T">The type of the part to get.</typeparam>
        /// <returns>The requested part.</returns>
        protected T RequireBasePart<T>()
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
