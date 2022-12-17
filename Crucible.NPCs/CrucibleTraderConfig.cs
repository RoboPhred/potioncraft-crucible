// <copyright file="CrucibleTraderConfig.cs" company="RoboPhredDev">
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
    /// Defines the configuration for a trader.
    /// </summary>
    public class CrucibleTraderConfig : CrucibleNPCConfig<CrucibleTraderNpcTemplate>
    {
        /// <summary>
        /// Gets or sets the minimum karma at which this trader can appear.
        /// </summary>
        public int MinimumKarmaForSpawn { get; set; } = -100;

        /// <summary>
        /// Gets or sets the maximum karma at which this trader can appear.
        /// </summary>
        public int MaximumKarmaForSpawn { get; set; } = 100;

        /// <summary>
        /// Gets or sets the chapter this trader unlocks at.
        /// </summary>
        public int UnlockAtChapter { get; set; } = 1;

        /// <summary>
        /// Gets or sets the tags associated with this trader. This allows ingredient mods to easily target this trader without needing to know the template name.
        /// </summary>
        public OneOrMany<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the trader's gold.
        /// </summary>
        public int Gold { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the array of items this trader has access to sell by default.
        /// </summary>
        public OneOrMany<CrucibleInventoryItemSoldByNpcStaticConfig> Items { get; set; } = new OneOrMany<CrucibleInventoryItemSoldByNpcStaticConfig>();

        /// <summary>
        /// Gets or sets the day time for trader spawn. 0 is at the start of the day and 100 is at the end of the day.
        /// </summary>
        public int DayTimeForSpawn { get; set; } = 50;

        /// <summary>
        /// Gets or sets the visual mood of the faction ("Bad", "Normal", "Good").
        /// </summary>
        public string VisualMood { get; set; } = "Normal";

        /// <summary>
        /// Gets or sets the minimum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MinimumDaysOfCooldown { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum number of days of cooldown for this trader to spawn.
        /// </summary>
        public int MaximumDaysOfCooldown { get; set; } = 1;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.PackageMod.Namespace + "." + this.ID;
        }

        /// <inheritdoc/>
        protected override CrucibleTraderNpcTemplate GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;
            return CrucibleTraderNpcTemplate.GetTraderNpcTemplateById(id) ?? CrucibleTraderNpcTemplate.CreateTraderNpcTemplate(id);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CrucibleTraderNpcTemplate subject)
        {
            base.OnApplyConfiguration(subject);

            //subject.maxClosenessForChapters = this.maxClosenessForChapters.ToList();
            subject.UnlockAtChapter = this.UnlockAtChapter;
            subject.DayTimeForSpawn = this.DayTimeForSpawn;
            subject.VisualMood = this.VisualMood;
            subject.DaysOfCooldown = (this.MinimumDaysOfCooldown, this.MaximumDaysOfCooldown);
            subject.KarmaForSpawn = (this.MinimumKarmaForSpawn, this.MaximumKarmaForSpawn);







            //    CrucibleNpcTemplate copyFromTemplate = null;
            //if (!string.IsNullOrEmpty(copyAppearanceFrom))
            //{
            //    copyFromTemplate = GetNpcTemplateById(copyAppearanceFrom);
            //    if (copyFromTemplate == null || !copyFromTemplate.IsTrader)
            //    {
            //        throw new ArgumentException($"Could not find Trader NPC template with id \"{copyAppearanceFrom}\" to copy appearance from.", nameof(copyAppearanceFrom));
            //    }
            //}

            //var template = ScriptableObject.CreateInstance<NpcTemplate>();

            //template.name = name;

            //var copyFrom = copyFromTemplate.NpcTemplate;

            //template.closenessLevelUpIcon = copyFrom.closenessLevelUpIcon;
            //template.showDontComeAgainOption = copyFrom.showDontComeAgainOption;
            //template.maxClosenessForChapters = copyFrom.maxClosenessForChapters.ToList();
            //template.unlockAtChapter = copyFrom.unlockAtChapter;
            //template.dayTimeForSpawn = copyFrom.dayTimeForSpawn;
            //template.visualMood = copyFrom.visualMood;
            //template.daysOfCooldown = copyFrom.daysOfCooldown;
            //template.karmaForSpawn = copyFrom.karmaForSpawn;

            //// TODO: How do prefabs differ?
            //var prefab = ScriptableObject.CreateInstance<NpcPrefab>();
            //var parentPrefab = copyFrom.baseParts.OfType<NpcPrefab>().FirstOrDefault();
            //if (parentPrefab == null)
            //{
            //    throw new Exception("Copy target had no prefab!");
            //}

            //// Used in getting potion reactions
            //var gender = ScriptableObject.CreateInstance<Gender>();
            //var parentGender = copyFrom.baseParts.OfType<Gender>().FirstOrDefault();
            //if (parentGender == null)
            //{
            //    throw new Exception("Copy target had no Gender part!");
            //}

            //gender.gender = parentGender.gender;

            //var animationOnHaggle = ScriptableObject.CreateInstance<AnimationOnHaggle>();
            //var parentAnimationOnHaggle = copyFrom.baseParts.OfType<AnimationOnHaggle>().FirstOrDefault();
            //if (parentAnimationOnHaggle == null)
            //{
            //    throw new Exception("Copy target had no AnimationOnHaggle part!");
            //}

            //animationOnHaggle.positionShift = parentAnimationOnHaggle.positionShift;
            //animationOnHaggle.rotationShift = parentAnimationOnHaggle.rotationShift;
            //animationOnHaggle.animationTime = parentAnimationOnHaggle.animationTime;
            //animationOnHaggle.ease = parentAnimationOnHaggle.ease;

            //var haggleStaticSettings = ScriptableObject.CreateInstance<HaggleStaticSettings>();
            //var parentHaggleStaticSettings = copyFrom.baseParts.OfType<HaggleStaticSettings>().FirstOrDefault();
            //if (parentHaggleStaticSettings == null)
            //{
            //    throw new Exception("Copy target had no HaggleStaticSettings part!");
            //}

            //haggleStaticSettings.veryEasyTheme = parentHaggleStaticSettings.veryEasyTheme;
            //haggleStaticSettings.easyTheme = parentHaggleStaticSettings.easyTheme;
            //haggleStaticSettings.mediumTheme = parentHaggleStaticSettings.mediumTheme;
            //haggleStaticSettings.hardTheme = parentHaggleStaticSettings.hardTheme;
            //haggleStaticSettings.veryHardTheme = parentHaggleStaticSettings.veryHardTheme;

            //var queueSpace = ScriptableObject.CreateInstance<QueueSpace>();
            //var parentQueueSpace = copyFrom.baseParts.OfType<QueueSpace>().FirstOrDefault();
            //if (parentQueueSpace == null)
            //{
            //    throw new Exception("Copy target had no QueueSpace part!");
            //}

            //queueSpace.spawnAfterPause = parentQueueSpace.spawnAfterPause;
            //queueSpace.pauseAfterSpawn = parentQueueSpace.pauseAfterSpawn;

            //template.baseParts = new NonAppearancePart[] { prefab, animationOnHaggle, haggleStaticSettings, queueSpace, gender };

            //// Copy closeness parts for each level of closeness
            //foreach (var closenessPart in copyFrom.closenessParts)
            //{
            //    var parentDialogueData = copyFrom.baseParts.OfType<DialogueData>().FirstOrDefault();
            //    if (parentDialogueData == null)
            //    {
            //        throw new Exception("Copy target had no DialogueData part!");
            //    }

            //    var dialogueData = new CrucibleDialogueData(parentDialogueData).Clone();

            //    var traderSettings = ScriptableObject.CreateInstance<TraderSettings>();
            //    var parentTraderSettings = copyFrom.baseParts.OfType<TraderSettings>().FirstOrDefault();
            //    if (parentTraderSettings == null)
            //    {
            //        throw new Exception("Copy target had no TraderSettings part!");
            //    }

            //    traderSettings.canTrade = parentTraderSettings.canTrade;
            //    traderSettings.gold = parentTraderSettings.gold;
            //    traderSettings.deliveriesCategories = parentTraderSettings.deliveriesCategories.Select(CopyDeliveryCategory).ToList();

            //    template.closenessParts.Add(new NonAppearanceClosenessPartsList
            //    {
            //        parts = new List<NonAppearancePart> { dialogueData.DialogueData, traderSettings },
            //    });
            //}

            //template.appearance = new AppearanceContainer();
            //var crucibleTemplate = new CrucibleTraderNpcTemplate(template);
            //crucibleTemplate.Appearance.CopyFrom(copyFromTemplate);

            //NpcTemplate.allNpcTemplates.templates.Add(template);

            //return crucibleTemplate;

        }
    }
}


