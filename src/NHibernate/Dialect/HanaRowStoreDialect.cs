using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for the SAP HANA row store
	/// </summary>
	/// <remarks>
	/// The HanaRowStoreDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.HanaRowStoreDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class HanaRowStoreDialect : HanaDialectBase
	{
		public HanaRowStoreDialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = typeof(NHibernate.Driver.HanaRowStoreDriver).FullName;
		}

		/// <inheritdoc />
		public override string CreateTableString => "create row table";

		/// <inheritdoc />
		public override string CreateTemporaryTableString => "create local temporary row table";

		/// <inheritdoc />
		public override bool SupportsOuterJoinForUpdate => false;

		/// <inheritdoc />
		public override bool SupportsDistributedTransactions => false;
	}
}
