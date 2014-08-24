using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace NHibernate.Test.NHSpecificTest.NH2700
{
    public class CustomDialect : MsSql2005Dialect
    {
        public CustomDialect()
        {
            RegisterFunction(
                "AddDays",
                new SQLFunctionTemplate(
                    NHibernateUtil.DateTime,
                    "dateadd(day,?2,?1)"

                    )
                );
        }
    }
}
