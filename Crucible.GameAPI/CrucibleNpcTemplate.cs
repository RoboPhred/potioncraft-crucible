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
        /// Gets or sets the left eye sprite for this NPC.
        /// </summary>
        /// <remarks>
        /// If this NPC's eyes are randomized, this will set every possible left eye to the given value.
        /// </remarks>
        public Sprite LeftEyeSprite
        {
            get
            {
                var eyes = this.NpcTemplate.appearance.eyes.partsInGroup.FirstOrDefault();
                if (eyes == null)
                {
                    return null;
                }

                return eyes.part.left;
            }

            set
            {
                if (this.NpcTemplate.appearance.eyes.partsInGroup.Length == 0)
                {
                    var eyes = ScriptableObject.CreateInstance<Eyes>();
                    eyes.left = value;
                    eyes.right = value;
                    this.NpcTemplate.appearance.eyes.partsInGroup = new[]
                    {
                        new PartContainer<Eyes>
                        {
                            part = eyes,
                        },
                    };
                }

                foreach (var part in this.NpcTemplate.appearance.eyes.partsInGroup)
                {
                    // Clone the part so we do not modify other npcs that we may have copied data from
                    var eyes = ScriptableObject.CreateInstance<Eyes>();
                    eyes.left = value;
                    eyes.right = part.part.right;
                    part.part = eyes;
                }
            }
        }

        /// <summary>
        /// Gets or sets the left eye sprite for this NPC.
        /// </summary>
        /// <remarks>
        /// If this NPC's eyes are randomized, this will set every possible left eye to the given value.
        /// </remarks>
        public Sprite RightEyeSprite
        {
            get
            {
                var eyes = this.NpcTemplate.appearance.eyes.partsInGroup.FirstOrDefault();
                if (eyes == null)
                {
                    return null;
                }

                return eyes.part.right;
            }

            set
            {
                if (this.NpcTemplate.appearance.eyes.partsInGroup.Length == 0)
                {
                    var eyes = ScriptableObject.CreateInstance<Eyes>();
                    eyes.left = value;
                    eyes.right = value;
                    this.NpcTemplate.appearance.eyes.partsInGroup = new[]
                    {
                        new PartContainer<Eyes>
                        {
                            part = eyes,
                        },
                    };
                }

                foreach (var part in this.NpcTemplate.appearance.eyes.partsInGroup)
                {
                    // Clone the part so we do not modify other npcs that we may have copied data from
                    var eyes = ScriptableObject.CreateInstance<Eyes>();
                    eyes.left = part.part.left;
                    eyes.right = value;
                    part.part = eyes;
                }
            }
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
        /// Copies the apperance from an npc template to this one.
        /// </summary>
        /// <remarks>
        /// Appearance data is made up of multiple parts that themselves contain random chances.  This copy command
        /// copies all data, including the randomized data.  As such, the npc appearance might change whenever the NPC shows up.
        /// </remarks>
        /// <param name="template">The template to copy the appearance from.</param>
        public void CopyAppearanceFrom(CrucibleNpcTemplate template)
        {
            // There's lots of data encoded on the prefab :(
            var prefab = this.RequirePart<Prefab>();
            var templatePrefab = template.RequirePart<Prefab>();
            prefab.prefab = templatePrefab.prefab;
            prefab.clothesColorPalette1 = templatePrefab.clothesColorPalette1;
            prefab.clothesColorPalette2 = templatePrefab.clothesColorPalette2;
            prefab.clothesColorPalette3 = templatePrefab.clothesColorPalette3;
            prefab.clothesColorPalette4 = templatePrefab.clothesColorPalette4;
            prefab.hairColorPalette = templatePrefab.hairColorPalette;
            prefab.skinColorPalette = templatePrefab.skinColorPalette;

            this.NpcTemplate.appearance = new AppearanceContainer
            {
                skinColor = ClonePartContainerGroup(template.NpcTemplate.appearance.skinColor),
                behindBodyFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.behindBodyFeature2),
                behindBodyFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.behindBodyFeature1),
                handBackFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.handBackFeature2),
                handBackFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.handBackFeature1),
                bodyFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.bodyFeature2),
                bodyFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.bodyFeature1),
                handFrontFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.handFrontFeature2),
                handFrontFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.handFrontFeature1),
                skullShapeFeature3 = ClonePartContainerGroup(template.NpcTemplate.appearance.skullShapeFeature3),
                skullShapeFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.skullShapeFeature2),
                skullShapeFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.skullShapeFeature1),
                faceFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.faceFeature2),
                shortHairFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.shortHairFeature2),
                shortHairFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.shortHairFeature1),
                faceFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.faceFeature1),
                aboveHairFeature1 = ClonePartContainerGroup(template.NpcTemplate.appearance.aboveHairFeature1),
                hairColor = ClonePartContainerGroup(template.NpcTemplate.appearance.hairColor),
                clothesColor1 = ClonePartContainerGroup(template.NpcTemplate.appearance.clothesColor1),
                clothesColor2 = ClonePartContainerGroup(template.NpcTemplate.appearance.clothesColor2),
                clothesColor3 = ClonePartContainerGroup(template.NpcTemplate.appearance.clothesColor3),
                aboveHairFeature2 = ClonePartContainerGroup(template.NpcTemplate.appearance.aboveHairFeature2),
                body = ClonePartContainerGroup(template.NpcTemplate.appearance.body),
                clothesColor4 = ClonePartContainerGroup(template.NpcTemplate.appearance.clothesColor4),
                skullShape = ClonePartContainerGroup(template.NpcTemplate.appearance.skullShape),
                face = ClonePartContainerGroup(template.NpcTemplate.appearance.face),
                hat = ClonePartContainerGroup(template.NpcTemplate.appearance.hat),
                hairstyle = ClonePartContainerGroup(template.NpcTemplate.appearance.hairstyle),
                eyes = ClonePartContainerGroup(template.NpcTemplate.appearance.eyes),
                breastSize = ClonePartContainerGroup(template.NpcTemplate.appearance.breastSize),
            };
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

        private static PartContainerGroup<T> ClonePartContainerGroup<T>(PartContainerGroup<T> group)
        {
            return new PartContainerGroup<T>
            {
                groupName = group.groupName,
                groupChance = group.groupChance,
                partsInGroup = group.partsInGroup.Select(x => new PartContainer<T>()
                {
                    chanceBtwParts = x.chanceBtwParts,
                    part = x.part,
                }).ToArray(),
            };
        }
    }
}
