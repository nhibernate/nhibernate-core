using System.Data;
using System.Globalization;
using System.Text;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for DB2 on iSeries OS/400.
	/// </summary>
	/// <remarks>
	/// The DB2400Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.DB2400Driver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class DB2400Dialect : DB2Dialect
	{
		public DB2400Dialect()
		{
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.DB2400Driver";
		}
	}
}