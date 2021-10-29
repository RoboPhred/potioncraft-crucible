namespace RoboPhredDev.PotionCraft.Crucible.GameAPI
{
    using System;
    using RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks;

    /// <summary>
    /// A static class exposing various static game events.
    /// </summary>
    public static class CrucibleGameEvents
    {
        /// <summary>
        /// Raised when the game has finished loading.
        /// </summary>
        /// <remarks>
        /// This is called once the game has finished loading and the user is at the main menu.
        /// It is generally not safe to try to access the game before this event, as game objects that make up critical game components
        /// might not yet be loaded.
        /// </remarks>
        public static event EventHandler OnGameLoaded
        {
            add
            {
                RecipeMapObjectAwakeEvent.OnRecipeMapObjectAwake += value;
            }

            remove
            {
                RecipeMapObjectAwakeEvent.OnRecipeMapObjectAwake -= value;
            }
        }
    }
}
