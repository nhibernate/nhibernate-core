using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2293
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings => System.Array.Empty<string>();

		[Test]
		public void WhenQueryHasJustAfromThenThrowQuerySyntaxException()
		{
			using (var session = OpenSession())
			{
				Assert.That(() => session.CreateQuery("from").List(), Throws.TypeOf<QuerySyntaxException>());
			}
		}
	}
}
