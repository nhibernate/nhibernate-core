namespace NHibernate.Driver
{
    /// <summary>
    /// NHibernate driver for the Community CsharpSqlite data provider.
    /// <p>
    /// Author: <a href="mailto:nick_tountas@hotmail.com"> Nikolaos Tountas </a>
    /// </p>
    /// </summary>
    /// <remarks>
    /// <p>
    /// In order to use this Driver you must have the Community.CsharpSqlite.dll and Community.CsharpSqlite.SQLiteClient assemblies referenced.
    /// </p>
    /// <p>
    /// Please check <a href="http://code.google.com/p/csharp-sqlite/"> http://code.google.com/p/csharp-sqlite/ </a> for more information regarding csharp-sqlite.
    /// </p>
    /// </remarks>
    public class CsharpSqliteDriver : ReflectionBasedDriver
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CsharpSqliteDriver"/>.
        /// </summary>
        /// <exception cref="HibernateException">
        /// Thrown when the <c>Community.CsharpSqlite.dll</c> assembly can not be loaded.
        /// </exception>
        public CsharpSqliteDriver()
            : base(
            "Community.CsharpSqlite.SQLiteClient",
            "Community.CsharpSqlite.SQLiteClient.SqliteConnection",
            "Community.CsharpSqlite.SQLiteClient.SqliteCommand")
        {
        }

        public override bool UseNamedPrefixInSql
        {
            get { return true; }
        }

        public override bool UseNamedPrefixInParameter
        {
            get { return true; }
        }

        public override string NamedPrefix
        {
            get { return "@"; }
        }

        public override bool SupportsMultipleOpenReaders
        {
            get { return false; }
        }
    }
}
