// <copyright file="GetTooltipContentEventArgs.cs" company="RoboPhredDev">
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

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using ObjectBased.UIElements.Tooltip;

    /// <summary>
    /// Event arguments for requesting a tooltip for a given subject.
    /// </summary>
    /// <typeparam name="TSubject">The subject the tooltip is being requested for.</typeparam>
    public class GetTooltipContentEventArgs<TSubject> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTooltipContentEventArgs{TSubject}"/> class.
        /// </summary>
        /// <param name="subject">The subject a tooltip is being created for.</param>
        public GetTooltipContentEventArgs(TSubject subject)
        {
            this.Subject = subject;
        }

        /// <summary>
        /// Gets the subject for which the tooltip is being created.
        /// </summary>
        public TSubject Subject { get; }

        /// <summary>
        /// Gets or sets the tooltip to use for the subject.
        /// </summary>
        public TooltipContent TooltipContent { get; set; }
    }
}
