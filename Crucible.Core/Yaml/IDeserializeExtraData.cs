// <copyright file="IDeserializeExtraData.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.Yaml
{
    /// <summary>
    /// An interface marking a class as wanting to parse extra data out of its yaml parser.
    /// </summary>
    public interface IDeserializeExtraData
    {
        /// <summary>
        /// Called after deserialization of the object, with a parser containing the object's node stream.
        /// </summary>
        /// <param name="parser">A parser containing the object's node stream.</param>
        void OnDeserializeExtraData(ReplayParser parser);
    }
}
