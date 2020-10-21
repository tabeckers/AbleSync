namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Indicates the state of a given project.
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        ///     Newly created project.
        /// </summary>
        Created,

        /// <summary>
        ///     Completely up to date project.
        /// </summary>
        UpToDate,

        /// <summary>
        ///     Project has some pending actions.
        /// </summary>
        PendingActions,

        /// <summary>
        ///     Project has been considered invalid.
        /// </summary>
        Invalid
    }
}
