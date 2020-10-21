namespace AbleSync.Core.Types
{
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
