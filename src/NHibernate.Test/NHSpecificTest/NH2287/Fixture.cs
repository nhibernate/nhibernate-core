using System;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;
using SharpTestsEx;

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
				session.Executing(s => s.CreateQuery(query).List()).Throws<QuerySyntaxException>();
			}
		}
	}
}
