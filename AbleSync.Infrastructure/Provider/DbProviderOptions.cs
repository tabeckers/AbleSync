namespace AbleSync.Infrastructure.Provider
{
    /// <summary>
    ///     Configuration file for our database provider.
    /// </summary>
    public sealed class DbProviderOptions
    {
        /// <summary>
        ///     The name of our database connection string 
        ///     as it is declared in the connection string
        ///     section.
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}
