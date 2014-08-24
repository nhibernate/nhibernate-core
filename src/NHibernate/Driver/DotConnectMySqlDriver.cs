namespace NHibernate.Driver
{
    /// <summary>
    /// Provides a database driver for dotConnect for MySQL by DevArt.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order to use this driver you must have the assembly <c>Devart.Data.MySql.dll</c> available for 
    /// NHibernate to load, including its dependencies (<c>Devart.Data.dll</c>).
    /// </para>
    /// <para>
    /// Please check the product's <see href="http://www.devart.com/dotconnect/mysql/">website</see>
    /// for any updates and/or documentation regarding dotConnect for MySQL.
    /// </para>
    /// </remarks>
    public class DotConnectMySqlDriver : ReflectionBasedDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotConnectMySqlDriver"/> class.
        /// </summary>
        /// <exception cref="HibernateException">
        /// Thrown when the <c>Devart.Data.MySql</c> assembly can not be loaded.
        /// </exception>
        public DotConnectMySqlDriver() : base(
            "Devart.Data.MySql",
            "Devart.Data.MySql.NHibernate.NHibernateMySqlConnection",
            "Devart.Data.MySql.NHibernate.NHibernateMySqlCommand")
        {
        }

        /// <summary>
        /// Devart.Data.MySql uses named parameters in the sql.
        /// </summary>
        /// <value><see langword="true" /> - MySql uses <c>@</c> in the sql.</value>
        public override bool UseNamedPrefixInSql
        {
            get { return true; }
        }

        /// <summary></summary>
        public override bool UseNamedPrefixInParameter
        {
            get { return true; }
        }

        /// <summary>
        /// Devart.Data.MySql use the <c>@</c> to locate parameters in sql.
        /// </summary>
        /// <value><c>@</c> is used to locate parameters in sql.</value>
        public override string NamedPrefix
        {
            get { return "@"; }
        }
    }
}
