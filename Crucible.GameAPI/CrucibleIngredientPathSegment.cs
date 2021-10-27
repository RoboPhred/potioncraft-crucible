namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using UnityEngine;
    using Utils.BezierCurves;

    /// <summary>
    /// Represents a path segment in the form of a cubic bezier curve.
    /// </summary>
    public class CrucibleIngredientPathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleIngredientPathSegment"/> class.
        /// </summary>
        /// <remarks>
        /// The created segment will be a straight line to the end point.
        /// </remarks>
        /// <param name="end">The end point of the line.</param>
        public CrucibleIngredientPathSegment(Vector2 end)
        {
            this.P1 = end;
            this.P2 = end;
            this.End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleIngredientPathSegment"/> class.
        /// </summary>
        /// <remarks>
        /// The created segment will be a straight line to the end point.
        /// </remarks>
        /// <param name="end">The end point of the line.</param>
        /// <param name="isRelative">Whether the coordinates given are relative to the previous path segment.</param>
        public CrucibleIngredientPathSegment(Vector2 end, bool isRelative)
        {
            this.P1 = end;
            this.P2 = end;
            this.End = end;
            this.IsRelative = isRelative;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleIngredientPathSegment"/> class.
        /// </summary>
        /// <param name="p1">Control point 1.</param>
        /// <param name="p2">Control point 2.</param>
        /// <param name="endpoint">The end point of the curve.</param>
        public CrucibleIngredientPathSegment(Vector2 p1, Vector2 p2, Vector2 endpoint)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.End = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleIngredientPathSegment"/> class.
        /// </summary>
        /// <param name="p1">Control point 1.</param>
        /// <param name="p2">Control point 2.</param>
        /// <param name="endpoint">The end point of the curve.</param>
        /// <param name="isRelative">Whether the coordinates given are relative to the previous path segment.</param>
        public CrucibleIngredientPathSegment(Vector2 p1, Vector2 p2, Vector2 endpoint, bool isRelative)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.End = endpoint;
            this.IsRelative = isRelative;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment's points are relative to the previous segment.
        /// </summary>
        public bool IsRelative { get; set; } = false;

        /// <summary>
        /// Gets or sets the first control point for this cubic bezier curve segment.
        /// </summary>
        public Vector2 P1 { get; set; } = Vector2.zero;

        /// <summary>
        /// Gets or sets the second control point for this cubic bezier curve segment.
        /// </summary>
        public Vector2 P2 { get; set; } = Vector2.zero;

        /// <summary>
        /// Gets or sets the end point for this cubic bezier curve segment.
        /// </summary>
        public Vector2 End { get; set; } = Vector2.zero;

        public static CrucibleIngredientPathSegment LineTo(Vector2 end)
        {
            return new CrucibleIngredientPathSegment(end);
        }

        public static CrucibleIngredientPathSegment LineTo(float x, float y)
        {
            return new CrucibleIngredientPathSegment(new Vector2(x, y));
        }

        public static CrucibleIngredientPathSegment RelativeLineTo(Vector2 end)
        {
            return new CrucibleIngredientPathSegment(end, true);
        }

        public static CrucibleIngredientPathSegment RelativeLineTo(float x, float y)
        {
            return new CrucibleIngredientPathSegment(new Vector2(x, y), true);
        }

        public static CrucibleIngredientPathSegment CurveTo(Vector2 p1, Vector2 p2, Vector2 endpoint)
        {
            return new CrucibleIngredientPathSegment(p1, p2, endpoint);
        }

        public static CrucibleIngredientPathSegment RelativeCurveTo(Vector2 p1, Vector2 p2, Vector2 endpoint)
        {
            return new CrucibleIngredientPathSegment(p1, p2, endpoint, true);
        }

        internal static CrucibleIngredientPathSegment FromPotioncraftCurve(CubicBezierCurve curve)
        {
            return new CrucibleIngredientPathSegment(curve.P1, curve.P2, curve.PLast);
        }

        /// <summary>
        /// Generates a potioncraft cubic bezier curve from this segment given a starting position.
        /// </summary>
        /// <param name="start">The starting position of this segment.</param>
        /// <returns>A <see cref="CubicBezierCurve"/> generated from this path segment.</returns>
        internal CubicBezierCurve ToPotioncraftCurve(Vector2 start)
        {
            var p1 = this.P1;
            var p2 = this.P2;
            var end = this.End;
            if (this.IsRelative)
            {
                p1 += start;
                p2 += start;
                end += start;
            }

            return new CubicBezierCurve(start, p1, p2, end);
        }
    }
}
