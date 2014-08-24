using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1235
{
	public class SomeClass
	{
		private int id;
		private string name;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1235"; }
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			// Specific to MsSql2000Dialect. Does not apply to MsSql2005Dialect
			return dialect.GetType().Equals(typeof (MsSql2000Dialect));
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from SomeClass");
				tx.Commit();
			}
		}

		[Test]
		public void Test()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				SomeClass obj;

				for (int i = 0; i < 10; i++)
				{
					obj = new SomeClass();
					obj.Name = "someclass " + (i + 1).ToString();
					s.Save(obj);
				}

				tx.Commit();
			}


			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IQuery qry = s.CreateQuery("from SomeClass")
					.SetMaxResults(5);

				IList<SomeClass> list = qry.List<SomeClass>();

				Assert.AreEqual(5, list.Count, "Should have returned 5 entities");

				tx.Commit();
			}
		}

	}
}
