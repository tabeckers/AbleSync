namespace AbleSync.Core.Types
{
    // TODO Do we want this as an enum?
    /// <summary>
    ///     Represents the type of project task.
    /// </summary>
    public enum ProjectTaskType
    {
        /// <summary>
        ///     We need to upload an audio file.
        /// </summary>
        UploadAudio,

        /// <summary>
        ///     We need to fully backup a project.
        /// </summary>
        BackupFull,
    }
}
