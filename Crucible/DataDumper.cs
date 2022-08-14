// <copyright file="DataDumper.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DialogueSystem.Dialogue;
    using DialogueSystem.Dialogue.Data;
    using LocalizationSystem;
    using Npc.Parts;
    using Npc.Parts.Settings;
    using QuestSystem;
    using UnityEngine;

    /// <summary>
    /// Utility class to export game data.
    /// </summary>
    internal static class DataDumper
    {
        public static void DumpCurrentIngredientPath()
        {
            var sb = new StringBuilder();
            sb.Append("[\n");
            foreach (var hint in Managers.RecipeMap.path.fixedPathHints)
            {
                foreach (var p in hint.evenlySpacedPointsFixedPhysics.points)
                {
                    sb.Append("  {\"x\": ").Append(p.x).Append(", \"y\": ").Append(p.y).Append("},\n");
                }
            }

            sb.Append("]\n");
            File.WriteAllText("ingredientPath.json", sb.ToString());
        }

        /// <summary>
        /// Dumps various potioncraft data to the console.
        /// </summary>
        public static void DumpData()
        {
            Debug.Log("\n\nIngredients:");
            foreach (var ingredient in Managers.Ingredient.ingredients)
            {
                DumpIngredient(ingredient);
            }

            Debug.Log("\n\nPlotter Code:");
            foreach (var ingredient in Managers.Ingredient.ingredients)
            {
                DumpIngredientCode(ingredient);
            }

            Debug.Log("\n\nDays:");
            for (var i = 0; i < Managers.Day.calendar.Length; i++)
            {
                var day = Managers.Day.calendar[i];
                Debug.Log($"Day {i}");
                DumpDay(day);
            }

            Debug.Log($"Groundhog Day");
            DumpDay(Managers.Day.settings.groundhogDay);

            Debug.Log("\n\nNPC Templates");
            foreach (var template in NpcTemplate.allNpcTemplates)
            {
                DumpNpcTemplate(template, 1);
            }
        }

        private static string Tabs(int depth)
        {
            return string.Concat(Enumerable.Repeat("  ", depth));
        }

        private static void DumpIngredient(Ingredient ingredient, int tabDepth = 1)
        {
            Debug.Log($"{Tabs(tabDepth)} Ingredient: {ingredient.name}");
            Debug.Log($"{Tabs(tabDepth + 1)} Price: {ingredient.GetPrice()}");
            Debug.Log($"{Tabs(tabDepth + 1)} Unground Length: {ingredient.path.Length * ingredient.path.grindedPathStartsFrom}");
            // Break this out into x and y individually to avoid rounding.
            var ungroundEndPath = ingredient.path.GetPoint(ingredient.path.grindedPathStartsFrom);
            Debug.Log($"{Tabs(tabDepth + 1)} Unground end path: ({ungroundEndPath.x}, {ungroundEndPath.y})");
            Debug.Log($"{Tabs(tabDepth + 1)} Unground percentage: {ingredient.path.grindedPathStartsFrom * 100}%");
            Debug.Log($"{Tabs(tabDepth + 1)} Ground Length: {ingredient.path.Length}");
            // Break this out into x and y individually to avoid rounding.
            var groundEndPath = ingredient.path.GetPoint(1);
            Debug.Log($"{Tabs(tabDepth + 1)} Ground end path: ({groundEndPath.x}, {groundEndPath.y})");
            Debug.Log($"{Tabs(tabDepth + 1)} SVG Path: {ingredient.path.path.Select(p => $"C {p.P1.x},{p.P1.y} {p.P2.x},{p.P2.y} {p.PLast.x},{p.PLast.y}").Aggregate((a, b) => $"{a} {b}")}");
        }

        private static void DumpIngredientCode(Ingredient ingredient)
        {
            Debug.Log("{");
            Debug.Log($"  id: ingredientId(\"{ingredient.name}\"),");
            Debug.Log($"  name: \"{new Key($"ingredient_{ingredient.name}").GetText()}\",");
            Debug.Log($"  color: \"#{(int)(ingredient.grindedSubstanceColor.r * 255f):X2}{(int)(ingredient.grindedSubstanceColor.g * 255f):X2}{(int)(ingredient.grindedSubstanceColor.b * 255f):X2}\",");
            Debug.Log($"  price: {ingredient.GetPrice()},");
            Debug.Log("  path: [");
            for (var i = 0; i < ingredient.path.path.Count; i++)
            {
                var c = ingredient.path.path[i];
                Debug.Log($"    cubicBezierCurve({c.PFirst.x}, {c.PFirst.y}, {c.P1.x}, {c.P1.y}, {c.P2.x}, {c.P2.y}, {c.PLast.x}, {c.PLast.y}),");
            }
            Debug.Log("  ],");
            Debug.Log("  preGrindPercent: " + ingredient.path.grindedPathStartsFrom + ",");
            Debug.Log("},");
        }

        private static void DumpDay(Day day)
        {
            foreach (var template in day.templatesToSpawn)
            {
                var npcTemplate = template as NpcTemplate;
                if (npcTemplate != null)
                {
                    Debug.Log($" > NPC Template {npcTemplate.name}");
                }
            }
        }

        private static void DumpNpcTemplate(NpcTemplate npcTemplate, int index)
        {
            var prefix = " ";
            for (var i = 0; i < index; i++)
            {
                prefix += "> ";
            }

            Debug.Log($"{prefix}NPC Template {npcTemplate.name} (chance: {npcTemplate.spawnChance})");
            if (npcTemplate.baseParts.Length > 0)
            {
                Debug.Log($"{prefix}> Base parts");
                foreach (var part in npcTemplate.baseParts)
                {
                    DumpNpcPart(part, index + 2);
                }
            }

            if (npcTemplate.groupsOfContainers.Length > 0)
            {
                Debug.Log($"{prefix}> Groups");
                foreach (var group in npcTemplate.groupsOfContainers)
                {
                    Debug.Log($"{prefix}> > Group {group.groupName} chance={group.groupChance}");
                    foreach (var part in group.partsInGroup)
                    {
                        Debug.Log($"{prefix}> > > Part container chance={part.chanceBtwParts}");
                        DumpNpcPart(part.part, index + 4);
                    }
                }
            }
        }

        private static void DumpNpcPart(NonAppearancePart part, int index)
        {
            var prefix = " ";
            for (var i = 0; i < index; i++)
            {
                prefix += "> ";
            }

            if (part is Quest quest)
            {
                Debug.Log($"{prefix} Quest {quest.name}");
                Debug.Log($"{prefix}> Request Text \"{new Key("quest_text_" + quest.name).GetText()}\"");
                Debug.Log($"{prefix}> Desired Effects");
                foreach (var effect in quest.desiredEffects)
                {
                    Debug.Log($"{prefix}> > {effect.name}");
                }
            }
            else if (part is TraderSettings traderSettings)
            {
                Debug.Log($"{prefix} Trader Settings {traderSettings.name}");
                foreach (var category in traderSettings.deliveriesCategories)
                {
                    Debug.Log($"{prefix}> Category {category.name}");
                    foreach (var delivery in category.deliveries)
                    {
                        Debug.Log($"{prefix}> > Delivery {delivery.name} chance={delivery.appearingChance} min={delivery.minCount} max={delivery.maxCount} discount?={delivery.applyDiscounts} markup?={delivery.applyExtraCharge}");
                    }
                }
            }
            else if (part is DialogueData dialogueData)
            {
                Debug.Log($"{prefix} Dialogue Data");
                var nodes = new List<NodeData>();
                CollectDialogueNodeData(dialogueData, dialogueData.startDialogue, nodes);
                foreach (var node in nodes)
                {
                    DumpDialogNode(dialogueData, node, index + 1);
                }
            }
            else if (part is NpcTemplate npcTemplate)
            {
                Debug.Log($"{prefix} Child NPC template {npcTemplate.name}");
            }
        }

        private static void CollectDialogueNodeData(DialogueData dialogueData, NodeData node, List<NodeData> nodes)
        {
            if (node == null)
            {
                // huh?
                return;
            }

            if (nodes.Contains(node))
            {
                return;
            }

            nodes.Add(node);

            var count = node.GetNextNodesCount();
            for (var i = 0; i < count; i++)
            {
                var child = node.GetNext(dialogueData, i);
                CollectDialogueNodeData(dialogueData, child, nodes);
            }
        }

        private static void DumpDialogNode(DialogueData dialogueData, NodeData node, int index)
        {
            var prefix = " ";
            for (var i = 0; i < index; i++)
            {
                prefix += "> ";
            }

            if (node == null)
            {
                Debug.Log($"{prefix} Found null node!");
                return;
            }

            switch (node)
            {
                case StartDialogueNodeData startDialogueNodeData:
                    Debug.Log($"{prefix}{node.GetType().Name} guid={startDialogueNodeData.guid}");
                    break;
                case DialogueNodeData dialogueNodeData:
                    // Code prefixes key with #, but it seems to be working anyway
                    Debug.Log($"{prefix}{node.GetType().Name} guid={dialogueNodeData.guid} key={dialogueNodeData.key} text={new Key(dialogueNodeData.key).GetText()}");
                    foreach (var answer in dialogueNodeData.answers)
                    {
                        Debug.Log($"{prefix}> Answer guid={answer.guid} key={answer.key} text={new Key(answer.key).GetText()}");
                    }

                    break;
                case TradingNodeData tradingNodeData:
                    Debug.Log($"{prefix}{node.GetType().Name} guid={tradingNodeData.guid}");
                    break;
                case PotionRequestNodeData potionRequestNodeData:
                    Debug.Log($"{prefix}{node.GetType().Name} guid={potionRequestNodeData.guid}");
                    var morePort = potionRequestNodeData.morePort;
                    Debug.Log($"{prefix}> More info request guid={morePort.guid} key={morePort.key} text={new Key(morePort.key).GetText()}");
                    break;
                case EndOfDialogueNodeData endOfDialogNodeData:
                    Debug.Log($"{prefix}{node.GetType().Name} guid={endOfDialogNodeData.guid}");
                    break;
                default:
                    Debug.Log($"{prefix}Unknown node type {node.GetType().Name}");
                    break;
            }

            var count = node.GetNextNodesCount();
            for (var i = 0; i < count; i++)
            {
                var child = node.GetNext(dialogueData, i);
                if (child == null)
                {
                    continue;
                }

                Debug.Log($"{prefix}> Child {child.GetType().Name} guid={child.guid}");
            }
        }
    }
}
