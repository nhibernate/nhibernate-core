using NHibernate.AdoNet;

namespace NHibernate.Driver
{
	/// <summary>
	/// Provides a database driver base class for SAP HANA.
	/// </summary>
	public abstract class HanaDriverBase : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HanaDriverBase"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Sap.Data.Hana.v4.5</c> assembly can not be loaded.
		/// </exception>
		protected HanaDriverBase() : base(
			"Sap.Data.Hana",
			"Sap.Data.Hana.v4.5",
			"Sap.Data.Hana.HanaConnection",
			"Sap.Data.Hana.HanaCommand")
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// Named parameters are not supported by the SAP HANA .Net provider.
		/// https://help.sap.com/viewer/0eec0d68141541d1b07893a39944924e/2.0.02/en-US/d197835a6d611014a07fd73ee6fed6eb.html
		/// </remarks>
		public override bool UseNamedPrefixInSql => false;

		/// <inheritdoc />
		public override bool UseNamedPrefixInParameter => false;

		/// <inheritdoc />
		public override string NamedPrefix => string.Empty;

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		/// <summary>
		/// It does support it indeed, provided any previous transaction has finished completing. But scopes
		/// are always promoted to distributed with <c>HanaConnection</c>, which causes them to complete on concurrent
		/// threads. This creates race conditions with following a scope disposal. As this null enlistment feature
		/// is here for attemptinng de-enlisting a connection from a completed transaction not yet cleaned-up, and as
		/// <c>HanaConnection</c> does not handle such a case, better disable it.
		/// </summary>
		public override bool SupportsNullEnlistment => false;

		public override bool RequiresTimeSpanForTime => true;

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(HanaBatchingBatcherFactory);
	}
}
