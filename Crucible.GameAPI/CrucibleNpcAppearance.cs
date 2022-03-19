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
            this.AddBody(new[] { new AppearanceLayer(ColorLayer.Base, body) }, new[] { new AppearanceLayer(ColorLayer.Colorable1, leftArm) }, new[] { new AppearanceLayer(ColorLayer.Colorable2, rightArm) }, chance);
        }

        /// <summary>
        /// Adds a body to the appearance.
        /// </summary>
        /// <param name="bodyLayers">The layers that make up the body.</param>
        /// <param name="leftArmLayers">The layers that make up the left arm.</param>
        /// <param name="rightArmLayers">The layers that make up the right arm.</param>
        /// <param name="chance">The chance for this body to be selected.</param>
        public void AddBody(AppearanceLayer[] bodyLayers, AppearanceLayer[] leftArmLayers, AppearanceLayer[] rightArmLayers, float chance = 1f)
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

            var appearance = this.npcTemplate.appearance = new AppearanceContainer();

            // Set blanks into part groups that default to herbalist art.
            appearance.eyes.partsInGroup = new[]
            {
                new PartContainer<Eyes>
                {
                    part = eyes,
                },
            };
            appearance.face.partsInGroup = new[]
            {
                new PartContainer<Face>
                {
                    part = face,
                },
            };
            appearance.hairstyle.partsInGroup = new[]
            {
                new PartContainer<Hairstyle>
                {
                    part = hairStyle,
                },
            };

            this.ClearBodies();
            this.ClearHeadShapes();
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
        /// Defines a sprite on a specified appearance layer.
        /// </summary>
        public class AppearanceLayer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AppearanceLayer"/> class.
            /// </summary>
            /// <param name="layer">The layer the sprite should be on.</param>
            /// <param name="background">The background sprite to use.</param>
            /// <param name="contour">The contour sprite to use.</param>
            /// <param name="scratches">The scratches sprite to use.</param>
            public AppearanceLayer(ColorLayer layer, Sprite background, Sprite contour = null, Sprite scratches = null)
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

            /// <summary>
            /// Creates a colorable part from this layer.
            /// </summary>
            /// <returns>The colorable part.</returns>
            internal AppearancePart.ColorablePart ToColorablePart()
            {
                return new AppearancePart.ColorablePart
                {
                    background = this.Background,
                    contour = this.Contour,
                    scratches = this.Scratches,
                };
            }
        }
    }
}
