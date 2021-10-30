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
            if (typeof(Attribute).IsAssignableFrom(t))
            {
                throw new ArgumentException("The provided type must be an attribute.", nameof(t));
            }

            // Note: Originally required attributes to specify the searchable attributes so we can scan all types just once.
            // However, it might be more flexible to forgo this requirement and re-scan all types whenever a new/unknown attribute is requested.
            if (t.GetCustomAttribute<CrucibleRegistryAttributeAttribute>() == null)
            {
                throw new ArgumentException("Target attribute must be marked with CrucibleRegistryAttributeAttribute.", nameof(t));
            }

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

            // FIXME: Nice and compat way of doing the below.  It is crashing with an empty string error message on ToHashSet()
            // Search all loaded types for any type that has been marked with an attribute that is marked with CrucibleRegistryAttribute.
            // var candidates = from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //                  from type in assembly.GetTypes()
            //                  from registryAttribute in
            //                      from registryAttribute in registryAttributeTypes
            //                      where type.GetCustomAttribute(registryAttribute, true) != null
            //                      select registryAttribute
            //                  group type by registryAttribute into attributesByType
            //                  select attributesByType;
            // typesByAttribute = candidates.ToDictionary(x => x.Key, x => x.ToHashSet());

            var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         select type).ToArray();

            typesByAttribute = new Dictionary<Type, HashSet<Type>>();
            foreach (var type in types)
            {
                foreach (var registryAttribute in registryAttributeTypes)
                {
                    var attribute = type.GetCustomAttribute(registryAttribute, true);
                    if (attribute != null)
                    {
                        if (!typesByAttribute.TryGetValue(registryAttribute, out HashSet<Type> typesForAttribute))
                        {
                            typesForAttribute = new HashSet<Type>();
                            typesByAttribute.Add(registryAttribute, typesForAttribute);
                        }

                        typesForAttribute.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all attributes that have been marked as a CrucibleRegistryAttribute.
        /// </summary>
        /// <returns>An enumerable of all attribute types found.</returns>
        private static IEnumerable<Type> GetCrucibleRegistryAttributes()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.GetCustomAttribute<CrucibleRegistryAttributeAttribute>(true) != null
                   select type;
        }
    }
}
