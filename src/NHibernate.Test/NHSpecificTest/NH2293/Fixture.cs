using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2293
{
	public class Fixture : BugTestCase
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				return Enumerable.Empty<object>().ToList();
			}
		}

		[Test]
		public void WhenQueryHasJustAfromThenThrowQuerySyntaxException()
		{
			using (ISession session = OpenSession())
			{
				Assert.That(() => session.CreateQuery("from").List(), Throws.TypeOf<QuerySyntaxException>());
			}
		}
	}
}