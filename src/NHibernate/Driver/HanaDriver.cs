using System;
using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	/// <summary>
	/// Provides a database driver for SAP HANA.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this driver you must have the assembly <c>Sap.Data.Hana.v4.5.dll</c> available for 
	/// NHibernate to load, including its dependencies (<c>libadonetHDB.dll</c> and <c>libSQLDBCHDB.dll</c> 
	/// are required by the assembly <c>Sap.Data.Hana.v4.5.dll</c> as of the time of this writing).
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
		/// Thrown when the <c>Sap.Data.Hana.v4.5</c> assembly can not be loaded.
		/// </exception>
		public HanaDriver() : base(
			"Sap.Data.Hana",
			"Sap.Data.Hana.v4.5",
			"Sap.Data.Hana.HanaConnection",
			"Sap.Data.Hana.HanaCommand")
		{
		}

		/// <inheritdoc />
		public override bool UseNamedPrefixInSql => false;

		/// <inheritdoc />
		public override bool UseNamedPrefixInParameter => false;

		/// <inheritdoc />
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

		/// <inheritdoc />
		public override bool SupportsEnlistmentWhenAutoEnlistmentIsDisabled => false;

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(HanaBatchingBatcherFactory);
	}
}
