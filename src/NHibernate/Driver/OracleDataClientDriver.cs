using System.Data;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.DataAccess DataProvider
	/// </summary>
	/// <remarks>
	/// Code was contributed by <a href="http://sourceforge.net/users/jemcalgary/">James Mills</a>
	/// on the NHibernate forums in this 
	/// <a href="http://sourceforge.net/forum/message.php?msg_id=2952662">post</a>.
	/// </remarks>
	public class OracleDataClientDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of <see cref="OracleDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.DataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleDataClientDriver()
			: base(
			"Oracle.DataAccess",
			"Oracle.DataAccess.Client.OracleConnection",
			"Oracle.DataAccess.Client.OracleCommand")
		{
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}

		/// <remarks>
		/// This adds logic to ensure that a DbType.Boolean parameter is not created since
		/// ODP.NET doesn't support it.
		/// </remarks>
		protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
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
			get { return typeof (OracleDataClientBatchingBatcherFactory); }
		}

		#endregion
	}
}