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
