// <copyright file="CruciblePackageConfigSubjectNode.cs" company="RoboPhredDev">
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
    using System.Collections.Generic;
    using RoboPhredDev.PotionCraft.Crucible.Yaml;

    /// <summary>
    /// Defines a configration node in a <see cref="CruciblePackageMod"/> package config which creates or otherwise targets
    /// a specific class instance.
    /// </summary>
    /// <remarks>
    /// Subject nodes typically operate on game api classes, and contain enough configuration to instantiate or target existing classes.
    /// Subject nodes can be additionally extended using <see cref="CruciblePackageConfigExtensionAttribute"/>, which provide further
    /// configuration and modification of the subject config's subject.
    /// </remarks>
    /// <typeparam name="TSubject">The subject object created as a result of this configuration entry.</typeparam>
    public abstract class CruciblePackageConfigSubjectNode<TSubject> : CruciblePackageConfigNode, IDeserializeExtraData
    {
        private readonly List<ICruciblePackageConfigExtension<TSubject>> extensions = new();

        /// <summary>
        /// Applies the configuration node.
        /// </summary>
        public void ApplyConfiguration()
        {
            var subject = this.GetSubject();
            this.OnApplyConfiguration(subject);
            foreach (var extension in this.extensions)
            {
                extension.OnApplyConfiguration(subject);
            }
        }

        /// <summary>
        /// Parse and apply extensions to the subject.
        /// </summary>
        /// <param name="parser">The parser containing this configuration object.</param>
        void IDeserializeExtraData.OnDeserializeExtraData(ReplayParser parser)
        {
            this.extensions.Clear();

            foreach (var extensionType in CruciblePackageConfigElementRegistry.GetSubjectExtensionTypes<TSubject>())
            {
                parser.Reset();
                var extension = (ICruciblePackageConfigExtension<TSubject>)Deserializer.DeserializeFromParser(extensionType, parser);
                this.extensions.Add(extension);
            }
        }

        /// <summary>
        /// Gets or creates the subject of this configuration object.
        /// This is called after the element is deserialized, but before extension configurations are applied.
        /// </summary>
        /// <remarks>
        /// Where possible, this function should try to look up subjects by name and return pre-existing matching subjects.
        /// This allows configs to be reloaded without duplicating data.
        /// </remarks>
        /// <returns>The subject to which configuration options should be applied.</returns>
        protected abstract TSubject GetSubject();

        /// <summary>
        /// Apply the configuration to the subject.
        /// </summary>
        /// <param name="subject">The subject to apply configuration to.</param>
        protected abstract void OnApplyConfiguration(TSubject subject);
    }
}
