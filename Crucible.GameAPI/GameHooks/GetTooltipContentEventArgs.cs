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
