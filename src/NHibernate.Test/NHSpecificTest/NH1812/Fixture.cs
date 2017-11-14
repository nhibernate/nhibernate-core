using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1812
{
	public class AstBug : BugTestCase
	{
		/* to the nh guy...
		* sorry for not coming up with a more realistic use case
		* We have a query that works fine with the old parser but not with the new AST parser
		* I've broke our complex query down to this... 
		* I believe the problem is when mixing aggregate methods with isnull()
		*/

		[Test]
		public void Test()
		{
			var p = new Person();

			const string query =
				@"select p from Person p
					left outer join p.PeriodCollection p1
				where p1.Start > coalesce((select max(p2.Start) from Period p2), :nullStart)";

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(p);
					tx.Commit();
				}

				s.CreateQuery(query)
					.SetDateTime("nullStart", new DateTime(2001, 1, 1))
					.List<Person>();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return Dialect.SupportsScalarSubSelects;
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from Person");
					tx.Commit();
				}
			}
		}
	}
}