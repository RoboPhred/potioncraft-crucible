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
        private static readonly Sprite BlankSprite = SpriteUtilities.CreateBlankSprite(1, 1, Color.clear).WithName("Crucible cleared appearance sprite");

        private static readonly AppearancePart.ColorablePart BlankColorablePart = new()
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
            var blankColorablePartSet = Enumerable.Repeat(BlankColorablePart, 5).ToArray();

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
            this.AddBody(
                new[] { Layer.Base(body) },
                new[] { Layer.Base(leftArm) },
                new[] { Layer.Base(rightArm) },
                chance);
        }

        /// <summary>
        /// Adds a body to the appearance.
        /// </summary>
        /// <param name="bodyLayers">The layers that make up the body.</param>
        /// <param name="leftArmLayers">The layers that make up the left arm.</param>
        /// <param name="rightArmLayers">The layers that make up the right arm.</param>
        /// <param name="chance">The chance for this body to be selected.</param>
        public void AddBody(Layer[] bodyLayers, Layer[] leftArmLayers, Layer[] rightArmLayers, float chance = 1f)
        {
            var body = ScriptableObject.CreateInstance<Body>();

            body.bodyBase = Enumerable.Repeat(BlankColorablePart, 5).ToArray();
            body.bodyBaseSkin = BlankColorablePart;
            foreach (var bodyLayer in bodyLayers)
            {
                bodyLayer.Set(body.bodyBase);
            }

            body.handBack = Enumerable.Repeat(BlankColorablePart, 5).ToArray();
            body.handBackSkin = BlankColorablePart;
            foreach (var leftArmLayer in leftArmLayers)
            {
                leftArmLayer.Set(body.handBack);
            }

            body.handFront = Enumerable.Repeat(BlankColorablePart, 5).ToArray();
            body.handFrontSkin = BlankColorablePart;
            foreach (var rightArmLayer in rightArmLayers)
            {
                rightArmLayer.Set(body.handFront);
            }

            var part = new PartContainer<Body>
            {
                part = body,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.body;

            // Remove our blank if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.bodyBase.Length >= 1 && group.partsInGroup[0].part.bodyBase[0].background.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
            }
        }

        /// <summary>
        /// Clear all face data for this appearance.
        /// </summary>
        public void ClearFaces()
        {
            var face = ScriptableObject.CreateInstance<Face>();
            var blankEmotions = Enumerable.Repeat(BlankColorablePart, 6).ToArray();
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

        /// <summary>
        /// Add a face to this appearance.
        /// </summary>
        /// <param name="idle">The sprite to use for all emotions.</param>
        /// <param name="chance">The chance for this face to be selected.</param>
        public void AddFace(Sprite idle, float chance = 1f)
        {
            this.AddFace(new[] { Emotion.Idle(idle) }, chance);
        }

        /// <summary>
        /// Add a face to this appearance.
        /// </summary>
        /// <param name="idle">The sprite to use when the npc is idle, angry, or happy.</param>
        /// <param name="positiveReaction">The face to use when the npc is given a matching potion.</param>
        /// <param name="negativeReaction">The face to use when the npc is given a potion that does not match.</param>
        /// <param name="chance">The chance for this face to be selected.</param>
        public void AddFace(Sprite idle, Sprite positiveReaction, Sprite negativeReaction, float chance = 1f)
        {
            this.AddFace(
                new[]
                {
                    Emotion.Idle(idle),
                    Emotion.PositiveReaction(positiveReaction),
                    Emotion.NegativeReaction(negativeReaction),
                },
                chance);
        }

        /// <summary>
        /// Add a face to this appearance.
        /// </summary>
        /// <param name="emotions">The array of emotion elements to use for this face.</param>
        /// <param name="chance">The chance for this face to be selected.</param>
        public void AddFace(Emotion[] emotions, float chance = 1f)
        {
            var face = ScriptableObject.CreateInstance<Face>();
            face.hair = Enumerable.Repeat(BlankColorablePart, 6).ToArray();
            foreach (var emotion in emotions)
            {
                emotion.Set(face.hair);
            }

            // Set idle to missing entries.
            var idle = face.hair[3];
            if (idle.contour.name != BlankSprite.name)
            {
                for (var i = 0; i < face.hair.Length; i++)
                {
                    if (i != 3 && face.hair[i].contour.name == BlankSprite.name)
                    {
                        face.hair[i] = idle;
                    }
                }
            }

            face.skin = Enumerable.Repeat(BlankColorablePart, 6).ToArray();

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

        /// <summary>
        /// Clear all eyes from this appearance.
        /// </summary>
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

        /// <summary>
        /// Adds eyes to this appearance.
        /// </summary>
        /// <param name="leftEye">The sprite to use for the left eye.</param>
        /// <param name="rightEye">The sprite to use for the right eye.</param>
        /// <param name="chance">The chance for this eye pair to be selected.</param>
        public void AddEyes(Sprite leftEye, Sprite rightEye, float chance = 1f)
        {
            var eyes = ScriptableObject.CreateInstance<Eyes>();
            eyes.left = leftEye;
            eyes.right = rightEye;

            var part = new PartContainer<Eyes>
            {
                part = eyes,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.eyes;

            // Remove our blank placeholder if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.left.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
            }
        }

        /// <summary>
        /// Clears all hairstyles from this appearance.
        /// </summary>
        public void ClearHairStyles()
        {
            var hairStyle = ScriptableObject.CreateInstance<Hairstyle>();
            hairStyle.back = BlankColorablePart;
            hairStyle.longFront = BlankColorablePart;
            hairStyle.middle = BlankColorablePart;
            hairStyle.shortFront = BlankColorablePart;

            this.npcTemplate.appearance.hairstyle.partsInGroup = new[]
            {
                new PartContainer<Hairstyle>
                {
                    part = hairStyle,
                },
            };
        }

        /// <summary>
        /// Adds a hairstyle to this npc appearance.
        /// </summary>
        /// <param name="hairs">An array of hair parts to add.</param>
        /// <param name="chance">The chance for this hairstyle to be selected.</param>
        public void AddHairStyle(Hair[] hairs, float chance = 1)
        {
            var hairStyle = ScriptableObject.CreateInstance<Hairstyle>();

            // Hair isnt stored in an array, but the API is more convienent.
            var hairArray = Enumerable.Repeat(BlankColorablePart, 4).ToArray();
            foreach (var hair in hairs)
            {
                hair.Set(hairArray);
            }

            hairStyle.longFront = hairArray[0];
            hairStyle.middle = hairArray[1];
            hairStyle.shortFront = hairArray[2];
            hairStyle.back = hairArray[3];

            var part = new PartContainer<Hairstyle>
            {
                part = hairStyle,
                chanceBtwParts = chance,
            };

            var group = this.npcTemplate.appearance.hairstyle;

            // Remove our blank placeholder if present.
            if (group.partsInGroup.Length == 1 && group.partsInGroup[0].part.longFront.background.name == BlankSprite.name)
            {
                group.partsInGroup = new[] { part };
            }
            else
            {
                group.partsInGroup = group.partsInGroup.Concat(new[] { part }).ToArray();
            }
        }

        /// <summary>
        /// Clears out all appearance data.
        /// </summary>
        public void Clear()
        {
            this.npcTemplate.appearance = new AppearanceContainer();

            // Apply blank sprites to override the default herbalist art.
            this.ClearBodies();
            this.ClearHeadShapes();
            this.ClearFaces();
            this.ClearEyes();
            this.ClearHairStyles();
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
        /// Utility class to act as a setter for array based appearance data.
        /// </summary>
        public class AppearanceArraySetter
        {
            private readonly int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="AppearanceArraySetter"/> class.
            /// </summary>
            /// <param name="index">The index in the appearance data array to set.</param>
            internal AppearanceArraySetter(int index)
            {
                this.index = index;
            }

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

            /// <summary>
            /// Sets the appearance data in the array.
            /// </summary>
            /// <param name="parts">The array to set data for.</param>
            internal void Set(AppearancePart.ColorablePart[] parts)
            {
                if (parts.Length <= this.index)
                {
                    throw new InvalidOperationException("Part array is too small.");
                }

                parts[this.index] = new AppearancePart.ColorablePart
                {
                    background = this.Background ?? BlankSprite,
                    contour = this.Contour ?? BlankSprite,
                    scratches = this.Scratches ?? BlankSprite,
                };
            }
        }

        /// <summary>
        /// Defines the appearance of an emotion.
        /// </summary>
        public sealed class Emotion : AppearanceArraySetter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Emotion"/> class.
            /// </summary>
            /// <param name="index">The index in the appearance array to set sprites for.</param>
            internal Emotion(int index)
                : base(index)
            {
            }

            /// <summary>
            /// Create a negative reaction emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion NegativeReaction(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(0)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Create a very angry emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion Anger2(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(1)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Create a moderately angry emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion Anger1(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(2)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Create an idle emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion Idle(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(3)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Create a happy emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion Happy1(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(4)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Create a positive reaction emotion.
            /// </summary>
            /// <param name="contour">The contour sprite of the emotion.</param>
            /// <param name="background">The background sprite of the emotion.</param>
            /// <param name="scratches">The scratches sprite of the emotion.</param>
            /// <returns>An emotion to use in <see cref="CrucibleNpcAppearance.AddFace(Emotion[], float)"/>.</returns>
            public static Emotion PositiveReaction(Sprite contour, Sprite background = null, Sprite scratches = null)
            {
                return new Emotion(5)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }
        }

        /// <summary>
        /// Defines a sprite on a specified appearance layer.
        /// </summary>
        public class Layer : AppearanceArraySetter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Layer"/> class.
            /// </summary>
            /// <param name="index">The index in the appearance array to set.</param>
            internal Layer(int index)
                : base(index)
            {
            }

            /// <summary>
            /// Creates a sprite set for the base uncolored layer.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>A layer for use in <see cref="CrucibleNpcAppearance.AddBody(Layer[], Layer[], Layer[], float)"/>.</returns>
            public static Layer Base(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Layer(0)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a sprite set for the first colorable layer.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>A layer for use in <see cref="CrucibleNpcAppearance.AddBody(Layer[], Layer[], Layer[], float)"/>.</returns>
            public static Layer Colorable1(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Layer(1)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a sprite set for the second colorable layer.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>A layer for use in <see cref="CrucibleNpcAppearance.AddBody(Layer[], Layer[], Layer[], float)"/>.</returns>
            public static Layer Colorable2(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Layer(2)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a sprite set for the third colorable layer.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>A layer for use in <see cref="CrucibleNpcAppearance.AddBody(Layer[], Layer[], Layer[], float)"/>.</returns>
            public static Layer Colorable3(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Layer(3)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a sprite set for the fourth colorable layer.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>A layer for use in <see cref="CrucibleNpcAppearance.AddBody(Layer[], Layer[], Layer[], float)"/>.</returns>
            public static Layer Colorable4(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Layer(4)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }
        }

        /// <summary>
        /// A class for setting hair appearance data.
        /// </summary>
        public sealed class Hair : AppearanceArraySetter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Hair"/> class.
            /// </summary>
            /// <param name="index">The index in the hair array to set.</param>
            internal Hair(int index)
                : base(index)
            {
            }

            /// <summary>
            /// Creates a hair element for the front right hair segment.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>Hair data for use with <see cref="CrucibleNpcAppearance.AddHairStyle(Hair[], float)"/>.</returns>
            public static Hair Right(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Hair(0)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a hair element for the front middle hair segment.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>Hair data for use with <see cref="CrucibleNpcAppearance.AddHairStyle(Hair[], float)"/>.</returns>
            public static Hair Middle(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Hair(1)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a hair element for the front left hair segment.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>Hair data for use with <see cref="CrucibleNpcAppearance.AddHairStyle(Hair[], float)"/>.</returns>
            public static Hair Left(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Hair(2)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }

            /// <summary>
            /// Creates a hair element for the rear hair segment.
            /// </summary>
            /// <param name="background">The background sprite.</param>
            /// <param name="contour">The contour sprite.</param>
            /// <param name="scratches">The scratches sprite.</param>
            /// <returns>Hair data for use with <see cref="CrucibleNpcAppearance.AddHairStyle(Hair[], float)"/>.</returns>
            public static Hair Back(Sprite background, Sprite contour = null, Sprite scratches = null)
            {
                return new Hair(3)
                {
                    Background = background,
                    Contour = contour,
                    Scratches = scratches,
                };
            }
        }
    }
}
