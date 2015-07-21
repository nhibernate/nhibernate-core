using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2074
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}
     
		[Test]
      	public void CanQueryOnPropertyUsingUnicodeToken()
		{
            using (var s = OpenSession())
            {
            	s.CreateQuery("from Person").List();
            }
		} 

	}
}
