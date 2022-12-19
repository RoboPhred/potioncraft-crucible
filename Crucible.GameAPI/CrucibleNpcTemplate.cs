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
    using global::PotionCraft.ObjectBased.Deliveries;
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
        /// Gets or sets the time of day that this npc spawns.
        /// </summary>
        public int DayTimeForSpawn
        {
            get => this.NpcTemplate.dayTimeForSpawn;
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Day time values must range from 0 to 100. Unable to set DayTimeForSpawn");
                }

                this.NpcTemplate.dayTimeForSpawn = value;
            }
        }

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
        /// Gets the closeness parts list for this NPC.
        /// </summary>
        public List<CrucibleQuest> ClosenessQuests => this.NpcTemplate.uniqueClosenessQuests.Select(q => new CrucibleQuest(q)).ToList();

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

        /// <summary>
        /// Clears the existing closeness quest list and ensures the list is populated with the correct number of entries.
        /// </summary>
        public void PrepareClosenessQuestsForNewQuests()
        {
            for (var i = 0; i < this.MaximumCloseness; i++)
            {
                if (i == this.NpcTemplate.uniqueClosenessQuests.Count)
                {
                    this.NpcTemplate.uniqueClosenessQuests.Add(null);
                    continue;
                }

                this.NpcTemplate.uniqueClosenessQuests[i] = null;
            }
        }

        /// <summary>
        /// Creates a new blank NPC template based on another template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="copyAppearanceFrom">The NPC template to copy the appearance from.</param>
        /// <returns>A new blank NPC template.</returns>
        protected static NpcTemplate CreateNpcTemplate(string name, CrucibleNpcTemplate copyAppearanceFrom)
        {
            var template = ScriptableObject.CreateInstance<NpcTemplate>();
            template.name = name;

            var copyFrom = copyAppearanceFrom.NpcTemplate;

            template.showDontComeAgainOption = copyFrom.showDontComeAgainOption;
            template.maxClosenessForChapters = copyFrom.maxClosenessForChapters.ToList();
            template.unlockAtChapter = copyFrom.unlockAtChapter;
            template.visualMood = copyFrom.visualMood;
            template.daysOfCooldown = copyFrom.daysOfCooldown;
            template.karmaForSpawn = copyFrom.karmaForSpawn;

            // TODO: How do prefabs differ?
            var prefab = ScriptableObject.CreateInstance<NpcPrefab>();
            var parentPrefab = copyFrom.baseParts.OfType<NpcPrefab>().FirstOrDefault();
            if (parentPrefab == null)
            {
                throw new Exception("Copy target had no prefab!");
            }

            // Used in getting potion reactions
            var gender = ScriptableObject.CreateInstance<Gender>();
            var parentGender = copyFrom.baseParts.OfType<Gender>().FirstOrDefault();
            if (parentGender == null)
            {
                throw new Exception("Copy target had no Gender part!");
            }

            gender.gender = parentGender.gender;

            var animationOnHaggle = ScriptableObject.CreateInstance<AnimationOnHaggle>();
            var parentAnimationOnHaggle = copyFrom.baseParts.OfType<AnimationOnHaggle>().FirstOrDefault();
            if (parentAnimationOnHaggle == null)
            {
                throw new Exception("Copy target had no AnimationOnHaggle part!");
            }

            animationOnHaggle.positionShift = parentAnimationOnHaggle.positionShift;
            animationOnHaggle.rotationShift = parentAnimationOnHaggle.rotationShift;
            animationOnHaggle.animationTime = parentAnimationOnHaggle.animationTime;
            animationOnHaggle.ease = parentAnimationOnHaggle.ease;

            var haggleStaticSettings = ScriptableObject.CreateInstance<HaggleStaticSettings>();
            var parentHaggleStaticSettings = copyFrom.baseParts.OfType<HaggleStaticSettings>().FirstOrDefault();
            if (parentHaggleStaticSettings == null)
            {
                throw new Exception("Copy target had no HaggleStaticSettings part!");
            }

            haggleStaticSettings.veryEasyTheme = parentHaggleStaticSettings.veryEasyTheme;
            haggleStaticSettings.easyTheme = parentHaggleStaticSettings.easyTheme;
            haggleStaticSettings.mediumTheme = parentHaggleStaticSettings.mediumTheme;
            haggleStaticSettings.hardTheme = parentHaggleStaticSettings.hardTheme;
            haggleStaticSettings.veryHardTheme = parentHaggleStaticSettings.veryHardTheme;

            var queueSpace = ScriptableObject.CreateInstance<QueueSpace>();
            var parentQueueSpace = copyFrom.baseParts.OfType<QueueSpace>().FirstOrDefault();
            if (parentQueueSpace == null)
            {
                throw new Exception("Copy target had no QueueSpace part!");
            }

            queueSpace.spawnAfterPause = parentQueueSpace.spawnAfterPause;
            queueSpace.pauseAfterSpawn = parentQueueSpace.pauseAfterSpawn;

            template.baseParts = new NonAppearancePart[] { prefab, animationOnHaggle, haggleStaticSettings, queueSpace, gender };

            // Copy closeness parts for each level of closeness
            foreach (var closenessPart in copyFrom.closenessParts)
            {
                var parentDialogueData = copyFrom.baseParts.OfType<DialogueData>().FirstOrDefault();
                if (parentDialogueData == null)
                {
                    throw new Exception("Copy target had no DialogueData part!");
                }

                var dialogueData = new CrucibleDialogueData(parentDialogueData).Clone();

                var traderSettings = ScriptableObject.CreateInstance<TraderSettings>();
                var parentTraderSettings = copyFrom.baseParts.OfType<TraderSettings>().FirstOrDefault();
                if (parentTraderSettings == null)
                {
                    continue;
                }

                traderSettings.canTrade = parentTraderSettings.canTrade;
                traderSettings.gold = parentTraderSettings.gold;
                traderSettings.deliveriesCategories = parentTraderSettings.deliveriesCategories.Select(CopyDeliveryCategory).ToList();

                template.closenessParts.Add(new NonAppearanceClosenessPartsList
                {
                    parts = new List<NonAppearancePart> { dialogueData.DialogueData, traderSettings },
                });
            }

            template.appearance = new AppearanceContainer();

            // Add trader to list of all templates
            NpcTemplate.allNpcTemplates.templates.Add(template);
            return template;
        }

        private static Category CopyDeliveryCategory(Category source)
        {
            return new Category
            {
                name = source.name,
                deliveries = source.deliveries.Select(CopyDelivery).ToList(),
            };
        }

        private static Delivery CopyDelivery(Delivery source)
        {
            return new Delivery
            {
                name = source.name,
                appearingChance = source.appearingChance,
                minCount = source.minCount,
                maxCount = source.maxCount,
                applyDiscounts = source.applyDiscounts,
                applyExtraCharge = source.applyExtraCharge,
                item = source.item,
            };
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
