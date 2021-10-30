namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    /// <summary>
    /// Defines an interface for accessing mod resources.
    /// </summary>
    public interface ICrucibleResourceProvider
    {
        /// <summary>
        /// Reads all text from a text resource.
        /// </summary>
        /// /// <param name="resourceName">The name of the resource to read.</param>
        /// <returns>All text contained in the resource.</returns>
        string ReadAllText(string resourceName);

        /// <summary>
        /// Reads all bytes from a binary resource.
        /// </summary>
        /// <param name="resourceName">The resource to read.</param>
        /// <returns>The read bytes.</returns>
        byte[] ReadAllBytes(string resourceName);
    }
}
