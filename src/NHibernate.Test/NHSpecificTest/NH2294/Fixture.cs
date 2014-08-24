using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;
using SharpTestsEx;

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
				session.Executing(s => s.CreateQuery("where").List()).Throws<QuerySyntaxException>();
			}
		}
	}
}