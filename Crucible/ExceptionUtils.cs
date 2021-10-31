namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Text;

    /// <summary>
    /// Utilities for working with exceptions.
    /// </summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// Generates a string containing all exception messages traced through inner exceptions.
        /// </summary>
        /// <param name="ex">The exception to stringify.</param>
        /// <returns>A string containing all exception messages, plus the stack trace of the innermost exception.</returns>
        public static string ToExpandedString(this Exception ex)
        {
            var sb = new StringBuilder();
            while (true)
            {
                sb.AppendLine(ex.Message);
                if (ex.InnerException == null)
                {
                    sb.AppendLine(ex.StackTrace);
                    break;
                }

                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
