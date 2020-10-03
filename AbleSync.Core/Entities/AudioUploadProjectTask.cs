namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     <see cref="ProjectTask"/> representing the upload of an audio file.
    /// </summary>
    public sealed class AudioUploadProjectTask : ProjectTask
    {
        /// <summary>
        ///     The file name of the file to upload.
        /// </summary>
        public string FileName { get; set; }
    }
}
