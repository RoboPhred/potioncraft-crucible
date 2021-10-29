namespace RoboPhredDev.PotionCraft.Crucible.Config
{
    using System;

    /// <summary>
    /// An attribute marking a class as being a root configuration object.
    /// Such classes will be deserialized from the root of each mod configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [CrucibleRegistryAttribute]
    public class CrucibleConfigRootAttribute : Attribute
    {
    }
}
