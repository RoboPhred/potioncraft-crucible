namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using HarmonyLib;

    /// <summary>
    /// A static class storing the game api's harmony instance.
    /// </summary>
    internal static class HarmonyInstance
    {
        private static Harmony harmony;

        /// <summary>
        /// Gets the harmony instance.
        /// </summary>
        public static Harmony Instance
        {
            get
            {
                return harmony ??= new Harmony("RoboPhredDev.PotionCraft.Crucible.GameAPI");
            }
        }
    }
}
