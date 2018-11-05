using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2294
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings => System.Array.Empty<string>();

		[Test, Ignore("External issue. The bug is inside RecognitionException of Antlr.")]
		public void WhenQueryHasJustAWhereThenThrowQuerySyntaxException()
		{
			using (ISession session = OpenSession())
			{
				Assert.That(() => session.CreateQuery("where").List(), Throws.TypeOf<QuerySyntaxException>());
			}
		}
	}
}
