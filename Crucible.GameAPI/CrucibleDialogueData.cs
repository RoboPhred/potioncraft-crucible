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
        /// <param name="isCustomer">True if the dialogue is being created for a customer.</param>
        /// <returns>A <see cref="CrucibleDialogueData"/> based on the provided CruiclbeDialogNode.</returns>
        public static CrucibleDialogueData CreateDialogueData(string localizationKey, CrucibleDialogueNode startingDialogue, bool isCustomer)
        {
            var dialogue = new CrucibleDialogueData();

            var localizationKeyUniqueId = 0;

            // Setup the start and end nodes before iterating through the dialogue tree
            dialogue.DialogueData.startDialogue = GetNode<StartDialogueNodeData>(localizationKey, ref localizationKeyUniqueId, startingDialogue);
            dialogue.DialogueData.endsOfDialogue.Add(GetNode<EndOfDialogueNodeData>(localizationKey, ref localizationKeyUniqueId, startingDialogue));

            // Mark the initial node as a quest node if this is a customer
            if (isCustomer)
            {
                startingDialogue.IsQuestNode = true;
            }

            // Naviate through the provided dialogue nodes constructing a list of nodes and edges
            var node = CreateDialogueNode(localizationKey, ref localizationKeyUniqueId, dialogue, startingDialogue);

            // Create an edge linking the starting node to the first dialogue node
            CreateEdge(dialogue, dialogue.DialogueData.startDialogue.guid, node.guid);

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

        private static NodeData CreateDialogueNode(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueData dialogueData, CrucibleDialogueNode dialogueNode)
        {
            NodeData newNode;
            if (dialogueNode.IsQuestNode)
            {
                var newQuestNode = GetNode<PotionRequestNodeData>(localizationKey, ref localizationKeyUniqueId, dialogueNode);
                dialogueData.DialogueData.potionRequests.Add(newQuestNode);
                newNode = newQuestNode;

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
            }

            foreach(var answer in dialogueNode.Answers)
            {
                CreateAnswer(localizationKey, ref localizationKeyUniqueId, dialogueData, newNode, answer);
            }

            return newNode;
        }

        private static void CreateAnswer(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueData dialogueData, NodeData parent, CrucibleAnswerNode answer)
        {
            var localizationkey = $"{localizationKey}_dialogue_{localizationKeyUniqueId}";
            CrucibleLocalization.SetLocalizationKey(localizationkey, answer.AnswerText);
            var newAnswer = new AnswerData
            {
                guid = Guid.NewGuid().ToString(),
                key = localizationKey,
            };
            AddAnswerToParent(parent, newAnswer);

            // If there is no next node and this node is not a conversation end node then add an edge leading back to the first dialogue
            NodeData nextNode = null;
            if (answer.NextNode.Equals(CrucibleDialogueNode.Empty) && !answer.IsConversationEndAnswer)
            {
                nextNode = GetNodeToGoBackTo(dialogueData, parent);
            }
            else if (answer.IsConversationEndAnswer)
            {
                nextNode = dialogueData.DialogueData.endsOfDialogue.First();
            }
            else if (!answer.NextNode.Equals(CrucibleDialogueNode.Empty))
            {
                nextNode = CreateDialogueNode(localizationKey, ref localizationKeyUniqueId, dialogueData, answer.NextNode);
            }

            if (nextNode == null)
            {
                return;
            }

            CreateEdge(dialogueData, newAnswer.guid, nextNode.guid);
        }

        private static NodeData GetNodeToGoBackTo(CrucibleDialogueData dialogueData, NodeData node)
        {
            var parent = dialogueData.DialogueData.edges.FirstOrDefault(e => e.input.Equals(node.guid)).output;

            // Check if this is a child node of a potion request (quest) node and return that if it is.
            var matchingPotionRequestNode = dialogueData.DialogueData.potionRequests.FirstOrDefault(p => p.guid.Equals(parent));
            if (matchingPotionRequestNode != null)
            {
                return matchingPotionRequestNode;
            }

            // Get the parent dialogue node.
            var matchingDialogueNode = dialogueData.DialogueData.dialogues.FirstOrDefault(p => p.guid.Equals(parent));
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
            throw new NotImplementedException();
        }

        private static T GetNode<T>(string localizationKey, ref int localizationKeyUniqueId, CrucibleDialogueNode node)
            where T : NodeData
        {
            var nodeData = default(T);

            // Populate basic node data
            nodeData.guid = Guid.NewGuid().ToString();

            // Switch on node type to populate data specific to this node type
            switch (nodeData)
            {
                case StartDialogueNodeData startingNodeData:
                    // TODO is there anything special we need to do for the starting node?
                    break;
                case DialogueNodeData dialogNodeData:
                    // Localize node strings
                    var dialogueLocalizationKey = $"{localizationKey}_dialogue_{localizationKeyUniqueId}";
                    CrucibleLocalization.SetLocalizationKey(dialogueLocalizationKey, node.Dialogue);
                    dialogNodeData.key = dialogueLocalizationKey;

                    // Increment the unique id so the next dialogue node has a new localization key
                    localizationKeyUniqueId++;
                    break;
            }

            return nodeData;
        }

        private static T CopyNode<T>(T source)
            where T : NodeData
        {
            var newNode = default(T);
            newNode.guid = source.guid;
            newNode.position = source.position;

            switch(source)
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
        public struct CrucibleDialogueNode : IEqualityComparer<CrucibleDialogueNode>
        {
            /// <summary>
            /// Gets the default value for any node which should be treated like a null value for a class.
            /// </summary>
            public static CrucibleDialogueNode Empty { get; } = new CrucibleDialogueNode { };

            /// <summary>
            /// Gets or sets the NPC's dialogue text for this node. This can be left blank for quest nodes.
            /// </summary>
            public LocalizedString Dialogue { get; set; }

            /// <summary>
            /// Gets or sets the list of possible responses to this dialogue node.
            /// </summary>
            public List<CrucibleAnswerNode> Answers { get; set; } = new List<CrucibleAnswerNode>();

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is a quest node. This value should be true for a trader's special request closeness quest node.
            /// </summary>
            public bool IsQuestNode { get; set; }

            /// <inheritdoc/>
            public bool Equals(CrucibleDialogueNode x, CrucibleDialogueNode y)
            {
                return x.Dialogue.Equals(y.Dialogue);
            }

            /// <inheritdoc/>
            public int GetHashCode(CrucibleDialogueNode obj)
            {
                return obj.Dialogue.GetHashCode();
            }
        }

        /// <summary>
        /// Defines a possible response to an NPC's dialogue.
        /// </summary>
        public struct CrucibleAnswerNode
        {
            /// <summary>
            /// Gets or sets the text used to populate the answer button.
            /// </summary>
            public LocalizedString AnswerText { get; set; }

            /// <summary>
            /// Gets or sets the next node which would be loaded if this answer is selected. If this is equalt to CrucibleDialogNode.Empty this answer is treated as either the end of the conversation or as an answer which should return to the starting node.
            /// </summary>
            public CrucibleDialogueNode NextNode { get; set; } = CrucibleDialogueNode.Empty;

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is the answer which opens the trader's trade screen. There should only be a single one of these nodes in the dialog system.
            /// </summary>
            public bool IsTradeAnswer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this node is the answer which ends interaction with the trader. There should only be a single one of these nodes in the dialog system.
            /// </summary>
            public bool IsConversationEndAnswer { get; set; }
        }
    }
}
