using System.Diagnostics;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CriteriaFromHql
{
	using System.Collections;

	[TestFixture]
	public class Fixture : TestCase
	{

		protected override IList Mappings
		{
			get { return new string[] { "NHSpecificTest.CriteriaFromHql.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void UsingCriteriaAndHql()
		{
			CreateData();

			using (SqlLogSpy spy = new SqlLogSpy())
			using (ISession session = sessions.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Person result = session.CreateQuery(@"
select p from Person p 
join fetch p.Children c 
join fetch c.Children gc
where p.Parent is null")
					.UniqueResult<Person>();

				string hqlQuery = spy.Appender.GetEvents()[0].MessageObject.ToString();
				Debug.WriteLine("HQL: " + hqlQuery);
				Assertions(result);
			}

			using (SqlLogSpy spy = new SqlLogSpy())
			using (ISession session = sessions.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Person result = session.CreateCriteria(typeof(Person))
					.Add(Restrictions.IsNull("Parent"))
					.SetFetchMode("Children", FetchMode.Join)
					.SetFetchMode("Children.Children", FetchMode.Join)
					.UniqueResult<Person>();
				string criteriaQuery = spy.Appender.GetEvents()[0].MessageObject.ToString();
				Debug.WriteLine("Criteria: " + criteriaQuery);
				Assertions(result);
			}

			DeleteData();
		}

		private void DeleteData()
		{
			using (ISession session = sessions.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				tx.Commit();
			}
		}

		private void CreateData()
		{
			using (ISession session = sessions.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Person root = new Person();
				session.Save(root);
				for (int i = 0; i < 2; i++)
				{
					Person child = new Person();
					root.Children.Add(child);
					child.Parent = root;
					session.Save(child);
					for (int j = 0; j < 3; j++)
					{
						Person child2 = new Person();
						child2.Parent = child;
						child.Children.Add(child2);
						session.Save(child2);
					}
				}
				tx.Commit();
			}
		}

		private static void Assertions(Person p)
		{
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children));
			Assert.AreEqual(2, p.Children.Count);
			foreach (Person child in p.Children)
			{
				Assert.IsTrue(NHibernateUtil.IsInitialized(child));
				Assert.IsTrue(NHibernateUtil.IsInitialized(child.Children));
				Assert.AreEqual(3, child.Children.Count);
			}
		}
	}
}
