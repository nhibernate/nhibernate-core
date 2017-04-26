using System;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2287
{
	public class Fixture: BugTestCase
	{
		[Test]
		public void DotInStringLiteralsConstant()
		{
			using (ISession session = OpenSession())
			{
				var query = string.Format("from Foo f {0}where f.", Environment.NewLine);
				Assert.That(() => session.CreateQuery(query).List(), Throws.TypeOf<QuerySyntaxException>());
			}
		}
	}
}
