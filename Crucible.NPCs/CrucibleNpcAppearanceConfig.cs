// <copyright file="CrucibleNpcAppearanceConfig.cs" company="RoboPhredDev">
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
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using UnityEngine;

    /// <summary>
    /// Configuration for the appearance of an NPC.
    /// </summary>
    public class CrucibleNpcAppearanceConfig
    {
        private static readonly Sprite BlankSprite = SpriteUtilities.CreateBlankSprite(1, 1, Color.clear).WithName("Crucible unset appearance sprite");

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's head.
        /// </summary>
        public OneOrMany<HeadShapeConfig> HeadShape { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's hair.
        /// </summary>
        public OneOrMany<HairStyleConfig> HairStyle { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's face.
        /// </summary>
        public OneOrMany<FaceConfig> Face { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's eyes.
        /// </summary>
        public OneOrMany<EyesConfig> Eyes { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's beard.
        /// </summary>
        public OneOrMany<BeardConfig> Beard { get; set; }

        /// <summary>
        /// Gets or sets the configuration for the appearance of the NPC's body.
        /// </summary>
        public OneOrMany<BodyConfig> Body { get; set; }

        /// <summary>
        /// Apply the appearance configuration to the given NPC.
        /// </summary>
        /// <param name="npc">The NPC to apply the configuration to.</param>
        public void ApplyAppearance(CrucibleNpcTemplate npc)
        {
            if (this.HeadShape != null)
            {
                npc.Appearance.ClearHeadShapes();
                foreach (var shape in this.HeadShape)
                {
                    shape.Apply(npc);
                }
            }

            if (this.HairStyle != null)
            {
                npc.Appearance.ClearHairStyles();
                foreach (var style in this.HairStyle)
                {
                    style.Apply(npc);
                }
            }

            if (this.Face != null)
            {
                npc.Appearance.ClearFaces();
                foreach (var face in this.Face)
                {
                    face.Apply(npc);
                }
            }

            if (this.Eyes != null)
            {
                npc.Appearance.ClearEyes();
                foreach (var eyes in this.Eyes)
                {
                    eyes.Apply(npc);
                }
            }

            if (this.Beard != null)
            {
                npc.Appearance.ClearBeards();
                foreach (var beard in this.Beard)
                {
                    beard.Apply(npc);
                }
            }

            if (this.Body != null)
            {
                npc.Appearance.ClearBodies();
                foreach (var bodyConfig in this.Body)
                {
                    bodyConfig.Apply(npc);
                }
            }
        }

        public class HeadShapeConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite Mask { get; set; }

            public Sprite Background { get; set; }

            public Sprite Contour { get; set; }

            public Sprite Scratches { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                npc.Appearance.AddHeadShape(this.Mask, this.Background, this.Contour, this.Scratches, this.Chance);
            }
        }

        public class BodyConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite Torso { get; set; }

            public Sprite LeftArm { get; set; }

            public Sprite RightArm { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                npc.Appearance.AddBody(this.Torso ?? BlankSprite, this.LeftArm ?? BlankSprite, this.RightArm ?? BlankSprite, this.Chance);
            }
        }

        public class FaceConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite PositiveReaction { get; set; }

            public Sprite NegativeReaction { get; set; }

            public Sprite Idle { get; set; }

            public Sprite Anger1 { get; set; }

            public Sprite Anger2 { get; set; }

            public Sprite Happy1 { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                var emotions = new List<CrucibleNpcAppearance.Emotion>();
                if (this.PositiveReaction)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.PositiveReaction(this.PositiveReaction));
                }

                if (this.NegativeReaction)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.NegativeReaction(this.NegativeReaction));
                }

                if (this.Idle)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.Idle(this.Idle));
                }

                if (this.Anger1)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.Anger1(this.Anger1));
                }

                if (this.Anger2)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.Anger2(this.Anger2));
                }

                if (this.Happy1)
                {
                    emotions.Add(CrucibleNpcAppearance.Emotion.Happy1(this.Happy1));
                }

                npc.Appearance.AddFace(emotions.ToArray(), this.Chance);
            }
        }

        public class EyesConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite LeftEye { get; set; }

            public Sprite RightEye { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                npc.Appearance.AddEyes(this.LeftEye ?? BlankSprite, this.RightEye ?? BlankSprite, this.Chance);
            }
        }

        public class HairStyleConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite FrontLeft { get; set; }

            public Sprite FrontRight { get; set; }

            public Sprite Back { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                var hairs = new List<CrucibleNpcAppearance.Hair>();

                if (this.FrontLeft != null)
                {
                    hairs.Add(CrucibleNpcAppearance.Hair.Left(this.FrontLeft));
                }

                if (this.FrontRight != null)
                {
                    hairs.Add(CrucibleNpcAppearance.Hair.Right(this.FrontRight));
                }

                if (this.Back != null)
                {
                    hairs.Add(CrucibleNpcAppearance.Hair.Back(this.Back));
                }

                npc.Appearance.AddHairStyle(hairs.ToArray(), this.Chance);
            }
        }

        public class BeardConfig
        {
            public float Chance { get; set; } = 1f;

            public Sprite Background { get; set; }

            public Sprite Contour { get; set; }

            public Sprite Scratches { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                npc.Appearance.AddBeard(this.Background ?? BlankSprite, this.Contour ?? BlankSprite, this.Scratches ?? BlankSprite, this.Chance);
            }
        }
    }
}
