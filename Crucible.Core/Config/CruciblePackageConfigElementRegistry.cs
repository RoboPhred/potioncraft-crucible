// <copyright file="CruciblePackageConfigElementRegistry.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The registry to store and retrieve configuration class types.
    /// </summary>
    public static class CruciblePackageConfigElementRegistry
    {
        private static readonly HashSet<Type> WarnedTypes = new();

        /// <summary>
        /// Gets all configuration roots to be parsed from mod configs.
        /// </summary>
        /// <returns>An enumerable of configuration roots.</returns>
        public static IEnumerable<Type> GetConfigRoots()
        {
            foreach (var candidate in CrucibleTypeRegistry.GetTypesByAttribute<CruciblePackageConfigRootAttribute>())
            {
                if (!typeof(CruciblePackageConfigRoot).IsAssignableFrom(candidate))
                {
                    WarnMisimplementedRoot(candidate);
                }

                yield return candidate;
            }
        }

        /// <summary>
        /// Gets all <see cref="ICrucibleConfigExtension"/>s that apply to the given <paramref name="TSubject"/>.
        /// </summary>
        /// <typeparam name="TSubject">The subject type to fetch config extensions for.</typeparam>
        /// <returns>An enumerable of types that instantiate classes implementing <see cref="ICrucibleConfigExtension"/> and targeting the given subject.</returns>
        public static IEnumerable<Type> GetSubjectExtensionTypes<TSubject>()
        {
            var subjectCandidates = from candidate in CrucibleTypeRegistry.GetTypesByAttribute<CruciblePackageConfigExtensionAttribute>()
                                    let attribute = (CruciblePackageConfigExtensionAttribute)Attribute.GetCustomAttribute(candidate, typeof(CruciblePackageConfigExtensionAttribute))
                                    where attribute.SubjectType.IsAssignableFrom(typeof(TSubject))
                                    select new { Type = candidate, Attribute = attribute };

            foreach (var candidate in subjectCandidates)
            {
                // Check to see if the candidate implements the expected base type.
                if (!VerifySubjectExtension(candidate.Type, candidate.Attribute))
                {
                    continue;
                }

                yield return candidate.Type;
            }
        }

        private static bool VerifySubjectExtension(Type classType, CruciblePackageConfigExtensionAttribute extensionAttribute)
        {
            var expectedAssignable = typeof(ICruciblePackageConfigExtension<>).MakeGenericType(extensionAttribute.SubjectType);
            if (!expectedAssignable.IsAssignableFrom(classType) || !classType.BaseTypeIncludes(typeof(CruciblePackageConfigNode)))
            {
                WarnMisimplementedExtension(classType, extensionAttribute.SubjectType);
                return false;
            }

            return true;
        }

        private static void WarnMisimplementedRoot(Type type)
        {
            if (WarnedTypes.Add(type))
            {
                CrucibleLog.LogInScope("net.robophreddev.PotionCraft.Crucible", $"Cannot use {type.FullName} as a config root because it lacks the appropriate {typeof(CruciblePackageConfigRoot).Name} interface.");
            }
        }

        private static void WarnMisimplementedExtension(Type type, Type subjectType)
        {
            if (WarnedTypes.Add(type))
            {
                CrucibleLog.LogInScope("net.robophreddev.PotionCraft.Crucible", $"Cannot use {type.FullName} as a config extension for {subjectType.Name} because it either lacks the appropriate {typeof(ICruciblePackageConfigExtension<>).Name} interface, specifies the wrong TSubject, or does not implement {nameof(CruciblePackageConfigNode)}.");
            }
        }
    }
}
