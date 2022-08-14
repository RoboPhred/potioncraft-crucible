// <copyright file="ExceptionUtils.cs" company="RoboPhredDev">
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
                sb.Append(ex.GetType().FullName);
                sb.Append(" ");
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
