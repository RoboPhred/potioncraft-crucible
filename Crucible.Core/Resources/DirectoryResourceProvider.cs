namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    using System.IO;

    /// <summary>
    /// A resource provider that loads resources from within a directory.
    /// </summary>
    public class DirectoryResourceProvider : ICrucibleResourceProvider
    {
        private readonly string rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryResourceProvider"/> class.
        /// </summary>
        /// <param name="directory">The directory to pull resources from.</param>
        public DirectoryResourceProvider(string directory)
        {
            this.rootPath = Path.GetFullPath(directory);
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string resource)
        {
            var resourcePath = this.GetResourcePath(resource);
            return File.ReadAllBytes(resourcePath);
        }

        /// <inheritdoc/>
        public string ReadAllText(string resource)
        {
            var resourcePath = this.GetResourcePath(resource);
            return File.ReadAllText(resourcePath);
        }

        private string GetResourcePath(string resource)
        {
            var resourcePath = Path.GetFullPath(Path.Combine(this.rootPath, resource));
            if (!resourcePath.StartsWith(this.rootPath))
            {
                throw new CrucibleResourceException($"The request resource \"{resource}\" is outside of the mod directory \"{this.rootPath}\".")
                {
                    ResourceName = resource,
                };
            }

            return resourcePath;
        }
    }
}
