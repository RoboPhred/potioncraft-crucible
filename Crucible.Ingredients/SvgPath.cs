// <copyright file="SvgPath.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// A list of <see cref="CrucibleIngredientPathSegment"/>s parsable from an SVG Path.
    /// </summary>
    public class SvgPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgPath"/> class.
        /// </summary>
        public SvgPath()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgPath"/> class.
        /// </summary>
        /// <param name="path">The SVG path data.</param>
        public SvgPath(string path)
        {
            this.Data = path;
        }

        /// <summary>
        /// Gets or sets the svg path data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the x axis scaling.
        /// </summary>
        public float ScaleX { get; set; } = 1;

        /// <summary>
        /// Gets or sets the y axis scaling.
        /// </summary>
        public float ScaleY { get; set; } = 1;

        /// <summary>
        /// Parses an SVG Path into a list of <see cref="CrucibleIngredientPathSegment"/>s.
        /// </summary>
        /// <returns>An enumerable of path segments derived from the svg path.</returns>
        public IEnumerable<CrucibleIngredientPathSegment> ToPathSegments()
        {
            var path = this.Data;
            var scale = new Vector2(this.ScaleX, this.ScaleY * -1);
            var lastEnd = Vector2.zero;
            CrucibleIngredientPathSegment curve;
            while ((curve = PartToCurve(ref path, lastEnd)) != null)
            {
                // Filter out any M 0,0 that might have gotten in from path editors.
                if (curve.P1 == curve.P2 && curve.P1 == curve.End)
                {
                    continue;
                }

                curve.P1 *= scale;
                curve.P2 *= scale;
                curve.End *= scale;

                if (curve.IsRelative)
                {
                    lastEnd += curve.End;
                }
                else
                {
                    lastEnd = curve.End;
                }

                yield return curve;
            }
        }

        private static string GetToken(ref string svgPath)
        {
            var token = new StringBuilder();
            int i = 0;
            bool? isAlphanumeric = null;
            for (; i < svgPath.Length; i++)
            {
                var c = svgPath[i];
                if (c == ' ' || c == ',' || c == '\n' || c == '\r')
                {
                    if (token.Length > 0)
                    {
                        break;
                    }

                    continue;
                }

                if (!isAlphanumeric.HasValue)
                {
                    isAlphanumeric = char.IsLetter(c);
                }
                else if (char.IsLetter(c) != isAlphanumeric.Value)
                {
                    break;
                }

                token.Append(c);
            }

            svgPath = svgPath.Substring(i);

            if (token.Length == 0)
            {
                return null;
            }

            return token.ToString();
        }

        private static float GetFloatTokenOrFail(ref string svgPath)
        {
            var token = GetToken(ref svgPath);
            if (!float.TryParse(token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
            {
                throw new Exception($"Failed to parse decimal value: \"{token}\".");
            }

            return result;
        }

        private static CrucibleIngredientPathSegment PartToCurve(ref string svgPath, Vector2 start)
        {
            var token = GetToken(ref svgPath);
            if (token == null)
            {
                return null;
            }

            return token switch
            {
                "M" or "L" => AbsoluteLine(ref svgPath),
                "H" => AbsoluteHorizontal(ref svgPath, start),
                "V" => AbsoluteVertical(ref svgPath, start),
                "C" => AbsoluteCubicCurve(ref svgPath),
                "m" or "l" => RelativeLine(ref svgPath),
                "v" => RelativeVertical(ref svgPath),
                "h" => RelativeHorizontal(ref svgPath),
                "c" => RelativeCubicCurve(ref svgPath),
                _ => throw new Exception($"Unknown SVG path command \"{token}\"."),
            };
        }

        private static CrucibleIngredientPathSegment AbsoluteLine(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static CrucibleIngredientPathSegment RelativeLine(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static CrucibleIngredientPathSegment AbsoluteHorizontal(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), start.y);
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static CrucibleIngredientPathSegment RelativeHorizontal(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), 0);
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static CrucibleIngredientPathSegment AbsoluteVertical(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(start.x, GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static CrucibleIngredientPathSegment RelativeVertical(ref string svgPath)
        {
            var end = new Vector2(0, GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static CrucibleIngredientPathSegment AbsoluteCubicCurve(ref string svgPath)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.CurveTo(p1, p2, end);
        }

        private static CrucibleIngredientPathSegment RelativeCubicCurve(ref string svgPath)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeCurveTo(p1, p2, end);
        }
    }
}
