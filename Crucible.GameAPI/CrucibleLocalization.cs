namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// Tools for registering keys and values in PotionCraft's localization system.
    /// </summary>
    public static class CrucibleLocalization
    {
        private static readonly Dictionary<string, string> Localization = new();
        private static bool initialized = false;

        /// <summary>
        /// Sets the value of a localization key.
        /// If the key is already registered by the game or another mod, it will be replaced.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to use for all languages.</param>
        public static void SetLocalizationKey(string key, string value)
        {
            TryInitialize();
            Localization[key] = value;
        }

        private static void TryInitialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            KeyGetTextEvent.OnGetText += (_, e) =>
            {
                if (Localization.TryGetValue(e.Key, out var value))
                {
                    e.Result = value;
                }
            };
        }
    }
}
