namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    using System;
    using System.Collections.Generic;

    public static class CrucibleResources
    {
        private static readonly Stack<ICrucibleResourceProvider> resourceProviderStack = new();

        public static ICrucibleResourceProvider CurrentResource
        {
            get
            {
                if (resourceProviderStack.Count == 0)
                {
                    return null;
                }

                return resourceProviderStack.Peek();
            }
        }

        public static void WithResourceProvider(ICrucibleResourceProvider provider, Action action)
        {
            resourceProviderStack.Push(provider);
            try
            {
                action();
            }
            finally
            {
                resourceProviderStack.Pop();
            }
        }

        public static T WithResourceProvider<T>(ICrucibleResourceProvider provider, Func<T> action)
        {
            resourceProviderStack.Push(provider);
            try
            {
                return action();
            }
            finally
            {
                resourceProviderStack.Pop();
            }
        }
    }
}
