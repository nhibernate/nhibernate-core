using System.Linq;
using NHibernate.Cfg;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class LimitClauseFixture : BaseFixture
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsVariableLimit
				&& !(Dialect is Dialect.MsSql2000Dialect &&  cfg.Properties[Environment.ConnectionDriver] == typeof(Driver.OdbcDriver).FullName); // don't know why, but these tests don't work on SQL Server using ODBC
		}

		protected override void OnSetUp()
		{
			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			var mother = new Human {BodyWeight = 10, Description = "mother"};
			var father = new Human {BodyWeight = 15, Description = "father"};
			var child1 = new Human {BodyWeight = 5, Description = "child1"};
			var child2 = new Human {BodyWeight = 6, Description = "child2"};
			var friend = new Human {BodyWeight = 20, Description = "friend"};

			session.Save(mother);
			session.Save(father);
			session.Save(child1);
			session.Save(child2);
			session.Save(friend);

			txn.Commit();
			session.Close();
		}

		protected override void OnTearDown()
		{
			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();
			session.Delete("from Animal");
			txn.Commit();
			session.Close();
		}

		[Test]
		public void None()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h order by h.bodyWeight").List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {5, 6, 10, 15, 20};
			CollectionAssert.AreEqual(expected, actual);

			txn.Commit();
			s.Close();
		}

		[Test]
		public void Skip()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h where h.bodyWeight > :minW order by h.bodyWeight skip 2").SetDouble("minW", 0d).List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {10, 15, 20};
			CollectionAssert.AreEqual(expected, actual);

			txn.Commit();
			s.Close();
		}

		[Test]
		public void SkipTake()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h order by h.bodyWeight skip 1 take 3").List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {6, 10, 15};
			CollectionAssert.AreEqual(expected, actual);

			txn.Commit();
			s.Close();
		}

		[Test]
		public void SkipTakeWithParameter()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h order by h.bodyWeight skip :pSkip take :pTake")
				.SetInt32("pSkip", 1)
				.SetInt32("pTake", 3).List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {6f, 10f, 15f};
			Assert.That(actual, Is.EquivalentTo(expected));

			txn.Commit();
			s.Close();
		}

		[Test]
		public void SkipTakeWithParameterList()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h where h.bodyWeight in (:list) order by h.bodyWeight skip :pSkip take :pTake")
				.SetParameterList("list", new[] {10f, 15f, 5f})
				.SetInt32("pSkip", 1)
				.SetInt32("pTake", 4).List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {10f, 15f};
			Assert.That(actual, Is.EquivalentTo(expected));

			txn.Commit();
			s.Close();
		}

		[Test]
		public void SkipWithParameter()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h order by h.bodyWeight skip :jump").SetInt32("jump", 2).List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {10f, 15f, 20f};
			Assert.That(actual, Is.EquivalentTo(expected));

			txn.Commit();
			s.Close();
		}

		[Test]
		public void Take()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h order by h.bodyWeight take 2").List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {5, 6};
			CollectionAssert.AreEqual(expected, actual);

			txn.Commit();
			s.Close();
		}

		[Test]
		public void TakeSkip()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			Assert.Throws<QuerySyntaxException>(() => s.CreateQuery("from Human h order by h.bodyWeight take 1 skip 2").List<Human>(), "take should not be allowed before skip");

			txn.Commit();
			s.Close();
		}

		[Test]
		public void TakeWithParameter()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			float[] actual = s.CreateQuery("from Human h where h.bodyWeight > :minW order by h.bodyWeight take :jump")
				.SetDouble("minW", 1d)
				.SetInt32("jump", 2).List<Human>().Select(h => h.BodyWeight).ToArray();
			var expected = new[] {5, 6};
			CollectionAssert.AreEqual(expected, actual);

			txn.Commit();
			s.Close();
		}
	}
}