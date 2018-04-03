using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

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
	public class HanaColumnStoreDialect : AbstractHanaDialect
	{
		public HanaColumnStoreDialect()
		{
		}

		/// <summary> Command used to create a table. </summary>
		public override string CreateTableString
		{
			get { return "create column table"; }
		}

		/// <summary> Command used to create a temporary table. </summary>
		public override string CreateTemporaryTableString
		{
			get { return "create local temporary column table"; }
		}
	}
}
