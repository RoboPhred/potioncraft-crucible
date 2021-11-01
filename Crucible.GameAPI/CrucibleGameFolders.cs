namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Provides common game folder locations.
    /// </summary>
    public static class CrucibleGameFolders
    {
        // Untested, and neither the potioncraft executable nor the assembly dll have any company or product name specified.
        /*
        /// <summary>
        /// Gets the path to the folder containing user data, such as logs and saves.
        /// </summary>
        public static string GameUserDataPath
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", versionInfo.CompanyName, versionInfo.ProductName);
            }
        }
        */
    }
}
