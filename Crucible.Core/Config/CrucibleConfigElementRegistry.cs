namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The registry to store and retrieve configuration class types.
    /// </summary>
    public static class CrucibleConfigElementRegistry
    {
        /// <summary>
        /// A set of configuration roots.
        /// </summary>
        private static readonly HashSet<Type> ConfigRoots = new();

        /// <summary>
        /// A map of configuration extensions to their subject type.
        /// </summary>
        private static readonly Dictionary<Type, HashSet<Type>> SubjectExtensions = new();

        /// <summary>
        /// Gets all configuration roots to be parsed from mod configs.
        /// </summary>
        /// <returns>A read-only collection of configuration roots.</returns>
        public static IReadOnlyCollection<Type> GetConfigRoots()
        {
            return ConfigRoots.ToArray();
        }

        /// <summary>
        /// Gets all <see cref="ICrucibleConfigExtension"/>s that apply to the given <paramref name="TSubject"/>.
        /// </summary>
        /// <typeparam name="TSubject">The subject type to fetch config extensions for.</typeparam>
        /// <returns>A read-only collection of types that instantiate classes implementing <see cref="ICrucibleConfigExtension"/> and targeting the given subject.</returns>
        public static IReadOnlyCollection<Type> GetSubjectExtensionTypes<TSubject>()
        {
            if (SubjectExtensions.TryGetValue(typeof(TSubject), out var extensionTypes))
            {
                return extensionTypes.ToArray();
            }

            return new Type[0];
        }

        /// <summary>
        /// Registers all relevant types from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to register types from.</param>
        public static void RegisterAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                TryRegisterType(type);
            }
        }

        /// <summary>
        /// Attempts to register the type as a crucible configuration class.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <returns>True if the type was registered, false if the type was not known.</returns>
        public static bool TryRegisterType(Type type)
        {
            if (IsConfigRoot(type))
            {
                ConfigRoots.Add(type);
                return true;
            }

            if (IsConfigExtension(type, out var subjectType))
            {
                if (!SubjectExtensions.TryGetValue(subjectType, out var extensionTypes))
                {
                    extensionTypes = new HashSet<Type>();
                    SubjectExtensions.Add(subjectType, extensionTypes);
                }

                extensionTypes.Add(type);
                return true;
            }

            return false;
        }

        private static bool IsConfigRoot(Type type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            if (Attribute.GetCustomAttribute(type, typeof(CrucibleConfigRootAttribute)) is CrucibleConfigRootAttribute)
            {
                return true;
            }

            return false;
        }

        private static bool IsConfigExtension(Type type, out Type subjectType)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                subjectType = null;
                return false;
            }

            if (Attribute.GetCustomAttribute(type, typeof(CrucibleConfigExtensionAttribute)) is not CrucibleConfigExtensionAttribute extensionAttribute)
            {
                subjectType = null;
                return false;
            }

            subjectType = extensionAttribute.SubjectType;
            if (!typeof(ICrucibleConfigExtension<>).MakeGenericType(subjectType).IsAssignableFrom(type))
            {
                // TODO: This was meant for us but not proplerly implemented.  Log a warning.
                return false;
            }

            return true;
        }
    }
}
