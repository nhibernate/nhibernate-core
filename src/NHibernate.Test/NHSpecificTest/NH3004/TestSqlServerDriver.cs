using NHibernate.Driver;

namespace NHibernate.Test.NHSpecificTest.NH3004
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class TestSqlServerDriver : SqlServer2000Driver
	{
		bool _UseNamedPrefixInSql = true;
		bool _UseNamedPrefixInParameter = false;

		public TestSqlServerDriver()
		{

		}

		public TestSqlServerDriver(bool UseNamedPrefixInSql, bool UseNamedPrefixInParameter)
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
