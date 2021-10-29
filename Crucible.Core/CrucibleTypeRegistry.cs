namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A registry for obtaining types from attributes.
    /// </summary>
    public static class CrucibleTypeRegistry
    {
        private static bool initialized = false;

        private static Dictionary<Type, HashSet<Type>> typesByAttribute = null;

        /// <summary>
        /// Gets all types with the given attribute.
        /// </summary>
        /// <typeparam name="T">The attribute to fetch all types for.</typeparam>
        /// <returns>An enumerable of all discovered types with the given attribute.</returns>
        public static IEnumerable<Type> GetTypesByAttribute<T>()
        {
            return GetTypesByAttribute(typeof(T));
        }

        /// <summary>
        /// Gets all types with the given attribute.
        /// </summary>
        /// <param name="t">The attribute to fetch all types for.</typeparam>
        /// <returns>An enumerable of all discovered types with the given attribute.</returns>
        public static IEnumerable<Type> GetTypesByAttribute(Type t)
        {
            EnsureTypesLoaded();

            if (typesByAttribute.TryGetValue(t, out HashSet<Type> types))
            {
                return types.ToArray();
            }

            return new Type[0];
        }

        private static void EnsureTypesLoaded()
        {
            EnsureInitialized();

            if (typesByAttribute == null)
            {
                BuildTypesByAttribute();
            }
        }

        private static void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                // Clear the cache if a new assembly is loaded.
                typesByAttribute = null;
            };
        }

        private static void BuildTypesByAttribute()
        {
            var registryAttributeTypes = GetCrucibleRegistryAttributes().ToArray();

            // Search all loaded types for any type that has been marked with an attribute that is marked with CrucibleRegistryAttribute.
            var candidates = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             from typeAttribute in type.GetCustomAttributes(true)
                             from registryAttribute in
                                 from registryAttribute in registryAttributeTypes
                                 where registryAttribute.IsAssignableFrom(typeAttribute.GetType())
                                 select registryAttribute
                             group type by registryAttribute into attributesByType
                             select attributesByType;
            typesByAttribute = candidates.ToDictionary(x => x.Key, x => x.ToHashSet());
        }

        /// <summary>
        /// Gets all attributes that have been marked as a CrucibleRegistryAttribute.
        /// </summary>
        /// <returns>An enumerable of all attribute types found.</returns>
        private static IEnumerable<Type> GetCrucibleRegistryAttributes()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.GetCustomAttribute<CrucibleRegistryAttributeAttribute>() != null
                   select type;
        }
    }
}
