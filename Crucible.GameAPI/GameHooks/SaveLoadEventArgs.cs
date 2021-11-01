namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GameHooks
{
    using System;
    using SaveFileSystem;

    /// <summary>
    /// Event arguments for notifying of a load or save operation.
    /// </summary>
    public class SaveLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLoadEventArgs"/> class.
        /// </summary>
        /// <param name="file">The file being loaded or saved.</param>
        public SaveLoadEventArgs(File file)
        {
            this.File = file;
        }

        /// <summary>
        /// Gets the file being loaded or saved.
        /// </summary>
        public File File { get; }
    }
}
