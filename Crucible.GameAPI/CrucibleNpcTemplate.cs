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
    using System.Collections.Generic;
    using System.Linq;
    using Npc.Parts;
    using Npc.Parts.Settings;
    using ObjectBased.Deliveries;
    using QuestSystem;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="NpcTemplate"/>s.
    /// </summary>
    public sealed class CrucibleNpcTemplate
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

        private CrucibleNpcTemplate(NpcTemplate template)
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
        public bool IsTrader => this.GetAllParts().Any(x => x is TraderSettings);

        /// <summary>
        /// Gets a value indicating whether this template is a customer.
        /// </summary>
        public bool IsCustomer => this.GetAllParts().Any(x => x is Quest);

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
        /// Gets all npc templates with the given tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>An enumerable of all npc templates that match the given tag.</returns>
        public static IEnumerable<CrucibleNpcTemplate> GetNpcTemplatesByTag(string tag)
        {
            var templatesByTag = from pair in NpcTemplateTagsById
                                 where pair.Value.Contains(tag)
                                 let template = GetNpcTemplateById(pair.Key)
                                 where template != null
                                 select template;
            return templatesByTag;
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
        /// If this npc is a trader, adds an item to this template's trader inventory.
        /// </summary>
        /// <remarks>
        /// The underlying <see cref="NpcTemplate"/> must have a <see cref="TraderSettings"/> part.
        /// </remarks>
        /// <param name="item">The inventory item to add to the trader's inventory.</param>
        /// <param name="chance">The chance of the trader having the item for any given appearance.</param>
        /// <param name="minCount">The minimum amount of the item to stock.</param>
        /// <param name="maxCount">The maximum amount of the item to stock.</param>
        public void AddTraderItem(CrucibleInventoryItem item, float chance = 1, int minCount = 1, int maxCount = 1)
        {
            var traderSettings = this.GetAllParts().OfType<TraderSettings>();
            bool found = false;

            // Typically, npc templates only have one TraderSettings in baseParts,
            // but to be safe, try adding it to every trader settings found.
            foreach (var settings in traderSettings)
            {
                found = true;
                var crucibleCategory = settings.deliveriesCategories.Find(x => x.name == "Crucible");
                if (crucibleCategory == null)
                {
                    crucibleCategory = new Category
                    {
                        name = "Crucible",
                        deliveries = new List<Delivery>(),
                    };
                    settings.deliveriesCategories.Add(crucibleCategory);
                }

                // We could probably precreate the delivery outside the loop and re-use it,
                // but the game does not reuse them, so let's play it safe.
                crucibleCategory.deliveries.Add(new Delivery
                {
                    item = item.InventoryItem,
                    appearingChance = chance,
                    minCount = minCount,
                    maxCount = maxCount,
                    applyDiscounts = true,
                    applyExtraCharge = true,
                });
            }

            // TODO: Allow adding TraderSettings to base if not exists.  This will be to allow creation of new blank npc templates.
            // We need to see if the game makes an npc a trader purely from the existence of TraderSettings, or if there is
            // more work we need to do to make an npc into a trader.  Probably needs dialog nodes for it.
            if (!found)
            {
                throw new IncompatibleNpcTemplateException("This NpcTemplate does not define a trader NPC.");
            }
        }

        private IEnumerable<NonAppearancePart> GetAllParts()
        {
            foreach (var part in this.NpcTemplate.baseParts)
            {
                yield return part;
            }

            foreach (var group in this.NpcTemplate.groupsOfContainers)
            {
                if (group.groupChance == 0)
                {
                    continue;
                }

                foreach (var x in group.partsInGroup)
                {
                    if (x.chanceBtwParts == 0)
                    {
                        continue;
                    }

                    yield return x.part;
                }
            }
        }
    }
}
