// <copyright file="CrucibleLog.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Foobar is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BepInEx;

    /// <summary>
    /// Utilities for logging.
    /// </summary>
    public static class CrucibleLog
    {
        private static readonly Stack<string> LogScopeStack = new();

        /// <summary>
        /// Logs the given message.
        /// </summary>
        /// <remarks>
        /// Log messages will be associated with the BepInEx mod implemented in the currently active mod context, defaulting to the caller's assembly.
        /// </remarks>
        /// <param name="message">The message to log.</param>
        /// <param name="logParams">Parameters for the message.</param>
        public static void Log(string message, params object[] logParams)
        {
            var caller = Assembly.GetCallingAssembly();
            var scope = GetLogGUID(caller);

            WriteToLog(scope, message, logParams);
        }

        /// <summary>
        /// Logs the given message on behalf of the given mod name.
        /// </summary>
        /// <remarks>
        /// Log messages will be associated with the BepInEx mod implemented in the currently active mod context, defaulting to the caller's assembly.
        /// </remarks>
        /// <param name="modGUID">The GUID of the mod that logged this message.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="logParams">Parameters for the message.</param>
        public static void LogInScope(string modGUID, string message, params object[] logParams)
        {
            WriteToLog(modGUID, message, logParams);
        }

        /// <summary>
        /// Run the given action and associate all log messages to the given assembly's mod logs.
        /// </summary>
        /// <param name="modAssembly">The assembly to tag log messages as originating from.</param>
        /// <param name="action">The action to scope log messages for.</param>
        public static void RunInLogScope(Assembly modAssembly, Action action)
        {
            var guid = GetLogGUID(modAssembly);
            RunInLogScope(guid, action);
        }

        /// <summary>
        /// Run the given action and associate all log messages to the given mod name.
        /// </summary>
        /// <param name="modGUID">The name of the mod to tag logs with.</param>
        /// <param name="action">The action to scope log messages for.</param>
        public static void RunInLogScope(string modGUID, Action action)
        {
            LogScopeStack.Push(modGUID);
            try
            {
                action();
            }
            finally
            {
                LogScopeStack.Pop();
            }
        }

        private static void WriteToLog(string modGUID, string message, params object[] logParams)
        {
            UnityEngine.Debug.Log($"[{modGUID}]: " + string.Format(message, logParams));

            // Should we also log to a specific file for the given namespace?
        }

        private static string GetLogGUID(Assembly assembly)
        {
            if (LogScopeStack.Count > 0)
            {
                return LogScopeStack.Peek();
            }

            var pluginAttributes = from type in assembly.GetTypes()
                                   let attribute = (BepInPlugin)Attribute.GetCustomAttribute(type, typeof(BepInPlugin))
                                   where attribute != null
                                   select attribute;
            var bepAttr = pluginAttributes.FirstOrDefault();
            if (bepAttr != null)
            {
                return bepAttr.GUID;
            }

            return assembly.FullName;
        }
    }
}
