// <copyright file="IAfterYamlDeserialization.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Foobar is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    using YamlDotNet.Core;

    /// <summary>
    /// An interface to handle post-deserialization operations.
    /// </summary>
    public interface IAfterYamlDeserialization
    {
        /// <summary>
        /// Called when the object has been deserialized.
        /// </summary>
        /// <param name="start">The start of this object in the yaml file.</param>
        /// <param name="end">The end of this object in the yaml file.</param>
        void OnDeserializeCompleted(Mark start, Mark end);
    }
}
