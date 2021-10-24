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
