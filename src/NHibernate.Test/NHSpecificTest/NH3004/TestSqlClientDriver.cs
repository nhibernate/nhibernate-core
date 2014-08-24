using System.Data;
using System.Data.SqlClient;
using NHibernate.AdoNet;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Driver;

namespace NHibernate.Test.NHSpecificTest.NH3004
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class TestSqlClientDriver : SqlClientDriver
	{
		bool _UseNamedPrefixInSql = true;
		bool _UseNamedPrefixInParameter = false;

		public TestSqlClientDriver()
		{

		}

		public TestSqlClientDriver(bool UseNamedPrefixInSql, bool UseNamedPrefixInParameter)
		{
			_UseNamedPrefixInSql = UseNamedPrefixInSql;
			_UseNamedPrefixInParameter = UseNamedPrefixInParameter;
		}

		public override bool UseNamedPrefixInSql
		{
			get { return _UseNamedPrefixInSql; }
			
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return _UseNamedPrefixInParameter; }
		}
	}
}