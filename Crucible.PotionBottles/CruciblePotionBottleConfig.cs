// <copyright file="CruciblePotionBottleConfig.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.PotionBottles
{
    using RoboPhredDev.PotionCraft.Crucible.CruciblePackages;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using RoboPhredDev.PotionCraft.Crucible.SVG;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration subject for a PotionCraft potion bottle.
    /// </summary>
    public class CruciblePotionBottleConfig : CruciblePackageConfigSubjectNode<CruciblePotionBottle>
    {
        /// <summary>
        /// Gets or sets the ID of this potion base.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the icon used to display the potion bottle in the bottle menu.
        /// </summary>
        public Sprite BottleIcon { get; set; }

        /// <summary>
        /// Gets or sets the foreground sprite for the bottle.
        /// </summary>
        public Sprite BottleForeground { get; set; }

        /// <summary>
        /// Gets or sets the mask for the bottle.
        /// </summary>
        public Sprite BottleMask { get; set; }

        /// <summary>
        /// Gets or sets the path that makes up the bottle's collision.
        /// </summary>
        public SvgPath BottleCollision { get; set; }

        /// <summary>
        /// Gets or sets the offset of the label.
        /// </summary>
        public Vector2? LabelOffset { get; set; }

        /// <summary>
        /// Gets or sets the main liquid sprite.
        /// </summary>
        public Sprite LiquidMain
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the second liquid sprite to use for two-effect potions.
        /// </summary>
        public Sprite Liquid2Of2
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for three-effect potions.
        /// </summary>
        public Sprite Liquid1Of3 { get; set; }

        /// <summary>
        /// Gets or sets the third liquid sprite to use for three-effect potions.
        /// </summary>
        public Sprite Liquid3Of3 { get; set; }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid1Of4 { get; set; }

        /// <summary>
        /// Gets or sets the third liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid3Of4 { get; set; }

        /// <summary>
        /// Gets or sets the fourth liquid sprite to use for four-effect potions.
        /// </summary>
        public Sprite Liquid4Of4 { get; set; }

        /// <summary>
        /// Gets or sets the first liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid1Of5 { get; set; }

        /// <summary>
        /// Gets or sets the second liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid2Of5 { get; set; }

        /// <summary>
        /// Gets or sets the fourth liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid4Of5 { get; set; }

        /// <summary>
        /// Gets or sets the fifth liquid sprite to use for five-effect potions.
        /// </summary>
        public Sprite Liquid5Of5 { get; set; }

        /// <inheritdoc/>
        protected override CruciblePotionBottle GetSubject()
        {
            var id = this.PackageMod.Namespace + "." + this.ID;

            return CruciblePotionBottle.GetPotionBottleById(id) ?? CruciblePotionBottle.CreatePotionBottle(id);
        }

        /// <inheritdoc/>
        protected override void OnApplyConfiguration(CruciblePotionBottle subject)
        {
            if (this.BottleIcon != null)
            {
                subject.BottleIcon = this.BottleIcon;
            }

            if (this.BottleForeground != null)
            {
                subject.BottleForeground = this.BottleForeground;
            }

            if (this.BottleMask != null)
            {
                subject.BottleMask = this.BottleMask;
            }

            if (this.LabelOffset.HasValue)
            {
                subject.LabelOffset = this.LabelOffset.Value;
            }

            if (this.BottleCollision != null)
            {
                subject.SetColliderPolygon(this.BottleCollision.ToPoints());
            }

            if (this.LiquidMain != null)
            {
                subject.LiquidMain = this.LiquidMain;
            }

            if (this.Liquid2Of2 != null)
            {
                subject.Liquid2Of2 = this.Liquid2Of2;
            }

            if (this.Liquid1Of3 != null)
            {
                subject.Liquid1Of3 = this.Liquid1Of3;
            }

            if (this.Liquid3Of3 != null)
            {
                subject.Liquid3Of3 = this.Liquid3Of3;
            }

            if (this.Liquid1Of4 != null)
            {
                subject.Liquid1Of4 = this.Liquid1Of4;
            }

            if (this.Liquid3Of4 != null)
            {
                subject.Liquid3Of4 = this.Liquid3Of4;
            }

            if (this.Liquid4Of4 != null)
            {
                subject.Liquid4Of4 = this.Liquid4Of4;
            }

            if (this.Liquid1Of5 != null)
            {
                subject.Liquid1Of5 = this.Liquid1Of5;
            }

            if (this.Liquid2Of5 != null)
            {
                subject.Liquid2Of5 = this.Liquid2Of5;
            }

            if (this.Liquid4Of5 != null)
            {
                subject.Liquid4Of5 = this.Liquid4Of5;
            }

            if (this.Liquid5Of5 != null)
            {
                subject.Liquid5Of5 = this.Liquid5Of5;
            }
        }
    }
}
