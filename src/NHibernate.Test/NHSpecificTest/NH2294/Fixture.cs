using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2294
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