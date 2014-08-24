using NUnit.Framework;

using System;
using System.Collections;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1064
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1064"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from TypeC");
				s.Delete("from TypeB");
				s.Delete("from TypeA");

				tx.Commit();
			}
		}

		[Test]
		public void JoinFetch()
		{
			TypeA a1;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				a1 = new TypeA("a1");
				a1.C = new TypeC("c1", "c1");
				s.Save(a1.C);
				s.Save(a1);

				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				ICriteria crit = s.CreateCriteria(typeof(TypeA))
					.SetFetchMode("Bs", FetchMode.Join)
					.SetFetchMode("C", FetchMode.Join);
				// According to the issue description, the following line
				// would have thown an NHibernate.ADOException before the fix
				IList result = crit.List();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(a1.Id, (result[0] as TypeA).Id);
				Assert.AreEqual(a1.Name, (result[0] as TypeA).Name);
				Assert.AreEqual(a1.C.Id, (result[0] as TypeA).C.Id);
				Assert.AreEqual(a1.C.Name, (result[0] as TypeA).C.Name);

				tx.Commit();
			}
		}
	}
}
