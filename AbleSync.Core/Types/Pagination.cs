namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Contains information about pagination.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        ///     Indicates the items per page.
        /// </summary>
        /// <remarks>
        ///     Defaults to 25.
        /// </remarks>
        public uint ItemsPerPage { get; set; } = 25;

        /// <summary>
        ///     Indicates the page.
        /// </summary>
        /// <remarks>
        ///     Defaults to 0.
        /// </remarks>
        public uint Page { get; set; }

        /// <summary>
        ///     Indicates the sorting order.
        /// </summary>
        /// <remarks>
        ///     Defaults to <see cref="SortOrder.Ascending"/>.
        /// </remarks>
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        /// <summary>
        ///     Instantiates a default pagination object with
        ///     <see cref="ItemsPerPage"/> = 25 and <see cref="Page"/> = 0.
        /// </summary>
        public static Pagination Default
            => new Pagination
            {
                ItemsPerPage = 25,
                Page = 0,
                SortOrder = SortOrder.Ascending
            };

        /// <summary>
        ///     Instantiates a pagination which will get all
        ///     objects in our data store at once.
        /// </summary>
        public static Pagination All
            => new Pagination
            {
                ItemsPerPage = 0,
                Page = 0,
                SortOrder = SortOrder.Ascending
            };
    }
}
