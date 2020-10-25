namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Indicates the status of a tracking file.
    /// </summary>
    public enum TrackingFileStatus
    {
        /// <summary>
        ///     In sync with the cloud and without errors.
        /// </summary>
        UpToDate,

        /// <summary>
        ///     Some state error occurred between the
        ///     tracking file and the data store. This
        ///     marks a local invalid state.
        /// </summary>
        InvalidLocal,
    }
}
