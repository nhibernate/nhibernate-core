using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	/// <summary>
	/// The PostgreSQL data provider provides a database driver for PostgreSQL.
	/// <p>
	/// Author: <a href="mailto:oliver@weichhold.com">Oliver Weichhold</a>
	/// </p>
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the Npgsql.dll Assembly available for 
	/// NHibernate to load it.
	/// </p>
	/// <p>
	/// Please check the products website 
	/// <a href="http://www.postgresql.org/">http://www.postgresql.org/</a>
	/// for any updates and or documentation.
	/// </p>
	/// <p>
	/// The homepage for the .NET DataProvider is: 
	/// <a href="http://pgfoundry.org/projects/npgsql">http://pgfoundry.org/projects/npgsql</a>. 
	/// </p>
	/// </remarks>
	public class NpgsqlDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NpgsqlDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Npgsql</c> assembly can not be loaded.
		/// </exception>
		public NpgsqlDriver() : base(
			"Npgsql",
			"Npgsql",
			"Npgsql.NpgsqlConnection",
			"Npgsql.NpgsqlCommand")
		{
		}

		public override bool UseNamedPrefixInSql => true;

		public override bool UseNamedPrefixInParameter => true;

		public override string NamedPrefix => ":";

		public override bool SupportsMultipleOpenReaders => false;

		/// <remarks>
		/// NH-2267 Patrick Earl
		/// </remarks>
		protected override bool SupportsPreparingCommands => true;

		public override bool SupportsNullEnlistment => false;

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool SupportsMultipleQueries => true;

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlTypes.SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);

			// Since the .NET currency type has 4 decimal places, we use a decimal type in PostgreSQL instead of its native 2 decimal currency type.
			if (sqlType.DbType == DbType.Currency)
				dbParam.DbType = DbType.Decimal;
		}

		// Prior to v3, Npgsql was expecting DateTime for time.
		// https://github.com/npgsql/npgsql/issues/347
		public override bool RequiresTimeSpanForTime => (DriverVersion?.Major ?? 3) >= 3;

		public override bool HasDelayedDistributedTransactionCompletion => true;

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(GenericBatchingBatcherFactory);
	}
}
