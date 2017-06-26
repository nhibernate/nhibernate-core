using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.DataAccess.Lite DataProvider
	/// </summary>
	public class OracleLiteDataClientDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of <see cref="OracleLiteDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.DataAccess.Lite_w32</c> assembly can not be loaded.
		/// </exception>
		public OracleLiteDataClientDriver()
			: base(
						"Oracle.DataAccess.Lite_w32",
						"Oracle.DataAccess.Lite.OracleConnection",
						"Oracle.DataAccess.Lite.OracleCommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return string.Empty; }
		}

		/// <remarks>
		/// This adds logic to ensure that a DbType.Boolean parameter is not created since
		/// ODP.NET doesn't support it.
		/// </remarks>
		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			// if the parameter coming in contains a boolean then we need to convert it 
			// to another type since ODP.NET doesn't support DbType.Boolean
			if (sqlType.DbType == DbType.Boolean)
			{
				sqlType = SqlTypeFactory.Int16;
			}
			base.InitializeParameter(dbParam, name, sqlType);
		}

		#region IEmbeddedBatcherFactoryProvider Members

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
		{
			get { return typeof(OracleDataClientBatchingBatcherFactory); }
		}

		#endregion
	}
}