// <copyright file="YamlExceptionExtensions.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using System;
    using YamlDotNet.Core;

    /// <summary>
    /// Utilities for <see cref="YamlException"/>.
    /// </summary>
    public static class YamlExceptionExtensions
    {
        /// <summary>
        /// Gets the innermost non-YamlException message.
        /// </summary>
        /// <param name="exception">The exception to extract the message of.</param>
        /// <returns>The innermost non-YamlException message.</returns>
        public static string GetInnermostMessage(this YamlException exception)
        {
            Exception ex = exception;
            while (ex is YamlException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex.Message;
        }
    }
}
