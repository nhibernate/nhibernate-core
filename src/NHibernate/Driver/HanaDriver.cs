using System;
using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	/// <summary>
	/// Provides a database driver for SAP HANA.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this driver you must have the assembly <c>Sap.Data.Hana.dll</c> available for 
	/// NHibernate to load, including its dependencies (<c>libadonetHDB.dll</c> and <c>libSQLDBCHDB.dll</c> 
	/// are required by the assembly <c>Sap.Data.Hana.dll</c> as of the time of this writing).
	/// </para>
	/// <para>
	/// Please check the product's <see href="https://help.sap.com/viewer/0eec0d68141541d1b07893a39944924e/2.0.02/en-US/469dee9e6d611014af70d4e9a9cd6b0a.html">website</see>
	/// for any updates and/or documentation regarding SAP HANA.
	/// </para>
	/// </remarks>
	public class HanaDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HanaDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Sap.Data.Hana</c> assembly can not be loaded.
		/// </exception>
		public HanaDriver() : base(
			"Sap.Data.Hana",
			"Sap.Data.Hana.v4.5",
			"Sap.Data.Hana.HanaConnection",
			"Sap.Data.Hana.HanaCommand")
		{
		}

		/// <summary>
		/// Does this Driver require the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// For example, SqlClient requires <c>select * from simple where simple_id = @simple_id</c>
		/// If this is false, like with the OleDb provider, then it is assumed that  
		/// the <c>?</c> can be a placeholder for the parameter in the SQL statement.
		/// </remarks>
		public override bool UseNamedPrefixInSql => false;

		/// <summary>
		/// Does this Driver require the use of the Named Prefix when trying
		/// to reference the Parameter in the Command's Parameter collection.  
		/// </summary>
		/// <remarks>
		/// This is really only useful when the UseNamedPrefixInSql == true.  When this is true the
		/// code will look like:
		/// <code>DbParameter param = cmd.Parameters["@paramName"]</code>
		/// if this is false the code will be 
		/// <code>DbParameter param = cmd.Parameters["paramName"]</code>.
		/// </remarks>
		public override bool UseNamedPrefixInParameter => false;

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <remarks>
		/// Sql Server uses <c>"@"</c> and Oracle uses <c>":"</c>.
		/// </remarks>
		public override string NamedPrefix => String.Empty;

		public override bool SupportsMultipleOpenReaders => false;

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool SupportsSystemTransactions => true;

		public override bool SupportsNullEnlistment => false;

		public override bool RequiresTimeSpanForTime => true;

		public override bool HasDelayedDistributedTransactionCompletion => false;

		public override bool SupportsEnlistmentWhenAutoEnlistmentIsDisabled => false;

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(HanaBatchingBatcherFactory);
	}
}
