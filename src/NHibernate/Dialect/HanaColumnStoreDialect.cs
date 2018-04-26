namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for the SAP HANA column store
	/// </summary>
	/// <remarks>
	/// The HanaColumnStoreDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.HanaDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class HanaColumnStoreDialect : HanaDialectBase
	{
		/// <inheritdoc />
		public override string CreateTableString => "create column table";

		/// <inheritdoc />
		public override string CreateTemporaryTableString => "create local temporary column table";
	}
}
