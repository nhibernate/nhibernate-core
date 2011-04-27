using NHibernate.Cfg;
using NHibernate.Dialect;

namespace NHibernate.Test.DialectTest
{
	public class FirebirdClientDialect : FirebirdDialect
	{
		public FirebirdClientDialect() : base()
		{
			// overrides default driver to allow tests to run using embedded client
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.FirebirdClientDriver";
		}
	}
}
