using System;
using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	/// <summary>
	/// Provides a database driver for MySQL.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this driver you must have the assembly <c>MySql.Data.dll</c> available for 
	/// NHibernate to load, including its dependencies (<c>ICSharpCode.SharpZipLib.dll</c> is required by
	/// the assembly <c>MySql.Data.dll</c> as of the time of this writing).
	/// </para>
	/// <para>
	/// Please check the product's <see href="http://www.mysql.com/products/connector/net/">website</see>
	/// for any updates and/or documentation regarding MySQL.
	/// </para>
	/// </remarks>
	public class MySqlDataDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlDataDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>MySql.Data</c> assembly can not be loaded.
		/// </exception>
		public MySqlDataDriver() : base(
			"MySql.Data.MySqlClient",
			"MySql.Data",
			"MySql.Data.MySqlClient.MySqlConnection",
			"MySql.Data.MySqlClient.MySqlCommand")
		{
		}

		/// <summary>
		/// MySql.Data uses named parameters in the sql.
		/// </summary>
		/// <value><see langword="true" /> - MySql uses <c>?</c> in the sql.</value>
		public override bool UseNamedPrefixInSql => true;

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter => true;

		/// <summary>
		/// MySql.Data use the <c>?</c> to locate parameters in sql.
		/// </summary>
		/// <value><c>?</c> is used to locate parameters in sql.</value>
		public override string NamedPrefix => "?";

		/// <summary>
		/// The MySql.Data driver does NOT support more than 1 open DbDataReader
		/// with only 1 DbConnection.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		public override bool SupportsMultipleOpenReaders => false;

		/// <summary>
		/// MySql.Data supports prepared statements but in the current state NHibernate may
		/// execute two queries in one command separated by a semicolon (e.g. SELECT LAST_INSERT_ID() when
		/// native id generator is used), which throws an exception when <see cref="System.Data.Common.DbCommand.Prepare"/>
		/// is called.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		/// <remarks>
		/// With the Gamma MySql.Data provider it is throwing an exception with the 
		/// message "Expected End of data packet" when a select command is prepared.
		/// </remarks>
		protected override bool SupportsPreparingCommands => false;

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool SupportsMultipleQueries => true;

		public override bool RequiresTimeSpanForTime => true;

		// As of v5.7, lower dates may "work" but without guarantees.
		// https://dev.mysql.com/doc/refman/5.7/en/datetime.html
		/// <inheritdoc />
		public override DateTime MinDate => new DateTime(1000, 1, 1);

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(MySqlClientBatchingBatcherFactory);
	}
}
