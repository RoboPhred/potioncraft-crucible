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
    using System.Linq;
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

            // This is the scale to apply to each path command.
            // Flip the y axis, as standard SVG paths are top-down, while game paths are bottom-up.
            var scale = new Vector2(this.ScaleX, this.ScaleY * -1);

            var lastEnd = Vector2.zero;
            CrucibleIngredientPathSegment curve;
            while ((curve = PartToCurve(ref path, lastEnd)) != null)
            {
                // Apply the scale
                curve.P1 *= scale;
                curve.P2 *= scale;
                curve.End *= scale;

                // Keep track of where we ended, so PartToCurve can calculate relative commands.
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

        /// <summary>
        /// Returns a list of points describing the path.
        /// </summary>
        /// <returns>A list of points describing the path.</returns>
        public List<Vector2> ToPoints()
        {
            var path = this.Data;

            var partStart = Vector2.zero;
            var figureStart = Vector2.zero;
            Vector2[] partPoints;
            var result = new List<Vector2>();
            var isClosed = true;
            while ((partPoints = PartToPoints(ref path, ref partStart, ref figureStart)) != null)
            {
                if (partPoints.Length == 0)
                {
                    // Detect when we close the figure.
                    isClosed = true;
                }
                else if (isClosed)
                {
                    // If we are starting a new figure, add the figure start position.
                    isClosed = false;
                    result.Add(figureStart);
                }

                result.AddRange(partPoints);
            }


            // This is the scale to apply to each path command.
            // Flip the y axis, as standard SVG paths are top-down, while game paths are bottom-up.
            var scale = new Vector2(this.ScaleX, this.ScaleY * -1);

            return result.ConvertAll(x => x * scale);
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

        private static Vector2[] PartToPoints(ref string svgPath, ref Vector2 partStart, ref Vector2 figureStart)
        {
            var token = GetToken(ref svgPath);
            if (token == null)
            {
                return null;
            }

            return token switch
            {
                "M" => AbsoluteMoveToPoints(ref svgPath, ref partStart, ref figureStart),
                "L" => UpdatePartStart(AbsoluteLineToPoints(ref svgPath), ref partStart),
                "H" => UpdatePartStart(RelativeLineToPoints(ref svgPath, partStart), ref partStart),
                "V" => UpdatePartStart(AbsoluteVerticalToPoints(ref svgPath, partStart), ref partStart),
                "C" => UpdatePartStart(AbsoluteCubicCurveToPoints(ref svgPath), ref partStart),
                "m" => RelativeMoveToPoints(ref svgPath, ref partStart, ref figureStart),
                "l" => UpdatePartStart(RelativeLineToPoints(ref svgPath, partStart), ref partStart),
                "v" => UpdatePartStart(RelativeVerticalToPoints(ref svgPath, partStart), ref partStart),
                "h" => UpdatePartStart(RelativeHorizontalToPoints(ref svgPath, partStart), ref partStart),
                "c" => UpdatePartStart(RelativeCubicCurveToPoints(ref svgPath, partStart), ref partStart),

                // FIXME: Go to start of last figure.
                "Z" or "z" => ClosePoints(ref svgPath, ref partStart, ref figureStart),
                _ => throw new Exception($"Unsupported SVG path command \"{token}\"."),
            };
        }

        private static Vector2[] UpdatePartStart(Vector2[] result, ref Vector2 partStart)
        {
            partStart = result.Last();
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
                "M" or "L" => AbsoluteLineToCurve(ref svgPath),
                "H" => AbsoluteHorizontalToCurve(ref svgPath, start),
                "V" => AbsoluteVerticalToCurve(ref svgPath, start),
                "C" => AbsoluteCubicCurveToCurve(ref svgPath),
                "m" or "l" => RelativeLineToCurve(ref svgPath),
                "v" => RelativeVerticalToCurve(ref svgPath),
                "h" => RelativeHorizontalToCurve(ref svgPath),
                "c" => RelativeCubicCurveToCurve(ref svgPath),
                _ => throw new Exception($"Unsupported SVG path command \"{token}\"."),
            };
        }

        private static Vector2[] AbsoluteMoveToPoints(ref string svgPath, ref Vector2 partStart, ref Vector2 figureStart)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            partStart = p1;
            figureStart = p1;
            return new Vector2[0];
        }

        private static Vector2[] RelativeMoveToPoints(ref string svgPath, ref Vector2 partStart, ref Vector2 figureStart)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            partStart += p1;
            figureStart = partStart;
            return new Vector2[0];
        }

        private static CrucibleIngredientPathSegment AbsoluteLineToCurve(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static Vector2[] AbsoluteLineToPoints(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return new[] { end };
        }

        private static CrucibleIngredientPathSegment RelativeLineToCurve(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static Vector2[] RelativeLineToPoints(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath) + start.x, GetFloatTokenOrFail(ref svgPath) + start.y);
            return new[] { end };
        }

        private static CrucibleIngredientPathSegment AbsoluteHorizontalToCurve(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), start.y);
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static CrucibleIngredientPathSegment RelativeHorizontalToCurve(ref string svgPath)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), 0);
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static Vector2[] RelativeHorizontalToPoints(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath) + start.x, start.y);
            return new[] { end };
        }

        private static CrucibleIngredientPathSegment AbsoluteVerticalToCurve(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(start.x, GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.LineTo(end);
        }

        private static Vector2[] AbsoluteVerticalToPoints(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(start.x, GetFloatTokenOrFail(ref svgPath));
            return new[] { end };
        }

        private static CrucibleIngredientPathSegment RelativeVerticalToCurve(ref string svgPath)
        {
            var end = new Vector2(0, GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeLineTo(end);
        }

        private static Vector2[] RelativeVerticalToPoints(ref string svgPath, Vector2 start)
        {
            var end = new Vector2(start.x, GetFloatTokenOrFail(ref svgPath) + start.y);
            return new[] { end };
        }

        private static CrucibleIngredientPathSegment AbsoluteCubicCurveToCurve(ref string svgPath)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.CurveTo(p1, p2, end);
        }

        private static Vector2[] AbsoluteCubicCurveToPoints(ref string svgPath)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));

            // FIXME: Break the cubic curve into line segments.
            return new[] { p1, p2, end };
        }

        private static CrucibleIngredientPathSegment RelativeCubicCurveToCurve(ref string svgPath)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath), GetFloatTokenOrFail(ref svgPath));
            return CrucibleIngredientPathSegment.RelativeCurveTo(p1, p2, end);
        }

        private static Vector2[] RelativeCubicCurveToPoints(ref string svgPath, Vector2 start)
        {
            var p1 = new Vector2(GetFloatTokenOrFail(ref svgPath) + start.x, GetFloatTokenOrFail(ref svgPath) + start.y);
            var p2 = new Vector2(GetFloatTokenOrFail(ref svgPath) + start.x, GetFloatTokenOrFail(ref svgPath) + start.y);
            var end = new Vector2(GetFloatTokenOrFail(ref svgPath) + start.x, GetFloatTokenOrFail(ref svgPath) + start.y);

            // FIXME: Break the cubic curve into line segments.
            return new[] { p1, p2, end };
        }

        private static Vector2[] ClosePoints(ref string svgPath, ref Vector2 partStart, ref Vector2 figureStart)
        {
            partStart = figureStart;
            return new[] { figureStart };
        }
    }
}
