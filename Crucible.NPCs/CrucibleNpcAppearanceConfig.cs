using System.Collections.Generic;
using RoboPhredDev.PotionCraft.Crucible.GameAPI;
using RoboPhredDev.PotionCraft.Crucible.Yaml;
using UnityEngine;

namespace RoboPhredDev.PotionCraft.Crucible.NPCs
{
    public class CrucibleNpcAppearanceConfig
    {
        private static readonly Sprite BlankSprite = SpriteUtilities.CreateBlankSprite(1, 1, Color.clear).WithName("Crucible unset appearance sprite");

        public string CopyFrom { get; set; }

        public OneOrMany<HeadShapeConfig> HeadShape { get; set; }

        public OneOrMany<FaceConfig> Face { get; set; }

        public OneOrMany<EyesConfig> Eyes { get; set; }

        public OneOrMany<BodyConfig> Body { get; set; }

        public void ApplyAppearance(CrucibleNpcTemplate npc)
        {
            npc.Appearance.Clear();

            if (!string.IsNullOrEmpty(this.CopyFrom))
            {
                var template = CrucibleNpcTemplate.GetNpcTemplateById(this.CopyFrom);
                if (template == null)
                {
                    CrucibleLog.Log($"Could not apply \"copyAppearanceFrom\" for customer ID \"{npc.ID}\" because no NPC template with an ID of \"{this.CopyFrom}\" was found.");
                }
                else
                {
                    npc.Appearance.CopyFrom(template);
                }
            }

            if (this.HeadShape != null)
            {
                foreach (var shape in this.HeadShape)
                {
                    shape.Apply(npc);
                }
            }

            if (this.Face != null)
            {
                foreach (var face in this.Face)
                {
                    face.Apply(npc);
                }
            }

            if (this.Eyes != null)
            {
                foreach (var eyes in this.Eyes)
                {
                    eyes.Apply(npc);
                }
            }

            if (this.Body != null)
            {
                foreach (var bodyConfig in this.Body)
                {
                    bodyConfig.Apply(npc);
                }
            }
        }

        public class HeadShapeConfig
        {
            public float Chance = 1f;

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
            public float Chance = 1f;

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
            public float Chance = 1f;

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
            public float Chance = 1f;

            public Sprite LeftEye { get; set; }
            public Sprite RightEye { get; set; }

            public void Apply(CrucibleNpcTemplate npc)
            {
                npc.Appearance.AddEyes(this.LeftEye ?? BlankSprite, this.RightEye ?? BlankSprite, this.Chance);
            }
        }
    }
}