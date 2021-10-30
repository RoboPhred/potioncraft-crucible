namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Static class for accessing the current active resource provider.
    /// </summary>
    public static class CrucibleResources
    {
        private static readonly Stack<ICrucibleResourceProvider> ResourceProviderStack = new();

        /// <summary>
        /// Gets the current active resource provider.
        /// </summary>
        public static ICrucibleResourceProvider CurrentResourceProvider
        {
            get
            {
                if (ResourceProviderStack.Count == 0)
                {
                    return null;
                }

                return ResourceProviderStack.Peek();
            }
        }

        /// <summary>
        /// Reads all text from a text resource.
        /// </summary>
        /// /// <param name="resourceName">The name of the resource to read.</param>
        /// <returns>All text contained in the resource.</returns>
        public static string ReadAllText(string resourceName)
        {
            if (CurrentResourceProvider == null)
            {
                throw new CrucibleResourceException($"No resource provider is active.")
                {
                    ResourceName = resourceName,
                };
            }

            return CurrentResourceProvider.ReadAllText(resourceName);
        }

        /// <summary>
        /// Reads all bytes from a binary resource.
        /// </summary>
        /// <param name="resourceName">The resource to read.</param>
        /// <returns>The read bytes.</returns>
        public static byte[] ReadAllBytes(string resourceName)
        {
            if (CurrentResourceProvider == null)
            {
                throw new CrucibleResourceException($"No resource provider is active.")
                {
                    ResourceName = resourceName,
                };
            }

            return CurrentResourceProvider.ReadAllBytes(resourceName);
        }

        /// <summary>
        /// Runs the action with the given resource provider as the active resource provider.
        /// </summary>
        /// <param name="provider">The resource provider to make active during the operation.</param>
        /// <param name="action">The action to run while the resource provider is active.</param>
        public static void WithResourceProvider(ICrucibleResourceProvider provider, Action action)
        {
            ResourceProviderStack.Push(provider);
            try
            {
                action();
            }
            finally
            {
                ResourceProviderStack.Pop();
            }
        }

        /// <summary>
        /// Runs the action with the given resource provider as the active resource provider.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="provider">The resource provider to make active during the operation.</param>
        /// <param name="action">The action to run while the resource provider is active.</param>
        /// <returns>The result of the action.</returns>
        public static T WithResourceProvider<T>(ICrucibleResourceProvider provider, Func<T> action)
        {
            ResourceProviderStack.Push(provider);
            try
            {
                return action();
            }
            finally
            {
                ResourceProviderStack.Pop();
            }
        }
    }
}
