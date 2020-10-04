namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Enum representing the status of a project task.
    /// </summary>
    public enum ProjectTaskStatus
    {
        /// <summary>
        ///     The task has been created.
        /// </summary>
        Created,

        /// <summary>
        ///     Some entity is processing the task.
        /// </summary>
        Processing,

        /// <summary>
        ///     The task has been processed successfully.
        /// </summary>
        Done,

        /// <summary>
        ///     The task has failed.
        /// </summary>
        Failed
    }
}
