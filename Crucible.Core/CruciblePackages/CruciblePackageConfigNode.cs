// <copyright file="CruciblePackageConfigNode.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.CruciblePackages
{
    using RoboPhredDev.PotionCraft.Crucible.Yaml;
    using YamlDotNet.Core;

    /// <summary>
    /// Provides the base class to define configuration nodes in a <see cref="CruciblePackageMod"/>.
    /// </summary>
    public abstract class CruciblePackageConfigNode : IAfterYamlDeserialization
    {
        /// <summary>
        /// Gets the Crucible mod this node is a part of.
        /// </summary>
        public CruciblePackageMod PackageMod
        {
            get; private set;
        }

        /// <inheritdoc/>
        void IAfterYamlDeserialization.OnDeserializeCompleted(Mark start, Mark end)
        {
            CruciblePackageMod.OnNodeLoaded(this);
            this.OnDeserializeCompleted(start, end);
        }

        /// <summary>
        /// Sets the crucible config mod that owns this node.
        /// </summary>
        /// <param name="packageMod">The <see cref="CruciblePackageMod"/> that owns this node.</param>
        internal void SetParentMod(CruciblePackageMod packageMod)
        {
            this.PackageMod = packageMod;
        }

        /// <summary>
        /// Runs when the deserialization of this node is complete.
        /// </summary>
        /// <param name="start">The start of this node in the yaml document.</param>
        /// <param name="end">The end of this node in the yaml document.</param>
        /// <remarks>
        /// This can be used to perform validation on the loaded data.
        /// </remarks>
        protected virtual void OnDeserializeCompleted(Mark start, Mark end)
        {
        }
    }
}
