using System.Collections;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class WithClauseFixture : BaseFixture
	{
		public ISession OpenNewSession()
		{
			return OpenSession();
		}

		[Test]
		public void WithClauseFailsWithFetch()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			Assert.Throws<SemanticException>(
			  () =>
				s.CreateQuery("from Animal a inner join fetch a.offspring as o with o.bodyWeight = :someLimit").SetDouble(
					"someLimit", 1).List(), "ad-hoc on clause allowed with fetched association");

			txn.Commit();
			s.Close();

			data.Cleanup();
		}

		[Test]
		public void InvalidWithSemantics()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			// PROBLEM : f.bodyWeight is a reference to a column on the Animal table; however, the 'f'
			// alias relates to the Human.friends collection which the aonther Human entity.  The issue
			// here is the way JoinSequence and Joinable (the persister) interact to generate the
			// joins relating to the sublcass/superclass tables
			Assert.Throws<InvalidWithClauseException>(
				() =>
				s.CreateQuery("from Human h inner join h.friends as f with f.bodyWeight < :someLimit").SetDouble("someLimit", 1).
					List());

			Assert.Throws<InvalidWithClauseException>(
				() =>
				s.CreateQuery(
					"from Animal a inner join a.offspring o inner join o.mother as m inner join m.father as f with o.bodyWeight > 1").
					List());

			Assert.Throws<InvalidWithClauseException>(
				() =>
				s.CreateQuery("from Human h inner join h.offspring o with o.mother.father = :cousin").SetEntity("cousin",
				                                                                                                s.Load<Human>(123L))
					.List());

			txn.Commit();
			s.Close();
		}

		[Test]
		public void WithClause()
		{
			var data = new TestData(this);
			data.Prepare();

			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			// one-to-many
				IList list =
				s.CreateQuery("from Human h inner join h.offspring as o with o.bodyWeight < :someLimit").SetDouble("someLimit", 1).
					List();
			Assert.That(list, Is.Empty, "ad-hoc on did not take effect");

			// many-to-one
			list =
				s.CreateQuery("from Animal a inner join a.mother as m with m.bodyWeight < :someLimit").SetDouble("someLimit", 1).
					List();
			Assert.That(list, Is.Empty, "ad-hoc on did not take effect");

			// many-to-many
			list = s.CreateQuery("from Human h inner join h.friends as f with f.nickName like 'bubba'").List();
			Assert.That(list, Is.Empty, "ad-hoc on did not take effect");

			txn.Commit();
			s.Close();

			data.Cleanup();
		}

		private class TestData
		{
			private readonly WithClauseFixture tc;

			public TestData(WithClauseFixture tc)
			{
				this.tc = tc;
			}

			public void Prepare()
			{
				ISession session = tc.OpenNewSession();
				ITransaction txn = session.BeginTransaction();

				var mother = new Human {BodyWeight = 10, Description = "mother"};

				var father = new Human {BodyWeight = 15, Description = "father"};

				var child1 = new Human {BodyWeight = 5, Description = "child1"};

				var child2 = new Human {BodyWeight = 6, Description = "child2"};

				var friend = new Human {BodyWeight = 20, Description = "friend"};

				child1.Mother = mother;
				child1.Father = father;
				mother.AddOffspring(child1);
				father.AddOffspring(child1);

				child2.Mother = mother;
				child2.Father = father;
				mother.AddOffspring(child2);
				father.AddOffspring(child2);

				father.Friends = new[] {friend};

				session.Save(mother);
				session.Save(father);
				session.Save(child1);
				session.Save(child2);
				session.Save(friend);

				txn.Commit();
				session.Close();
			}

			public void Cleanup()
			{
				ISession session = tc.OpenNewSession();
				ITransaction txn = session.BeginTransaction();
				session.CreateQuery("delete Animal where mother is not null").ExecuteUpdate();
				IList humansWithFriends = session.CreateQuery("from Human h where exists(from h.friends)").List();
				foreach (var friend in humansWithFriends)
				{
					session.Delete(friend);
				}
				session.CreateQuery("delete Animal").ExecuteUpdate();
				txn.Commit();
				session.Close();
			}
		}
	}
}