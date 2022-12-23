// <copyright file="CrucibleTraderNpcTemplate.cs" company="RoboPhredDev">
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
    using global::PotionCraft.Core.ValueContainers;
    using global::PotionCraft.DialogueSystem.Dialogue;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.ManagersSystem.Npc;
    using global::PotionCraft.Npc.MonoBehaviourScripts;
    using global::PotionCraft.Npc.Parts;
    using global::PotionCraft.Npc.Parts.Settings;
    using global::PotionCraft.ObjectBased.Deliveries;
    using global::PotionCraft.ObjectBased.Haggle;
    using global::PotionCraft.Settings;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Represents an NPC Template that contains trader data.
    /// </summary>
    public sealed class CrucibleTraderNpcTemplate : CrucibleNpcTemplate
    {
        private static readonly List<string> AddedTraders = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleTraderNpcTemplate"/> class.
        /// </summary>
        /// <param name="npcTemplate">The NPC Template to wrap.</param>
        internal CrucibleTraderNpcTemplate(NpcTemplate npcTemplate)
            : base(npcTemplate)
        {
            if (this.TraderSettings == null)
            {
                throw new ArgumentException("NPC Template does not contain a TraderSettings.", nameof(npcTemplate));
            }
        }

        /// <summary>
        /// Gets the list of TraderSettings for this template organized by closeness.
        /// </summary>
        public Dictionary<int, TraderSettings> TraderSettings => this.GetTraderSettings();

        /// <summary>
        /// Gets or sets the chapter at which this npc unlocks.
        /// </summary>
        public int UnlockAtChapter
        {
            get => this.NpcTemplate.unlockAtChapter;
            set
            {
                if (value < 1 || value > 10)
                {
                    throw new ArgumentException("Chapter values must range from 1 to 10. Unable to set UnlockAtChapter");
                }

                this.NpcTemplate.unlockAtChapter = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum and maximum karma range that this trader will spawn in.
        /// </summary>
        public (int, int) KarmaForSpawn
        {
            get => (this.NpcTemplate.karmaForSpawn.min, this.NpcTemplate.karmaForSpawn.max);
            set
            {
                if (value.Item1 < -100 || value.Item2 > 100)
                {
                    throw new ArgumentException("Karma values must range from -100 to 100. Unable to set KarmaForSpawn");
                }

                if (value.Item1 > value.Item2)
                {
                    throw new ArgumentException("Minimum karma to spawn must be less than maximum karma to spawn. Unable to set KarmaForSpawn");
                }

                this.NpcTemplate.karmaForSpawn = new MinMaxInt(value.Item1, value.Item2);
            }
        }

        /// <summary>
        /// Gets or sets the starting gold of the trader.
        /// </summary>
        public int Gold
        {
            get => this.GetTraderSettings().FirstOrDefault().Value?.gold ?? 1000;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Gold cannot be less than zero. Unable to set Gold for custom trader.");
                }

                this.GetTraderSettings().Values.ToList().ForEach(v => v.gold = value);
            }
        }

        /// <summary>
        /// Creates a new blank NPC template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="copyAppearanceFrom">The NPC template to copy the appearance from.</param>
        /// <returns>A new blank NPC template.</returns>
        public static CrucibleTraderNpcTemplate CreateTraderNpcTemplate(string name, string copyAppearanceFrom = null)
        {
            CrucibleNpcTemplate copyFromTemplate = null;
            if (!string.IsNullOrEmpty(copyAppearanceFrom))
            {
                copyFromTemplate = GetNpcTemplateById(copyAppearanceFrom);
                if (copyFromTemplate == null || !copyFromTemplate.IsTrader)
                {
                    throw new ArgumentException($"Could not find Trader NPC template with id \"{copyAppearanceFrom}\" to copy appearance from.", nameof(copyAppearanceFrom));
                }
            }

            var baseTemplate = CreateNpcTemplate(name, copyFromTemplate);
            var template = new CrucibleTraderNpcTemplate(baseTemplate);
            template.NpcTemplate.closenessLevelUpIcon = copyFromTemplate.NpcTemplate.closenessLevelUpIcon;
            template.NpcTemplate.dayTimeForSpawn = copyFromTemplate.NpcTemplate.dayTimeForSpawn;

            template.Appearance.CopyFrom(copyFromTemplate);

            // Add trader to the karmic traders virtual queue so it can actually spawn like any other trader
            Settings<NpcManagerSettings>.Asset.mainTraders.templates.Add(template.NpcTemplate);
            AddTraderToPool(template);
            AddedTraders.Add(template.NpcTemplate.name);

            return template;
        }

        /// <summary>
        /// Adds all custom crucible traders to the trader bool if that trader is not already in the pool.
        /// </summary>
        public static void AddAllCustomTradersToPool()
        {
            var traderQueue = Managers.Npc.globalSettings.karmicTradersVirtualQueue;
            var traderPool = Traverse.Create(traderQueue).Field<List<string>>("temporaryPool").Value;

            AddedTraders.ForEach(trader =>
            {
                if (traderPool.Contains(trader))
                {
                    return;
                }

                traderPool.Add(trader);
            });
        }

        /// <summary>
        /// Adds the trader to the proper trader pool if this trader is not already in the pool.
        /// </summary>
        /// <param name="template">The trader to add to the pool.</param>
        public static void AddTraderToPool(CrucibleTraderNpcTemplate template)
        {
            var traderQueue = Managers.Npc.globalSettings.karmicTradersVirtualQueue;
            var traderPool = Traverse.Create(traderQueue).Field<List<string>>("temporaryPool").Value;
            if (traderPool.Contains(template.NpcTemplate.name))
            {
                return;
            }

            traderPool.Add(template.NpcTemplate.name);
        }

        /// <summary>
        /// Gets the NPC Template by the given name.
        /// </summary>
        /// <param name="id">The name of the npc template to fetch.</param>
        /// <returns>A <see cref="CrucibleNpcTemplate"/> api object for manipulating the template.</returns>
        public static CrucibleTraderNpcTemplate GetTraderNpcTemplateById(string id)
        {
            var template = NpcTemplate.allNpcTemplates.templates.Find(x => x.name == id);
            if (template == null)
            {
                return null;
            }

            var traderTemplate = new CrucibleTraderNpcTemplate(template);

            if (!traderTemplate.IsTrader)
            {
                return null;
            }

            return traderTemplate;
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
        public void AddTradeItem(CrucibleInventoryItem item, float chance = 1, int minCount = 1, int maxCount = 1)
        {
            var allSettings = this.TraderSettings;
            var validSettings = allSettings.Where(s => s.Key >= item.ClosenessRequirement)
                                           .Select(s => s.Value)
                                           .Where(s => s != null);

            foreach(var settings in validSettings)
            {
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
        }

        /// <summary>
        /// Clears the trader's inventory of all items.
        /// </summary>
        public void ClearTradeItems()
        {
            var allSettings = this.TraderSettings;
            var validSettings = allSettings.Select(s => s.Value)
                                           .Where(s => s != null);

            foreach (var settings in validSettings)
            {
                settings.deliveriesCategories.Clear();
            }
        }

        private Dictionary<int, TraderSettings> GetTraderSettings()
        {
            var settings = new Dictionary<int, TraderSettings>();
            for (var i = 0; i < this.NpcTemplate.closenessParts.Count; i++)
            {
                settings[i] = this.NpcTemplate.closenessParts[i].parts.OfType<TraderSettings>().FirstOrDefault();
            }

            if (settings.All(s => s.Value == null))
            {
                return null;
            }

            return settings;
        }
    }
}
