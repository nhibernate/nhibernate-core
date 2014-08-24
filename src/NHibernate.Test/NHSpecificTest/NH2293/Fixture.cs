using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;
using SharpTestsEx;

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
				session.Executing(s => s.CreateQuery("from").List()).Throws<QuerySyntaxException>();
			}
		}
	}
}