using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace NHibernate.Test.NHSpecificTest.NH1849
{
    public class CustomDialect : MsSql2005Dialect
    {
        public CustomDialect()
        {
			RegisterFunction("contains", new StandardSQLFunction("contains"));
        }
    }
}
