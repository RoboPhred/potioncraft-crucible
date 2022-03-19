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
        private static readonly Sprite BlankSprite = SpriteUtilities.CreateBlankSprite(1, 1, Color.clear).WithName("Crucible appearance blank");

        private static readonly AppearancePart.ColorablePart BlankColorablePart = new AppearancePart.ColorablePart
        {
            background = BlankSprite,
            contour = BlankSprite,
            scratches = BlankSprite,
        };

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
        /// Indidcates a colorable artwork layer.
        /// </summary>
        public enum ColorLayer
        {
            /// <summary>
            /// The base, uncolorable layer
            /// </summary>
            Base,

            /// <summary>
            /// The first colorable layer
            /// </summary>
            Colorable1,

            /// <summary>
            /// The second colorable layer
            /// </summary>
            Colorable2,

            /// <summary>
            /// The third colorable layer
            /// </summary>
            Colorable3,

            /// <summary>
            /// The fourth colorable layer
            /// </summary>
            Colorable4,
        }

        public enum Emotion
        {
            NegativeReaction = 0,
            Anger2 = 1,
            Anger1 = 2,
            Idle = 3,
            Happy1 = 4,
            PositiveReaction = 5,
        }

        /// <summary>
        /// Clear all head shapes from this appearance.
        /// </summary>
        public void ClearHeadShapes()
        {
            var skullShape = ScriptableObject.CreateInstance<SkullShape>();
            skullShape.mask = BlankSprite;
            skullShape.shape = BlankColorablePart;

            this.npcTemplate.appearance.skullShape.partsInGroup = new PartContainer<SkullShape>[]
            {
                new PartContainer<SkullShape>
                {
                    part = skullShape,
                },
            };
        }

        /// <summary>
        /// Adds a head shape to this appearance.
        /// </summary>
        /// <param name="headShape">The head shape sprite.</param>
        /// <param name="chance">The chance for this head shape to be chosen.</param>
        public void AddHeadShape(Sprite headShape, float chance = 1f)
        {
            var mask = SpriteUtilities.CreateBlankSprite((int)Math.Ceiling(headShape.bounds.size.x), (int)Math.Ceiling(headShape.bounds.size.y), Color.clear);
            this.AddHeadShape(mask: mask, background: headShape, chance: chance);
        }

        /// <summary>
        /// Adds a head shape to this appearance.
        /// </summary>
        /// <param name="mask">The mask for the head shape.</param>
        /// <param name="background">The head shape background sprite.</param>
        /// <param name="contour">The head shape contour sprite.</param>
        /// <param name="scratches">The head shape scratches sprite.</param>
        /// <param name="chance">The chance for this head shape to be chosen.</param>
        public void AddHeadShape(Sprite mask, Sprite background, Sprite contour = null, Sprite scratches = null, float chance = 1f)
        {
            var skullShape = ScriptableObject.CreateInstance<SkullShape>();
            skullShape.mask = mask;
            skullShape.shape = new AppearancePart.ColorablePart
            {
                background = background,
                contour = contour,
                scratches = scratches,
            };

            var container = new PartContainer<SkullShape>
            {
                part = skullShape,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.skullShape;

            // Remove our blank if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.shape.background.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { container };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { container }).ToArray();
            }
        }

        /// <summary>
        /// Clears out all bodies on this appearance.
        /// </summary>
        public void ClearBodies()
        {
            var blankColorablePartSet = new[] { BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart };

            var body = ScriptableObject.CreateInstance<Body>();
            body.bodyBase = blankColorablePartSet;
            body.bodyBaseSkin = BlankColorablePart;
            body.handBack = blankColorablePartSet;
            body.handBackSkin = BlankColorablePart;
            body.handFront = blankColorablePartSet;
            body.handFrontSkin = BlankColorablePart;

            this.npcTemplate.appearance.body.partsInGroup = new PartContainer<Body>[]
            {
                new PartContainer<Body>
                {
                    part = body,
                },
            };
        }

        /// <summary>
        /// Adds a body to the appearance.
        /// </summary>
        /// <param name="body">The sprite to use for the body.</param>
        /// <param name="leftArm">The sprite to use for the left arm.</param>
        /// <param name="rightArm">The sprite to use for the right arm.</param>
        /// <param name="chance">The chance for this body to be selected.</param>
        public void AddBody(Sprite body, Sprite leftArm, Sprite rightArm, float chance = 1f)
        {
            this.AddBody(new[] { new LayerAppearance(ColorLayer.Base, body) }, new[] { new LayerAppearance(ColorLayer.Colorable1, leftArm) }, new[] { new LayerAppearance(ColorLayer.Colorable2, rightArm) }, chance);
        }

        /// <summary>
        /// Adds a body to the appearance.
        /// </summary>
        /// <param name="bodyLayers">The layers that make up the body.</param>
        /// <param name="leftArmLayers">The layers that make up the left arm.</param>
        /// <param name="rightArmLayers">The layers that make up the right arm.</param>
        /// <param name="chance">The chance for this body to be selected.</param>
        public void AddBody(LayerAppearance[] bodyLayers, LayerAppearance[] leftArmLayers, LayerAppearance[] rightArmLayers, float chance = 1f)
        {
            var bodyBaseSet = new AppearancePart.ColorablePart[]
            {
                Array.Find(bodyLayers, l => l.Layer == ColorLayer.Base)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(bodyLayers, l => l.Layer == ColorLayer.Colorable1)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(bodyLayers, l => l.Layer == ColorLayer.Colorable2)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(bodyLayers, l => l.Layer == ColorLayer.Colorable3)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(bodyLayers, l => l.Layer == ColorLayer.Colorable4)?.ToColorablePart() ?? BlankColorablePart,
            };

            var handFrontSet = new AppearancePart.ColorablePart[]
            {
                Array.Find(rightArmLayers, l => l.Layer == ColorLayer.Base)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(rightArmLayers, l => l.Layer == ColorLayer.Colorable1)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(rightArmLayers, l => l.Layer == ColorLayer.Colorable2)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(rightArmLayers, l => l.Layer == ColorLayer.Colorable3)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(rightArmLayers, l => l.Layer == ColorLayer.Colorable4)?.ToColorablePart() ?? BlankColorablePart,
            };

            var handBackSet = new AppearancePart.ColorablePart[]
            {
                Array.Find(leftArmLayers, l => l.Layer == ColorLayer.Base)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(leftArmLayers, l => l.Layer == ColorLayer.Colorable1)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(leftArmLayers, l => l.Layer == ColorLayer.Colorable2)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(leftArmLayers, l => l.Layer == ColorLayer.Colorable3)?.ToColorablePart() ?? BlankColorablePart,
                Array.Find(leftArmLayers, l => l.Layer == ColorLayer.Colorable4)?.ToColorablePart() ?? BlankColorablePart,
            };

            var body = ScriptableObject.CreateInstance<Body>();
            body.bodyBase = bodyBaseSet;
            body.bodyBaseSkin = BlankColorablePart;
            body.handBack = handBackSet;
            body.handBackSkin = BlankColorablePart;
            body.handFront = handFrontSet;
            body.handFrontSkin = BlankColorablePart;

            var part = new PartContainer<Body>
            {
                part = body,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.body;

            // Remove our blank if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.bodyBase.Length == 1 && group.partsInGroup[0].part.bodyBase[0].background.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
            }
        }

        public void ClearFaces()
        {
            var face = ScriptableObject.CreateInstance<Face>();
            var blankEmotions = new[] { BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart };
            face.hair = blankEmotions;
            face.skin = blankEmotions;

            this.npcTemplate.appearance.face.partsInGroup = new[]
            {
                new PartContainer<Face>
                {
                    part = face,
                },
            };
        }

        public void AddFace(Sprite idle, float chance = 1f)
        {
            this.AddFace(new[] { new EmotionAppearance(Emotion.Idle, idle) }, chance);
        }

        public void AddFace(Sprite idle, Sprite positiveReaction, Sprite negativeReaction, float chance = 1f)
        {
            this.AddFace(new[]
            {
                new EmotionAppearance(Emotion.Idle, idle),
                new EmotionAppearance(Emotion.PositiveReaction, positiveReaction),
                new EmotionAppearance(Emotion.NegativeReaction, negativeReaction),
            }, chance);
        }

        public void AddFace(EmotionAppearance[] emotions, float chance = 1f)
        {
            var face = ScriptableObject.CreateInstance<Face>();
            var idleEmotion = Array.Find(emotions, e => e.Emotion == Emotion.Idle)?.ToColorablePart() ?? BlankColorablePart;
            face.hair = new[] {
                Array.Find(emotions, e => e.Emotion == Emotion.NegativeReaction)?.ToColorablePart() ?? idleEmotion,
                Array.Find(emotions, e => e.Emotion == Emotion.Anger2)?.ToColorablePart() ?? idleEmotion,
                Array.Find(emotions, e => e.Emotion == Emotion.Anger1)?.ToColorablePart() ?? idleEmotion,
                idleEmotion,
                Array.Find(emotions, e => e.Emotion == Emotion.Happy1)?.ToColorablePart() ?? idleEmotion,
                Array.Find(emotions, e => e.Emotion == Emotion.PositiveReaction)?.ToColorablePart() ?? idleEmotion,
            };

            var blankEmotions = new[] { BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart, BlankColorablePart };
            face.skin = blankEmotions;

            var part = new PartContainer<Face>
            {
                part = face,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.face;

            // Remove our blank if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.hair.Length >= 6 && group.partsInGroup[0].part.hair[3].contour.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
            }
        }

        public void ClearEyes()
        {
            var eyes = ScriptableObject.CreateInstance<Eyes>();
            eyes.left = BlankSprite;
            eyes.right = BlankSprite;

            this.npcTemplate.appearance.eyes.partsInGroup = new[]
            {
                new PartContainer<Eyes>
                {
                    part = eyes,
                },
            };
        }

        public void AddEyes(Sprite leftEye, Sprite rightEye, float chance = 1f)
        {
            var eyes = ScriptableObject.CreateInstance<Eyes>();
            eyes.left = BlankSprite;
            eyes.right = BlankSprite;

            var part = new PartContainer<Eyes>
            {
                part = eyes,
            };

            var group = this.npcTemplate.appearance.eyes;

            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.left.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
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
            var hairStyle = ScriptableObject.CreateInstance<Hairstyle>();
            hairStyle.back = BlankColorablePart;
            hairStyle.longFront = BlankColorablePart;
            hairStyle.middle = BlankColorablePart;
            hairStyle.shortFront = BlankColorablePart;

            var appearance = this.npcTemplate.appearance = new AppearanceContainer();

            // Set blanks into part groups that default to herbalist art.
            appearance.hairstyle.partsInGroup = new[]
            {
                new PartContainer<Hairstyle>
                {
                    part = hairStyle,
                },
            };

            this.ClearBodies();
            this.ClearHeadShapes();
            this.ClearFaces();
            this.ClearEyes();
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

        /// <summary>
        /// Defines the appearance of an emotion
        /// </summary>
        public class EmotionAppearance
        {
            public EmotionAppearance(Emotion emotion, Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                this.Emotion = emotion;
                this.Contour = contour;
                this.Background = background;
                this.Scratches = scratches;
            }

            public Emotion Emotion { get; set; }

            public Sprite Background { get; set; }

            public Sprite Contour { get; set; }

            public Sprite Scratches { get; set; }

            /// <summary>
            /// Creates a colorable part from this layer.
            /// </summary>
            /// <returns>The colorable part.</returns>
            internal AppearancePart.ColorablePart ToColorablePart()
            {
                return new AppearancePart.ColorablePart
                {
                    background = this.Background ?? BlankSprite,
                    contour = this.Contour ?? BlankSprite,
                    scratches = this.Scratches ?? BlankSprite,
                };
            }
        }

        /// <summary>
        /// Defines a sprite on a specified appearance layer.
        /// </summary>
        public class LayerAppearance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LayerAppearance"/> class.
            /// </summary>
            /// <param name="layer">The layer the sprite should be on.</param>
            /// <param name="background">The background sprite to use.</param>
            /// <param name="contour">The contour sprite to use.</param>
            /// <param name="scratches">The scratches sprite to use.</param>
            public LayerAppearance(ColorLayer layer, Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                this.Layer = layer;
                this.Background = background;
                this.Contour = contour;
                this.Scratches = scratches;
            }

            /// <summary>
            /// Gets or sets the layer this sprite should be on.
            /// </summary>
            public ColorLayer Layer { get; set; }

            /// <summary>
            /// Gets or sets the background sprite to use.
            /// </summary>
            public Sprite Background { get; set; }

            /// <summary>
            /// Gets or sets the contour sprite to use.
            /// </summary>
            public Sprite Contour { get; set; }

            /// <summary>
            /// Gets or sets the scratches sprite to use.
            /// </summary>
            public Sprite Scratches { get; set; }

            public static LayerAppearance Base(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new LayerAppearance(ColorLayer.Base, background, contour, scratches);
            }

            public static LayerAppearance Colorable1(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new LayerAppearance(ColorLayer.Colorable1, background, contour, scratches);
            }

            public static LayerAppearance Colorable2(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new LayerAppearance(ColorLayer.Colorable2, background, contour, scratches);
            }

            public static LayerAppearance Colorable3(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new LayerAppearance(ColorLayer.Colorable3, background, contour, scratches);
            }

            public static LayerAppearance Colorable4(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new LayerAppearance(ColorLayer.Colorable4, background, contour, scratches);
            }

            /// <summary>
            /// Creates a colorable part from this layer.
            /// </summary>
            /// <returns>The colorable part.</returns>
            internal AppearancePart.ColorablePart ToColorablePart()
            {
                return new AppearancePart.ColorablePart
                {
                    background = this.Background ?? BlankSprite,
                    contour = this.Contour ?? BlankSprite,
                    scratches = this.Scratches ?? BlankSprite,
                };
            }
        }
    }
}
