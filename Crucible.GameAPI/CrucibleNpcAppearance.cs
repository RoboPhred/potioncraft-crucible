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

        /// <summary>
        /// Clears out all appearance data.
        /// </summary>
        public void Clear()
        {
            var blankTexture = SpriteUtilities.CreateBlankSprite(10, 10, Color.clear);
            var blankColorablePart = new AppearancePart.ColorablePart
            {
                background = blankTexture,
                contour = blankTexture,
                scratches = blankTexture,
            };
            var blankColorablePartSet = new[] { blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart, blankColorablePart };

            var body = ScriptableObject.CreateInstance<Body>();
            body.bodyBase = blankColorablePartSet;
            body.bodyBaseSkin = blankColorablePart;
            body.handBack = blankColorablePartSet;
            body.handBackSkin = blankColorablePart;
            body.handFront = blankColorablePartSet;
            body.handFrontSkin = blankColorablePart;

            var skullShape = ScriptableObject.CreateInstance<SkullShape>();
            skullShape.mask = blankTexture;
            skullShape.shape = blankColorablePart;

            var hairStyle = ScriptableObject.CreateInstance<Hairstyle>();
            hairStyle.back = blankColorablePart;
            hairStyle.longFront = blankColorablePart;
            hairStyle.middle = blankColorablePart;
            hairStyle.shortFront = blankColorablePart;

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
                eyes = new PartContainerGroup<Eyes>(),
                face = new PartContainerGroup<Face>(),
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
