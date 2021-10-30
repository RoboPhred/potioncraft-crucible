namespace RoboPhredDev.PotionCraft.Crucible.Ingredients
{
    using System;
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI;
    using UnityEngine;

    /// <summary>
    /// A list of <see cref="CrucibleIngredientPathSegment"/>s parsable from an SVG Path.
    /// </summary>
    public class SvgPath : List<CrucibleIngredientPathSegment>
    {
        /// <summary>
        /// Parses an SVG Path into a list of <see cref="CrucibleIngredientPathSegment"/>s.
        /// </summary>
        /// <param name="path">The path to parse.</param>
        /// <returns>The list of path segments.</returns>
        public static SvgPath Parse(string path)
        {
            var curves = new SvgPath();
            var lastEnd = Vector2.zero;
            CrucibleIngredientPathSegment curve;
            while ((curve = PartToCurve(ref path, lastEnd)) != null)
            {
                // Filter out any M 0,0 that might have gotten in from path editors.
                if (curve.P1 == curve.P2 && curve.P1 == curve.End)
                {
                    continue;
                }

                if (curve.IsRelative)
                {
                    lastEnd += curve.End;
                }
                else
                {
                    lastEnd = curve.End;
                }

                curves.Add(curve);
            }

            return curves;
        }

        private static string GetToken(ref string svgPath)
        {
            var token = string.Empty;
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

                token += c;
            }

            svgPath = svgPath.Substring(i);

            if (token.Length == 0)
            {
                return null;
            }

            return token;
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
