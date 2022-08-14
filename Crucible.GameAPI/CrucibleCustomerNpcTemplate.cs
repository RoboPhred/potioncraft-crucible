// <copyright file="CrucibleCustomerNpcTemplate.cs" company="RoboPhredDev">
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

#if DISABLED_FOR_0_5

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::PotionCraft.DialogueSystem.Dialogue;
    using global::PotionCraft.DialogueSystem.Dialogue.Data;
    using global::PotionCraft.LocalizationSystem;
    using global::PotionCraft.Npc.Parts;
    using global::PotionCraft.Npc.Parts.Settings;
    using global::PotionCraft.QuestSystem;
    using UnityEngine;

    /// <summary>
    /// Represents an NPC Template that contains customer data.
    /// </summary>
    public sealed class CrucibleCustomerNpcTemplate : CrucibleNpcTemplate, IEquatable<CrucibleCustomerNpcTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleCustomerNpcTemplate"/> class.
        /// </summary>
        /// <param name="npcTemplate">The NPC template to wrap.</param>
        internal CrucibleCustomerNpcTemplate(NpcTemplate npcTemplate)
            : base(npcTemplate)
        {
            if (this.Quest == null)
            {
                throw new ArgumentException("NPC Template does not contain a Quest.", nameof(npcTemplate));
            }
        }

        /// <summary>
        /// Gets or sets the request text in the user's current language.
        /// </summary>
        public string RequestText
        {
            get
            {
                return new Key($"#quest_text_{this.Quest.name}").GetText();
            }

            set
            {
                CrucibleLocalization.SetLocalizationKey($"#quest_text_{this.Quest.name}", value);
            }
        }

        /// <summary>
        /// Gets a collection of potion effects that this customer accepts.
        /// </summary>
        public CrucibleQuestEffectsCollection AcceptedEffects
        {
            get
            {
                return new CrucibleQuestEffectsCollection(this.Quest);
            }
        }

        /// <summary>
        /// Gets the base game Quest for this customer.
        /// </summary>
        public Quest Quest => this.NpcTemplate.baseParts.OfType<Quest>().FirstOrDefault();

        /// <summary>
        /// Gets the NPC Template by the given name.
        /// </summary>
        /// <param name="id">The name of the npc template to fetch.</param>
        /// <returns>A <see cref="CrucibleNpcTemplate"/> api object for manipulating the template.</returns>
        public static CrucibleCustomerNpcTemplate GetCustomerNpcTemplateById(string id)
        {
            var template = NpcTemplate.allNpcTemplates.templates.Find(x => x.name == id);
            if (template == null)
            {
                return null;
            }

            if (!template.baseParts.OfType<Quest>().Any())
            {
                return null;
            }

            return new CrucibleCustomerNpcTemplate(template);
        }

        /// <summary>
        /// Creates a new blank NPC template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <param name="copyAppearanceFrom">The NPC template to copy the appearance from.</param>
        /// <returns>A new blank NPC template.</returns>
        public static CrucibleCustomerNpcTemplate CreateCustomerNpcTemplate(string name, string copyAppearanceFrom = null)
        {
            CrucibleNpcTemplate copyFromTemplate = null;
            if (!string.IsNullOrEmpty(copyAppearanceFrom))
            {
                copyFromTemplate = GetNpcTemplateById(copyAppearanceFrom);
                if (copyFromTemplate == null)
                {
                    throw new ArgumentException($"Could not find NPC template with id \"{copyAppearanceFrom}\" to copy appearance from.", nameof(copyAppearanceFrom));
                }
            }

            var template = ScriptableObject.CreateInstance<NpcTemplate>();

            template.spawnChance = 1f;

            var quest = ScriptableObject.CreateInstance<Quest>();
            quest.name = name;
            quest.karmaReward = 0;
            quest.desiredEffects = new PotionEffect[0];

            var copyFrom = NpcTemplate.allNpcTemplates.Find(x => x.name == "HerbalistNpc 1");
            if (copyFrom == null)
            {
                throw new Exception("Could not load dummy template to copy from.");
            }

            // TODO: How do prefabs differ?
            var prefab = ScriptableObject.CreateInstance<NpcPrefab>();
            var parentPrefab = copyFrom.baseParts.OfType<NpcPrefab>().FirstOrDefault();
            if (parentPrefab == null)
            {
                throw new Exception("Copy target had no prefab!");
            }

            // FIXME: Create our own prefab.  Need to disable our prefab but re-enable when npc spawns
            // var npcPrefab = UnityEngine.Object.Instantiate(parentPrefab.prefab);
            // npcPrefab.transform.parent = GameObjectUtilities.CruciblePrefabRoot;

            prefab.prefab = parentPrefab.prefab;
            prefab.clothesColorPalette1 = parentPrefab.clothesColorPalette1;
            prefab.clothesColorPalette2 = parentPrefab.clothesColorPalette2;
            prefab.clothesColorPalette3 = parentPrefab.clothesColorPalette3;
            prefab.clothesColorPalette4 = parentPrefab.clothesColorPalette4;

            // Path NonAppearancePart seems to be unused.  It is not present on the herbalist NPC

            // Used in getting potion reactions
            var gender = ScriptableObject.CreateInstance<Gender>();
            gender.gender = Gender.GenderSet.Female;

            var dialogueData = GetTestDialogue2();

            template.baseParts = new NonAppearancePart[] { quest, prefab, gender, dialogueData };

            template.name = name;
            template.groupsOfContainers = new PartContainerGroup<NonAppearancePart>[0];
            template.appearance = new AppearanceContainer();

            NpcTemplate.allNpcTemplates.templates.Add(template);

            CrucibleLocalization.SetLocalizationKey($"quest_text_{name}", "I am a new quest!");

            var crucibleTemplate = new CrucibleCustomerNpcTemplate(template);

            if (copyFromTemplate != null)
            {
                crucibleTemplate.Appearance.CopyFrom(copyFromTemplate);
            }
            else
            {
                crucibleTemplate.Appearance.Clear();
            }

            return crucibleTemplate;
        }

        /// <summary>
        /// Sets the localized request text of this customer.
        /// </summary>
        /// <param name="requestText">The localized request text to use for this customer.</param>
        public void SetLocalizedRequestText(LocalizedString requestText)
        {
            CrucibleLocalization.SetLocalizationKey($"quest_text_{this.Quest.name}", requestText);
        }

        /// <inheritdoc/>
        public bool Equals(CrucibleCustomerNpcTemplate other)
        {
            return this.NpcTemplate == other.NpcTemplate;
        }

        private static DialogueData GetTestDialogue2()
        {
            var dialogueData = ScriptableObject.CreateInstance<DialogueData>();

            var startDialogueNodeData = new StartDialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
            };
            dialogueData.startDialogue = startDialogueNodeData;

            var introData = new DialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
                key = "__intro",
                text = string.Empty,
                answers = new List<AnswerData>
                {
                    new AnswerData
                    {
                        guid = Guid.NewGuid().ToString(),
                        key = "__intro_getquest",
                        text = string.Empty,
                    },
                    new AnswerData
                    {
                        guid = Guid.NewGuid().ToString(),
                        key = "__intro_test",
                        text = string.Empty,
                    },
                    new AnswerData
                    {
                        guid = Guid.NewGuid().ToString(),
                        key = "__intro_end",
                        text = string.Empty,
                    },
                },
            };
            CrucibleLocalization.SetLocalizationKey("__intro", "Hello, this is a dialogue demo!");
            CrucibleLocalization.SetLocalizationKey("__intro_getquest", "What do you want to buy?");
            CrucibleLocalization.SetLocalizationKey("__intro_test", "This is another player choice option.");
            CrucibleLocalization.SetLocalizationKey("__intro_end", "Goodbye!");
            dialogueData.dialogues.Add(introData);

            var testData = new DialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
                key = "__test",
                text = string.Empty,
                answers = new List<AnswerData>
                {
                    new AnswerData
                    {
                        guid = Guid.NewGuid().ToString(),
                        key = "__test_return",
                        text = string.Empty,
                    },
                },
            };
            CrucibleLocalization.SetLocalizationKey("__test", "This is another dialogue node.");
            CrucibleLocalization.SetLocalizationKey("__test_return", "Cool, lets start again.");
            dialogueData.dialogues.Add(testData);

            var potionRequestNodeData = new PotionRequestNodeData
            {
                guid = Guid.NewGuid().ToString(),
                morePort = new AnswerData
                {
                    guid = Guid.NewGuid().ToString(),
                    key = "__backtointro",
                },
            };
            CrucibleLocalization.SetLocalizationKey("__backtointro", "Let's start over");
            dialogueData.potionRequests.Add(potionRequestNodeData);

            var endNodeData = new EndOfDialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
            };
            dialogueData.endsOfDialogue.Add(endNodeData);

            // start to intro
            dialogueData.edges.Add(new EdgeData
            {
                output = startDialogueNodeData.guid,
                input = introData.guid,
            });

            // intro[0] to quest
            dialogueData.edges.Add(new EdgeData
            {
                output = introData.answers[0].guid,
                input = potionRequestNodeData.guid,
            });

            // intro[1] to test
            dialogueData.edges.Add(new EdgeData
            {
                output = introData.answers[1].guid,
                input = testData.guid,
            });

            // intro[2] to end
            dialogueData.edges.Add(new EdgeData
            {
                output = introData.answers[2].guid,
                input = endNodeData.guid,
            });

            // test[0] to intro
            dialogueData.edges.Add(new EdgeData
            {
                output = testData.answers[0].guid,
                input = introData.guid,
            });

            // quest[0] to intro
            dialogueData.edges.Add(new EdgeData
            {
                output = potionRequestNodeData.morePort.guid,
                input = introData.guid,
            });

            // quest to end
            dialogueData.edges.Add(new EdgeData
            {
                output = potionRequestNodeData.guid,
                input = endNodeData.guid,
            });

            return dialogueData;
        }

        private static DialogueData GetTestDialogue1()
        {
            var dialogueData = ScriptableObject.CreateInstance<DialogueData>();

            var startDialogueNodeData = new StartDialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
            };
            dialogueData.startDialogue = startDialogueNodeData;

            var potionRequestNodeData = new PotionRequestNodeData
            {
                guid = Guid.NewGuid().ToString(),
                morePort = new AnswerData
                {
                    guid = Guid.NewGuid().ToString(),
                    key = "__morePort__data",
                    text = "Hell World!  This is a potion request question.",
                },
            };
            CrucibleLocalization.SetLocalizationKey($"__morePort__data", "This is a discussion option!");
            dialogueData.potionRequests.Add(potionRequestNodeData);

            var requestAnswerData = new DialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
                key = "__requestanswerdata",
                text = "Hello world!  This is the potion request answer.",
                answers = new List<AnswerData>
                {
                    new AnswerData
                    {
                        guid = Guid.NewGuid().ToString(),
                        key = "__requestanswerdata_answer1",
                        text = "Hello world!  This is answer data for requestAnswerData and I don't know what its here for.",
                    },
                },
            };
            CrucibleLocalization.SetLocalizationKey($"__requestanswerdata", "This is an answer!");
            CrucibleLocalization.SetLocalizationKey($"__requestanswerdata_answer1", "This is the player's response to the answer!");
            dialogueData.dialogues.Add(requestAnswerData);

            var endNodeData = new EndOfDialogueNodeData
            {
                guid = Guid.NewGuid().ToString(),
            };
            dialogueData.endsOfDialogue.Add(endNodeData);

            // start => PostionRequestNodeData
            dialogueData.edges.Add(new EdgeData
            {
                output = startDialogueNodeData.guid,
                input = potionRequestNodeData.guid,
            });

            // PotionRequestNodeData => End
            dialogueData.edges.Add(new EdgeData
            {
                output = potionRequestNodeData.guid,
                input = endNodeData.guid,
            });

            // PotionRequestNodeData => dialog
            dialogueData.edges.Add(new EdgeData
            {
                output = potionRequestNodeData.guid,
                input = requestAnswerData.guid,
            });

            // dialog => PotionRequestNodeData
            dialogueData.edges.Add(new EdgeData
            {
                output = requestAnswerData.guid,
                input = potionRequestNodeData.guid,
            });

            // Wire up the additional discussion branch
            // moreInfo => dialog
            dialogueData.edges.Add(new EdgeData
            {
                output = potionRequestNodeData.morePort.guid,
                input = requestAnswerData.guid,
            });

            // dialog => potion request
            dialogueData.edges.Add(new EdgeData
            {
                output = requestAnswerData.answers[0].guid,
                input = potionRequestNodeData.guid,
            });

            return dialogueData;
        }
    }
}

#endif
