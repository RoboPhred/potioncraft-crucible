// <copyright file="CrucibleConfigElementRegistry.cs" company="RoboPhredDev">
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
    public static class CrucibleConfigElementRegistry
    {
        private static readonly HashSet<Type> WarnedTypes = new();

        /// <summary>
        /// Gets all configuration roots to be parsed from mod configs.
        /// </summary>
        /// <returns>An enumerable of configuration roots.</returns>
        public static IEnumerable<Type> GetConfigRoots()
        {
            foreach (var candidate in CrucibleTypeRegistry.GetTypesByAttribute<CrucibleConfigRootAttribute>())
            {
                if (!typeof(CrucibleConfigRoot).IsAssignableFrom(candidate))
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
            var subjectCandidates = from candidate in CrucibleTypeRegistry.GetTypesByAttribute<CrucibleConfigExtensionAttribute>()
                                    let attribute = (CrucibleConfigExtensionAttribute)Attribute.GetCustomAttribute(candidate, typeof(CrucibleConfigExtensionAttribute))
                                    where attribute.SubjectType.IsAssignableFrom(typeof(TSubject))
                                    select candidate;
            foreach (var candidate in subjectCandidates)
            {
                if (!typeof(CrucibleConfigExtension<>).MakeGenericType(typeof(TSubject)).IsAssignableFrom(candidate))
                {
                    WarnMisimplementedExtension(candidate, typeof(TSubject));
                    continue;
                }

                yield return candidate;
            }
        }

        private static void WarnMisimplementedRoot(Type type)
        {
            if (WarnedTypes.Add(type))
            {
                CrucibleLog.LogInScope("net.robophreddev.PotionCraft.Crucible", $"Cannot use {type.FullName} as a config root because it lacks the appropriate {typeof(CrucibleConfigRoot).Name} interface.");
            }
        }

        private static void WarnMisimplementedExtension(Type type, Type subjectType)
        {
            if (WarnedTypes.Add(type))
            {
                CrucibleLog.LogInScope("net.robophreddev.PotionCraft.Crucible", $"Cannot use {type.FullName} as a config extension for {subjectType.Name} because it lacks the appropriate {typeof(CrucibleConfigExtension<>).Name} interface.");
            }
        }
    }
}
