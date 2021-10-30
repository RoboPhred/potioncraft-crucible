namespace RoboPhredDev.PotionCraft.Crucible.Resources
{
    using System;

    public class CrucibleResourceException : Exception
    {
        public CrucibleResourceException(string message) : base(message) { }

        public string ResourceName { get; set; }
    }
}