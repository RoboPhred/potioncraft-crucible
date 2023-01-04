// <copyright file="CrucibleDialogueData.cs" company="RoboPhredDev">
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
    using global::PotionCraft.DialogueSystem.Dialogue.Data;
    using global::PotionCraft.DialogueSystem.Dialogue.LocalProperties;
    using global::PotionCraft.ManagersSystem;
    using global::PotionCraft.ObjectBased.UIElements.Dialogue;
    using UnityEngine;

    /// <summary>
    /// Provides a stable API for working with PotionCraft <see cref="DialogueData"/>s.
    /// </summary>
    public class CrucibleDialogueData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleDialogueData"/> class.
        /// </summary>
        public CrucibleDialogueData()
        {
            this.DialogueData = ScriptableObject.CreateInstance<DialogueData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleDialogueData"/> class.
        /// </summary>
        /// <param name="dialogueData">The game DialogueData to represent.</param>
        public CrucibleDialogueData(DialogueData dialogueData)
        {
            this.DialogueData = dialogueData;
        }

        /// <summary>
        /// Gets the raw object used by PotionCraft for this dialogue data.
        /// </summary>
        public DialogueData DialogueData { get; }

        /// <summary>
        /// Creates a <see cref="CrucibleDialogueData"/> based on the provided CruiclbeDialogNode.
        /// </summary>
        /// <param name="localizationKey">The localization key specific to this dialogue's parent subject..</param>
        /// <param name="startingDialogue">The starting dialogue node which links to all other dialogue options.</param>
        /// <param name="showQuestDialogue">Indicates whether or not the quest dialogue node (and answers leading up to that node) should be displayed.</param>
        /// <returns>A <see cref="CrucibleDialogueData"/> based on the provided CruiclbeDialogNode.</returns>
        public static CrucibleDialogueData CreateDialogueData(string localizationKey, CrucibleDialogueNode startingDialogue, bool showQuestDialogue, bool isTrader)
        {
            var dialogue = new CrucibleDialogueData();
            dialogue.DialogueData.name = $"{localizationKey}_dialogue";

            var localizationKeyUniqueId = 0;

            // If this is a trader make sure we have a trader node and set it up
            if (isTrader)
            {
                var traderNode = startingDialogue.TradeNode;

                // If no trade node has been specified use the first node
                if (traderNode == null)
                {
                    traderNode = startingDialogue;
                }

                // Trade nodes do not do a good job of autogenerating basic buttons so lets do it here.
                traderNode.Answers.Insert(0, new CrucibleAnswerNode
                {
                    AnswerText = "Trade",
                    IsTradeAnswer = true,
                });

                if (traderNode.Answers.Count > 3)
                {
                    traderNode.Answers = traderNode.Answers.Take(3).ToList();
                }

                traderNode.Answers.Add(new CrucibleAnswerNode
                {
                    IsConversationEndAnswer = true,
                });
            }

            // Setup the start and end nodes before iterating through the dialogue tree
            dialogue.DialogueData.startDialogue = GetNode<StartDialogueNodeData>(localizationKey, ref localizationKeyUniqueId, startingDialogue);
            dialogue.DialogueData.endsOfDialogue.Add(GetNode<EndOfDialogueNodeData>(localizationKey, ref localizationKeyUniqueId, startingDialogue));

            // Naviate through the provided dialogue nodes constructing a list of nodes and edges
            CreateDialogueNode(localizationKey, ref localizationKeyUniqueId, dialogue, startingDialogue, dialogue.DialogueData.startDialogue.guid, showQuestDialogue, isTrader);

            // Create these text nodes which are used to populate text for the end of dialogue node and the dialogue_back button
            dialogue.DialogueData.textProperties.Add(new TextProperty
            {
                entity = Property.TextEntity.Key,
                name = "#end_of_dialogue",
            });
            dialogue.DialogueData.textProperties.Add(new TextProperty
            {
                entity = Property.TextEntity.Key,
                name = "dialogue_back",
            });

            return dialogue;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="CrucibleDialogueData"/> including the underlying raw Potion Craft <see cref="DialogueData"/>.
        /// </summary>
        /// <returns>A deep copy of this <see cref="CrucibleDialogueData"/>.</returns>
        public CrucibleDialogueData Clone()
        {
            var dialogueData = ScriptableObject.CreateInstance<DialogueData>();

            // Copy all node data
            dialogueData.startDialogue = CopyNode(this.DialogueData.startDialogue);
            dialogueData.dialogues = this.DialogueData.dialogues.Select(CopyNode).ToList();
            dialogueData.conditions = this.DialogueData.conditions.Select(CopyNode).ToList();
            dialogueData.events = this.DialogueData.events.Select(CopyNode).ToList();
            dialogueData.setVariables = this.DialogueData.setVariables.Select(CopyNode).ToList();
            dialogueData.changeVariables = this.DialogueData.changeVariables.Select(CopyNode).ToList();
            dialogueData.comments = this.DialogueData.comments.Select(CopyNode).ToList();
            dialogueData.tradings = this.DialogueData.tradings.Select(CopyNode).ToList();
            dialogueData.potionRequests = this.DialogueData.potionRequests.Select(CopyNode).ToList();
            dialogueData.closenessPotionRequests = this.DialogueData.closenessPotionRequests.Select(CopyNode).ToList();
            dialogueData.endsOfDialogue = this.DialogueData.endsOfDialogue.Select(CopyNode).ToList();
            dialogueData.logicalOperators = this.DialogueData.logicalOperators.Select(CopyNode).ToList();
            dialogueData.notOperators = this.DialogueData.notOperators.Select(CopyNode).ToList();
            dialogueData.comparisonOperators = this.DialogueData.comparisonOperators.Select(CopyNode).ToList();
            dialogueData.operands = this.DialogueData.operands.Select(CopyNode).ToList();

            // Copy other data
            dialogueData.edges = this.DialogueData.edges.Select(CopyEdge).ToList();
            dialogueData.conditionProperties = this.DialogueData.conditionProperties.Select(conditionProperty => conditionProperty.Clone()).ToList();
            dialogueData.textProperties = this.DialogueData.textProperties.Select(textProperty => textProperty.Clone()).ToList();

            return new CrucibleDialogueData(dialogueData);
        }

        private static NodeData CreateDialogueNode(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueData dialogueData, CrucibleDialogueNode dialogueNode, string parentGuid, bool showQuestDialogue, bool isTrader)
        {
            if (!showQuestDialogue)
            {
                var nextDialogue = dialogueNode.NextNonQuestNode;
                if (nextDialogue == null)
                {
                    return null;
                }

                return CreateDialogueNode(localizationKey, ref localizationKeyUniqueId, dialogueData, nextDialogue, parentGuid, showQuestDialogue, isTrader);
            }

            NodeData newNode;
            if (dialogueNode.IsQuestNode)
            {
                NodeData newQuestNode;
                if (isTrader)
                {
                    var newClosenessQuestNode = GetNode<ClosenessPotionRequestNodeData>(localizationKey, ref localizationKeyUniqueId, dialogueNode);
                    dialogueData.DialogueData.closenessPotionRequests.Add(newClosenessQuestNode);
                    newQuestNode = newClosenessQuestNode;
                }
                else
                {
                    var newPotionRequestNode = GetNode<PotionRequestNodeData>(localizationKey, ref localizationKeyUniqueId, dialogueNode);
                    dialogueData.DialogueData.potionRequests.Add(newPotionRequestNode);
                    newQuestNode = newPotionRequestNode;
                }

                newNode = newQuestNode;

                // Create edge leading to this node from parent
                CreateEdge(dialogueData, parentGuid, newQuestNode.guid);

                if (isTrader)
                {
                    // Create edge leading to the end node for completing the potion request
                    CreateEdge(dialogueData, newQuestNode.guid, GetNodeToGoBackTo(dialogueData, newQuestNode).guid);
                }
                else
                {
                    // Create edge leading to the end node for completing the potion request
                    CreateEdge(dialogueData, newQuestNode.guid, dialogueData.DialogueData.endsOfDialogue.First().guid);
                }

                // Quest nodes are only allowed a single dynamic answer
                if (dialogueNode.Answers.Count > 1)
                {
                    dialogueNode.Answers = new List<CrucibleAnswerNode> { dialogueNode.Answers[0] };
                }
            }
            else
            {
                var newDialogueNode = GetNode<DialogueNodeData>(localizationKey, ref localizationKeyUniqueId, dialogueNode);
                dialogueData.DialogueData.dialogues.Add(newDialogueNode);
                newNode = newDialogueNode;

                // Create edge leading to this node from parent
                CreateEdge(dialogueData, parentGuid, newDialogueNode.guid);

                // Ensure every dialogue has a back button
                if (!dialogueNode.Answers.Any())
                {
                    dialogueNode.Answers.Add(new CrucibleAnswerNode
                    {
                        AnswerText = "[1]",
                    });
                }

                // Non-quest nodes can only have 4 answers max
                if (dialogueNode.Answers.Count > 4)
                {
                    dialogueNode.Answers = dialogueNode.Answers.Take(4).ToList();
                }

                // Ensure there is a way to get back to the beginning or end the conversation which is still possible.
                // If there is no way back add a back button to prevent soft lock.
                if (dialogueNode.Answers.All(a => !a.CanGoBack))
                {
                    if (dialogueNode.Answers.Count > 3)
                    {
                        dialogueNode.Answers = dialogueNode.Answers.Take(3).ToList();
                    }

                    dialogueNode.Answers.Add(new CrucibleAnswerNode
                    {
                        AnswerText = "[1]",
                    });
                }
            }

            foreach (var answer in dialogueNode.Answers)
            {
                if (!showQuestDialogue && answer.NextNode?.IsQuestNode == true)
                {
                    continue;
                }

                CreateAnswer(localizationKey, ref localizationKeyUniqueId, dialogueData, newNode, answer, showQuestDialogue, isTrader);
            }

            return newNode;
        }

        private static void CreateAnswer(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueData dialogueData, NodeData parent, CrucibleAnswerNode answer, bool showQuestDialogue, bool isTrader)
        {
            string answerKey;
            if (answer.IsConversationEndAnswer && answer.AnswerText == null)
            {
                answerKey = "end_of_dialogue";
            }
            else
            {
                answerKey = $"{localizationKey}_dialogue_answer_{localizationKeyUniqueId}";
                CrucibleLocalization.SetLocalizationKey(answerKey, answer.AnswerText);
            }

            // Increment the unique id so the next dialogue node has a new localization key
            localizationKeyUniqueId++;

            var newAnswer = new AnswerData
            {
                guid = Guid.NewGuid().ToString(),
                key = answerKey,
            };
            AddAnswerToParent(parent, newAnswer);

            if (answer.IsTradeAnswer)
            {
                var tradeNode = GetNode<TradingNodeData>(localizationKey, ref localizationKeyUniqueId, null);
                dialogueData.DialogueData.tradings.Add(tradeNode);

                // Create edge from answer to trading node
                CreateEdge(dialogueData, newAnswer.guid, tradeNode.guid);

                // Create edge from trading node back to parent dialogue node
                CreateEdge(dialogueData, tradeNode.guid, parent.guid);
                return;
            }

            if (answer.IsConversationEndAnswer)
            {
                var nextNode = dialogueData.DialogueData.endsOfDialogue.First();

                // Create edge from this answer to the end node
                CreateEdge(dialogueData, newAnswer.guid, nextNode.guid);
                return;
            }

            if (answer.IsBackToBeginningAnswer)
            {
                var nextNode = dialogueData.DialogueData.startDialogue.GetNext(dialogueData.DialogueData);

                // Create edge from this answer to the end node
                CreateEdge(dialogueData, newAnswer.guid, nextNode.guid);
                return;
            }

            // If there is no next node and this node is not a conversation end node then add an edge leading back to the first dialogue
            if (answer.NextNode == null)
            {
                var nextNode = GetNodeToGoBackTo(dialogueData, parent);

                // Create edge from this answer to the previous dialogue
                CreateEdge(dialogueData, newAnswer.guid, nextNode.guid);
                return;
            }

            CreateDialogueNode(localizationKey, ref localizationKeyUniqueId, dialogueData, answer.NextNode, newAnswer.guid, showQuestDialogue, isTrader);
        }

        private static NodeData GetNodeToGoBackTo(CrucibleDialogueData dialogueData, NodeData node)
        {
            var parent = dialogueData.DialogueData.edges.FirstOrDefault(e => e.input.Equals(node.guid)).output;

            // Check if this is a child node of a potion request (quest) node and return that if it is.
            var matchingPotionRequestNode = dialogueData.DialogueData.potionRequests.FirstOrDefault(p => p.morePort.guid.Equals(parent));
            if (matchingPotionRequestNode != null)
            {
                return matchingPotionRequestNode;
            }

            // Get the parent dialogue node.
            var matchingDialogueNode = dialogueData.DialogueData.dialogues.FirstOrDefault(p => p.answers.Any(a => a.guid.Equals(parent)));
            if (matchingDialogueNode != null)
            {
                return matchingDialogueNode;
            }

            // If this is a top level answer treat this answer as a conversation end answer.
            return dialogueData.DialogueData.endsOfDialogue.First();
        }

        private static void AddAnswerToParent(NodeData parent, AnswerData answer)
        {
            switch (parent)
            {
                case DialogueNodeData dialogNodeData:
                    dialogNodeData.answers.Add(answer);
                    break;
                case PotionRequestNodeData potionRequestNodeData:
                    potionRequestNodeData.morePort = answer;
                    break;
            }
        }

        private static void CreateEdge(CrucibleDialogueData dialogueData, string output, string input)
        {
            dialogueData.DialogueData.edges.Add(new EdgeData
            {
                output = output,
                input = input,
            });
        }

        private static T GetNode<T>(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueNode node)
            where T : NodeData, new()
        {
            // Populate basic node data
            var nodeData = new T
            {
                guid = Guid.NewGuid().ToString(),
            };

            // Switch on node type to populate data specific to this node type
            switch (nodeData)
            {
                case DialogueNodeData dialogNodeData:
                    // Localize node strings
                    var dialogueLocalizationKey = $"{localizationKey}_dialogue_{localizationKeyUniqueId}";
                    CrucibleLocalization.SetLocalizationKey(dialogueLocalizationKey, node.DialogueText);
                    dialogNodeData.key = dialogueLocalizationKey;

                    // Increment the unique id so the next dialogue node has a new localization key
                    localizationKeyUniqueId++;
                    break;
            }

            return nodeData;
        }

        private static T CopyNode<T>(T source)
            where T : NodeData, new()
        {
            var newNode = new T
            {
                guid = source.guid,
                position = source.position,
            };

            switch (source)
            {
                case DialogueNodeData castSource:
                    (newNode as DialogueNodeData).title = castSource.title;
                    (newNode as DialogueNodeData).key = castSource.key;
                    (newNode as DialogueNodeData).text = castSource.text;
                    (newNode as DialogueNodeData).answers = castSource.answers.Select(CopyAnswer).ToList();
                    break;
                case IfOperatorNodeData castSource:
                    (newNode as IfOperatorNodeData).title = castSource.title;
                    (newNode as IfOperatorNodeData).truePortGuid = castSource.truePortGuid;
                    (newNode as IfOperatorNodeData).falsePortGuid = castSource.falsePortGuid;
                    (newNode as IfOperatorNodeData).boolPortGuid = castSource.boolPortGuid;
                    break;
                case EventNodeData castSource:
                    (newNode as EventNodeData).title = castSource.title;
                    (newNode as EventNodeData).events = castSource.events.Select(CopyMethod).ToList();
                    break;
                case SetVariableNodeData castSource:
                    (newNode as SetVariableNodeData).title = castSource.title;
                    (newNode as SetVariableNodeData).properties = castSource.properties.Select(CopyMethod).ToList();
                    break;
                case ChangeVariableNodeData castSource:
                    (newNode as ChangeVariableNodeData).title = castSource.title;
                    (newNode as ChangeVariableNodeData).properties = castSource.properties.Select(CopyMethod).ToList();
                    break;
                case CommentNodeData castSource:
                    (newNode as CommentNodeData).text = castSource.text;
                    break;
                case PotionRequestNodeData castSource:
                    (newNode as PotionRequestNodeData).morePort = CopyAnswer(castSource.morePort);
                    break;
                case LogicalOperatorNodeData castSource:
                    (newNode as LogicalOperatorNodeData).type = castSource.type;
                    (newNode as LogicalOperatorNodeData).operand1Guid = castSource.operand1Guid;
                    (newNode as LogicalOperatorNodeData).operand2Guid = castSource.operand2Guid;
                    break;
                case ComparisonOperatorNodeData castSource:
                    (newNode as ComparisonOperatorNodeData).type = castSource.type;
                    (newNode as ComparisonOperatorNodeData).operand1Guid = castSource.operand1Guid;
                    (newNode as ComparisonOperatorNodeData).operand2Guid = castSource.operand2Guid;
                    break;
                case OperandNodeData castSource:
                    (newNode as OperandNodeData).type = castSource.type;
                    (newNode as OperandNodeData).value = castSource.value;
                    (newNode as OperandNodeData).customValue = castSource.customValue;
                    break;
            }

            return newNode;
        }

        private static AnswerData CopyAnswer(AnswerData source)
        {
            return new AnswerData
            {
                guid = source.guid,
                key = source.key,
                text = source.text,
            };
        }

        private static NodeData.Method CopyMethod(NodeData.Method source)
        {
            return new NodeData.Method
            {
                name = source.name,
                parameters = source.parameters,
            };
        }

        private static EdgeData CopyEdge(EdgeData edge)
        {
            return new EdgeData
            {
                output = edge.output,
                input = edge.input,
            };
        }

        /// <summary>
        /// Defines a single node of an NPC's dialogue.
        /// Currently there is no way to create a dialogue tree from a Potion Craft <see cref="DialogueData"/>.
        /// These structs are used only to construct Potion Craft <see cref="DialogueData"/>.
        /// </summary>
        public class CrucibleDialogueNode
        {
            /// <summary>
            /// Gets or sets the NPC's dialogue text for this node. This can be left blank for quest nodes.
            /// </summary>
            public LocalizedString DialogueText { get; set; }

            /// <summary>
            /// Gets or sets the list of possible responses to this dialogue node.
            /// </summary>
            public List<CrucibleAnswerNode> Answers { get; set; } = new List<CrucibleAnswerNode>();

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is a quest node. This value should be true for a trader's special request closeness quest node.
            /// </summary>
            public bool IsQuestNode { get; set; }

            /// <summary>
            /// Gets a value indicating whether or not this or any child nodes are a quest node.
            /// </summary>
            public bool HasQuestNode => this.IsQuestNode || this.Answers.Any(a => a.NextNode?.HasQuestNode ?? false);

            /// <summary>
            /// Gets a value indicating whether or not any child node can go back to the beginning or end the conversation.
            /// </summary>
            public bool HasWayBackToBeginning => this.Answers.Any(a => a.HasWayBackToBeginning);

            /// <summary>
            /// Gets the next non-quest node from all child nodes.
            /// </summary>
            public CrucibleDialogueNode NextNonQuestNode => this.Answers.Select(a => a.NextNonQuestNode).FirstOrDefault(n => n != null);

            /// <summary>
            /// Gets this dialogue tree's trade node if one exists by searching this node and all child nodes.
            /// </summary>
            public CrucibleDialogueNode TradeNode => this.IsTradeNode ? this : this.Answers.Select(a => a.NextNode).FirstOrDefault(n => n != null && n.IsTradeNode);

            private bool IsTradeNode => this.Answers.Any(a => a.IsTradeAnswer);
        }

        /// <summary>
        /// Defines a possible response to an NPC's dialogue.
        /// </summary>
        public class CrucibleAnswerNode
        {
            /// <summary>
            /// Gets or sets the text used to populate the answer button.
            /// </summary>
            public LocalizedString AnswerText { get; set; }

            /// <summary>
            /// Gets or sets the next node which would be loaded if this answer is selected. If this is equalt to CrucibleDialogNode.Empty this answer is treated as either the end of the conversation or as an answer which should return to the starting node.
            /// </summary>
            public CrucibleDialogueNode NextNode { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is the answer which ends interaction with the trader. There should only be a single one of these nodes in the dialog system.
            /// </summary>
            public bool IsConversationEndAnswer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is the answer which goes back to the beginning of the conversation.
            /// </summary>
            public bool IsBackToBeginningAnswer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is the answer which goes to the trade screen.
            /// </summary>
            public bool IsTradeAnswer { get; set; }


            /// <summary>
            /// Gets a value indicating whether or not this or any child node can go back to the parent node.
            /// </summary>
            public bool CanGoBack => this.NextNode == null || this.IsBackToBeginningAnswer || this.IsConversationEndAnswer || this.NextNode.HasWayBackToBeginning;

            /// <summary>
            /// Gets a value indicating whether or not this or any child node can go back to the beginning or end the conversation.
            /// </summary>
            public bool HasWayBackToBeginning => this.IsBackToBeginningAnswer || this.IsConversationEndAnswer || (this.NextNode?.HasWayBackToBeginning ?? false);

            /// <summary>
            /// Gets the next non-quest node from all child nodes.
            /// </summary>
            public CrucibleDialogueNode NextNonQuestNode => this.NextNode?.IsQuestNode != true ? this.NextNode : this.NextNode?.NextNonQuestNode;
        }
    }
}
