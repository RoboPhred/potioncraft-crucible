// <copyright file="CrucibleNpcAppearance.cs" company="RoboPhredDev">
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
    using System.Linq;
    using Npc.Parts;
    using Npc.Parts.Appearance;
    using Npc.Parts.Settings;
    using UnityEngine;

    /// <summary>
    /// Contains methods and properties to control an NPC's appearance.
    /// </summary>
    public sealed class CrucibleNpcAppearance
    {
        private readonly NpcTemplate npcTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleNpcAppearance"/> class.
        /// </summary>
        /// <param name="template">The npc template to control appearance for.</param>
        internal CrucibleNpcAppearance(NpcTemplate template)
        {
            this.npcTemplate = template;
        }

        /// <summary>
        /// Gets or sets the head background for this npc.
        /// </summary>
        /// <remarks>
        /// <p>
        /// NPC templates can have multiple possible values for many art assets.
        /// <p>
        /// <p>
        /// If this template has multiple values, this property will return the first one.
        /// </p>
        /// <p>
        /// If setting this property on an npc with multiple candidates, all candidates will be removed in favor of the provided value
        /// </p>
        /// </remarks>
        public Sprite HeadBackground
        {
            get
            {
                return this.npcTemplate.appearance.skullShape.partsInGroup.FirstOrDefault()?.part.shape.background;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.skullShape.partsInGroup.FirstOrDefault()?.part;
                var newPart = ScriptableObject.CreateInstance<SkullShape>();

                if (oldPart != null)
                {
                    newPart.mask = oldPart.mask;
                    newPart.shape = oldPart.shape.Clone();
                    newPart.useWith = oldPart.useWith; // This doesn't seem to be used by anything.
                }
                else
                {
                    newPart.mask = SpriteUtilities.CreateBlankSprite(25, 25, Color.black);
                    newPart.shape = new AppearancePart.ColorablePart
                    {
                        contour = SpriteUtilities.CreateBlankSprite(25, 25, Color.clear),
                        scratches = SpriteUtilities.CreateBlankSprite(25, 25, Color.clear),
                    };
                }

                newPart.shape = newPart.shape.Clone();
                newPart.shape.background = value;

                this.npcTemplate.appearance.skullShape.partsInGroup = new[]
                {
                    new PartContainer<SkullShape>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite BodyBackground
        {
            get
            {
                return this.npcTemplate.appearance.body.partsInGroup.FirstOrDefault()?.part.bodyBase[0].background;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.body.partsInGroup.FirstOrDefault()?.part;
                var newPart = ScriptableObject.CreateInstance<Body>();

                if (oldPart != null)
                {
                    newPart.bodyBase = oldPart.bodyBase.Select(x => x.Clone()).ToArray();
                    newPart.bodyBaseSkin = oldPart.bodyBaseSkin.Clone();
                    newPart.handBack = oldPart.handBack.Select(x => x.Clone()).ToArray();
                    newPart.handBackSkin = oldPart.handBackSkin.Clone();
                    newPart.handFront = oldPart.handFront.Select(x => x.Clone()).ToArray();
                    newPart.handFrontSkin = oldPart.handFrontSkin.Clone();
                    newPart.hinge = oldPart.hinge.Select(x => x.Clone()).ToArray();
                    newPart.hingePartColor = oldPart.hingePartColor;
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    var blankColorablePart = new AppearancePart.ColorablePart
                    {
                        background = blankTexture,
                        contour = blankTexture,
                        scratches = blankTexture,
                    };
                    var blankColorablePartSet = new[] { blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart };
                    newPart.bodyBase = blankColorablePartSet;
                    newPart.bodyBaseSkin = blankColorablePart;
                    newPart.handBack = blankColorablePartSet;
                    newPart.handBackSkin = blankColorablePart;
                    newPart.handFront = blankColorablePartSet;
                    newPart.handFrontSkin = blankColorablePart;
                }

                newPart.bodyBase[0] = newPart.bodyBase[0].Clone();
                newPart.bodyBase[0].background = value;

                this.npcTemplate.appearance.body.partsInGroup = new[]
                {
                    new PartContainer<Body>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite ArmRightBackground
        {
            get
            {
                return this.npcTemplate.appearance.body.partsInGroup.FirstOrDefault()?.part.handFront[0].background;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.body.partsInGroup.FirstOrDefault()?.part;
                var newPart = ScriptableObject.CreateInstance<Body>();

                if (oldPart != null)
                {
                    newPart.bodyBase = oldPart.bodyBase.Select(x => x.Clone()).ToArray();
                    newPart.bodyBaseSkin = oldPart.bodyBaseSkin.Clone();
                    newPart.handBack = oldPart.handBack.Select(x => x.Clone()).ToArray();
                    newPart.handBackSkin = oldPart.handBackSkin.Clone();
                    newPart.handFront = oldPart.handFront.Select(x => x.Clone()).ToArray();
                    newPart.handFrontSkin = oldPart.handFrontSkin.Clone();
                    newPart.hinge = oldPart.hinge.Select(x => x.Clone()).ToArray();
                    newPart.hingePartColor = oldPart.hingePartColor;
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    var blankColorablePart = new AppearancePart.ColorablePart
                    {
                        background = blankTexture,
                        contour = blankTexture,
                        scratches = blankTexture,
                    };
                    var blankColorablePartSet = new[] { blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart };
                    newPart.bodyBase = blankColorablePartSet;
                    newPart.bodyBaseSkin = blankColorablePart;
                    newPart.handBack = blankColorablePartSet;
                    newPart.handBackSkin = blankColorablePart;
                    newPart.handFront = blankColorablePartSet;
                    newPart.handFrontSkin = blankColorablePart;
                }

                newPart.handFront[0] = newPart.handFront[0].Clone();
                newPart.handFront[0].background = value;

                this.npcTemplate.appearance.body.partsInGroup = new[]
                {
                    new PartContainer<Body>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite FaceContour
        {
            get
            {
                return this.npcTemplate.appearance.face.partsInGroup.FirstOrDefault()?.part.hair[0].contour;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.face.partsInGroup.FirstOrDefault();
                var newPart = ScriptableObject.CreateInstance<Face>();

                if (oldPart != null)
                {
                    newPart.hair = oldPart.part.hair.Select(x => x.Clone()).ToArray();
                    newPart.skin = oldPart.part.skin.Select(x => x.Clone()).ToArray();
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    var blankColorablePart = new AppearancePart.ColorablePart
                    {
                        background = blankTexture,
                        contour = blankTexture,
                        scratches = blankTexture,
                    };

                    // 5 of these:
                    // 0 - wrong potion flash
                    // 1, 2 - anger states for being given wrong potions
                    // 3 - idle
                    // 4 - satisfied, given potion and leaviong
                    // 5 - o-face emotion when shown a matching potion
                    var blankColorablePartSet = new[] { blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart };
                    newPart.hair = blankColorablePartSet;
                    newPart.skin = blankColorablePartSet;
                }

                // Set the face contour for all emotions
                newPart.hair = newPart.hair.Select(x =>
                {
                    var r = x.Clone();
                    r.contour = value;
                    return r;
                }).ToArray();

                this.npcTemplate.appearance.face.partsInGroup = new[]
                {
                    new PartContainer<Face>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite EyeLeft
        {
            get
            {
                return this.npcTemplate.appearance.eyes.partsInGroup.FirstOrDefault()?.part.left;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.eyes.partsInGroup.FirstOrDefault();
                var newPart = ScriptableObject.CreateInstance<Eyes>();

                if (oldPart != null)
                {
                    newPart.left = oldPart.part.left;
                    newPart.right = oldPart.part.right;
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    newPart.left = blankTexture;
                    newPart.right = blankTexture;
                }

                newPart.left = value;

                this.npcTemplate.appearance.eyes.partsInGroup = new[]
                {
                    new PartContainer<Eyes>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite EyeRight
        {
            get
            {
                return this.npcTemplate.appearance.eyes.partsInGroup.FirstOrDefault()?.part.right;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.eyes.partsInGroup.FirstOrDefault();
                var newPart = ScriptableObject.CreateInstance<Eyes>();

                if (oldPart != null)
                {
                    newPart.left = oldPart.part.left;
                    newPart.right = oldPart.part.right;
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    newPart.left = blankTexture;
                    newPart.right = blankTexture;
                }

                newPart.right = value;

                this.npcTemplate.appearance.eyes.partsInGroup = new[]
                {
                    new PartContainer<Eyes>
                    {
                        part = newPart,
                    },
                };
            }
        }

        public Sprite HairFrontRight
        {
            get
            {
                return this.npcTemplate.appearance.hairstyle.partsInGroup.FirstOrDefault()?.part.longFront.contour;
            }

            set
            {
                var oldPart = this.npcTemplate.appearance.hairstyle.partsInGroup.FirstOrDefault();
                var newPart = ScriptableObject.CreateInstance<Hairstyle>();

                if (oldPart != null)
                {
                    newPart.back = oldPart.part.back.Clone();
                    newPart.longFront = oldPart.part.longFront.Clone();
                    newPart.shortFront = oldPart.part.shortFront.Clone();
                    newPart.middle = oldPart.part.middle.Clone();
                }
                else
                {
                    var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
                    var blankColorablePart = new AppearancePart.ColorablePart
                    {
                        background = blankTexture,
                        contour = blankTexture,
                        scratches = blankTexture,
                    };
                    newPart.back = blankColorablePart.Clone();
                    newPart.longFront = blankColorablePart.Clone();
                    newPart.shortFront = blankColorablePart.Clone();
                    newPart.middle = blankColorablePart.Clone();
                }

                newPart.longFront.background = value;

                this.npcTemplate.appearance.hairstyle.partsInGroup = new[]
                {
                    new PartContainer<Hairstyle>
                    {
                        part = newPart,
                    },
                };
            }
        }

        /// <summary>
        /// Clears out all appearance data.
        /// </summary>
        public void Clear()
        {
            var blankSprite = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
            blankSprite.name = "Crucible npc sprite placeholder";
            var blankColorablePart = new AppearancePart.ColorablePart
            {
                background = blankSprite,
                contour = blankSprite,
                scratches = blankSprite,
            };

            // Colorable sets seem to always involve 5 parts: 1 base and 4 recolorable.
            var blankColorablePartSet = new[] { blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart };

            // The following assets need to be set to blank sprites, otherwise default art assets from the herbalist will be used.
            var body = ScriptableObject.CreateInstance<Body>();
            body.bodyBase = blankColorablePartSet;
            body.bodyBaseSkin = blankColorablePart;
            body.handBack = blankColorablePartSet;
            body.handBackSkin = blankColorablePart;
            body.handFront = blankColorablePartSet;
            body.handFrontSkin = blankColorablePart;

            var skullShape = ScriptableObject.CreateInstance<SkullShape>();
            skullShape.mask = blankSprite;
            skullShape.shape = blankColorablePart;

            var hairStyle = ScriptableObject.CreateInstance<Hairstyle>();
            hairStyle.back = blankColorablePart;
            hairStyle.longFront = blankColorablePart;
            hairStyle.middle = blankColorablePart;
            hairStyle.shortFront = blankColorablePart;

            var face = ScriptableObject.CreateInstance<Face>();
            face.hair = blankColorablePartSet;
            face.skin = blankColorablePartSet;

            var eyes = ScriptableObject.CreateInstance<Eyes>();
            eyes.left = blankSprite;
            eyes.right = blankSprite;

            this.npcTemplate.appearance = new AppearanceContainer
            {
                aboveHairFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryAboveHair>(),
                aboveHairFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryAboveHair>(),
                behindBodyFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryBehindBody>(),
                behindBodyFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryBehindBody>(),
                body = new PartContainerGroup<Body>
                {
                    partsInGroup = new[]
                    {
                        new PartContainer<Body>
                        {
                            part = body,
                        },
                    },
                },
                bodyFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryBody>(),
                bodyFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryBody>(),
                breastSize = new PartContainerGroup<Breast>(),
                clothesColor1 = new PartContainerGroup<AppearanceColor>(),
                clothesColor2 = new PartContainerGroup<AppearanceColor>(),
                clothesColor3 = new PartContainerGroup<AppearanceColor>(),
                clothesColor4 = new PartContainerGroup<AppearanceColor>(),
                eyes = new PartContainerGroup<Eyes>
                {
                    partsInGroup = new[]
                    {
                        new PartContainer<Eyes>
                        {
                            part = eyes,
                        },
                    },
                },
                face = new PartContainerGroup<Face>
                {
                    partsInGroup = new[]
                    {
                        new PartContainer<Face>
                        {
                            part = face,
                        },
                    },
                },
                faceFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryFace>(),
                faceFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryFace>(),
                hairColor = new PartContainerGroup<AppearanceColor>(),
                hairstyle = new PartContainerGroup<Hairstyle>
                {
                    partsInGroup = new[]
                    {
                        new PartContainer<Hairstyle>
                        {
                            part = hairStyle,
                        },
                    },
                },
                hat = new PartContainerGroup<Hat>(),
                handBackFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryHandBack>(),
                handBackFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryHandBack>(),
                handFrontFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryHandFront>(),
                handFrontFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryHandFront>(),
                skinColor = new PartContainerGroup<AppearanceColor>(),
                shortHairFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryShortHair>(),
                shortHairFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessoryShortHair>(),
                skullShape = new PartContainerGroup<SkullShape>
                {
                    partsInGroup = new[]
                    {
                        new PartContainer<SkullShape>
                        {
                            part = skullShape,
                        },
                    },
                },
                skullShapeFeature1 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessorySkullShape>(),
                skullShapeFeature2 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessorySkullShape>(),
                skullShapeFeature3 = new PartContainerGroup<Npc.Parts.Appearance.Accessories.AccessorySkullShape>(),
            };
        }

        /// <summary>
        /// Copies the appearance from the given npc template to this npc.
        /// </summary>
        /// <remarks>
        /// Appearance data is made up of multiple parts that themselves contain random chances.  This copy command
        /// copies all data, including the randomized data.  As such, the npc appearance might change whenever the NPC shows up.
        /// </remarks>
        /// <param name="npcTemplate">The npc template to copy the appearance from.</param>
        public void CopyFrom(CrucibleNpcTemplate npcTemplate)
        {
            this.CopyFrom(npcTemplate.Appearance);
        }

        /// <summary>
        /// Copies the appearance from the given npc appearance data to this one.
        /// </summary>
        /// <remarks>
        /// Appearance data is made up of multiple parts that themselves contain random chances.  This copy command
        /// copies all data, including the randomized data.  As such, the npc appearance might change whenever the NPC shows up.
        /// </remarks>
        /// <param name="sourceAppearance">The appearance to copy from.</param>
        public void CopyFrom(CrucibleNpcAppearance sourceAppearance)
        {
            // There's lots of data encoded on the prefab :(
            var prefab = this.RequirePart<Prefab>();
            var sourcePrefab = sourceAppearance.RequirePart<Prefab>();

            var sourceTemplate = sourceAppearance.npcTemplate;

            prefab.prefab = sourcePrefab.prefab;
            prefab.clothesColorPalette1 = sourcePrefab.clothesColorPalette1;
            prefab.clothesColorPalette2 = sourcePrefab.clothesColorPalette2;
            prefab.clothesColorPalette3 = sourcePrefab.clothesColorPalette3;
            prefab.clothesColorPalette4 = sourcePrefab.clothesColorPalette4;
            prefab.hairColorPalette = sourcePrefab.hairColorPalette;
            prefab.skinColorPalette = sourcePrefab.skinColorPalette;

            this.npcTemplate.appearance = new AppearanceContainer
            {
                aboveHairFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.aboveHairFeature1),
                aboveHairFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.aboveHairFeature2),
                behindBodyFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.behindBodyFeature1),
                behindBodyFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.behindBodyFeature2),
                body = ClonePartContainerGroup(sourceTemplate.appearance.body),
                bodyFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.bodyFeature1),
                bodyFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.bodyFeature2),
                breastSize = ClonePartContainerGroup(sourceTemplate.appearance.breastSize),
                clothesColor1 = ClonePartContainerGroup(sourceTemplate.appearance.clothesColor1),
                clothesColor2 = ClonePartContainerGroup(sourceTemplate.appearance.clothesColor2),
                clothesColor3 = ClonePartContainerGroup(sourceTemplate.appearance.clothesColor3),
                clothesColor4 = ClonePartContainerGroup(sourceTemplate.appearance.clothesColor4),
                eyes = ClonePartContainerGroup(sourceTemplate.appearance.eyes),
                face = ClonePartContainerGroup(sourceTemplate.appearance.face),
                faceFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.faceFeature1),
                faceFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.faceFeature2),
                hairColor = ClonePartContainerGroup(sourceTemplate.appearance.hairColor),
                hairstyle = ClonePartContainerGroup(sourceTemplate.appearance.hairstyle),
                hat = ClonePartContainerGroup(sourceTemplate.appearance.hat),
                handBackFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.handBackFeature1),
                handBackFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.handBackFeature2),
                handFrontFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.handFrontFeature2),
                handFrontFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.handFrontFeature1),
                skinColor = ClonePartContainerGroup(sourceTemplate.appearance.skinColor),
                shortHairFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.shortHairFeature2),
                shortHairFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.shortHairFeature1),
                skullShape = ClonePartContainerGroup(sourceTemplate.appearance.skullShape),
                skullShapeFeature1 = ClonePartContainerGroup(sourceTemplate.appearance.skullShapeFeature1),
                skullShapeFeature2 = ClonePartContainerGroup(sourceTemplate.appearance.skullShapeFeature2),
                skullShapeFeature3 = ClonePartContainerGroup(sourceTemplate.appearance.skullShapeFeature3),
            };
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

        private T RequirePart<T>()
            where T : NonAppearancePart
        {
            var part = this.npcTemplate.baseParts.OfType<T>().FirstOrDefault();
            if (part == null)
            {
                throw new InvalidOperationException($"NPC template {this.npcTemplate.name} does not have a {typeof(T).Name} part.");
            }

            return part;
        }
    }
}
