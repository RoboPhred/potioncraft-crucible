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
