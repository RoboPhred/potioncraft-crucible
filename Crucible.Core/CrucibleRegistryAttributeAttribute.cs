namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;

    /// <summary>
    /// Specifies that classes tagged by this attribute should be tracked in the Crucible registry.
    /// </summary>
    /// <remarks>
    /// Attributes marked with this attribute will be discoverable through the <see cref="CrucibleRegistry"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class CrucibleRegistryAttributeAttribute : Attribute
    {
    }
}
