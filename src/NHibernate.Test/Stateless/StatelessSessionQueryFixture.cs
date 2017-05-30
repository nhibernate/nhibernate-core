using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessSessionQueryFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"Stateless.Contact.hbm.xml"}; }
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			cfg.SetProperty(Environment.MaxFetchDepth, 1.ToString());
		}

		private class TestData
		{
			internal readonly IList list = new ArrayList();

			private readonly ISessionFactory sessions;

			public TestData(ISessionFactory sessions)
			{
				this.sessions = sessions;
			}

			public virtual void createData()
			{
				using (ISession session = sessions.OpenSession())
				{
					using (ITransaction tx = session.BeginTransaction())
					{
						var usa = new Country();
						session.Save(usa);
						list.Add(usa);
						var disney = new Org();
						disney.Country = usa;
						session.Save(disney);
						list.Add(disney);
						var waltDisney = new Contact();
						waltDisney.Org = disney;
						session.Save(waltDisney);
						list.Add(waltDisney);
						tx.Commit();
					}
				}
			}

			public virtual void cleanData()
			{
				using (ISession session = sessions.OpenSession())
				{
					using (ITransaction tx = session.BeginTransaction())
					{
						foreach (object obj in list)
						{
							session.Delete(obj);
						}

						tx.Commit();
					}
				}
			}
		}

		[Test]
		public void Criteria()
		{
			var testData = new TestData(Sfi);
			testData.createData();

			using (IStatelessSession s = Sfi.OpenStatelessSession())
			{
				Assert.AreEqual(1, s.CreateCriteria<Contact>().List().Count);
			}

			testData.cleanData();
		}

		[Test]
		public void CriteriaWithSelectFetchMode()
		{
			var testData = new TestData(Sfi);
			testData.createData();

			using (IStatelessSession s = Sfi.OpenStatelessSession())
			{
				Assert.AreEqual(1, s.CreateCriteria<Contact>().SetFetchMode("Org", FetchMode.Select).List().Count);
			}

			testData.cleanData();
		}

		[Test]
		public void Hql()
		{
			var testData = new TestData(Sfi);
			testData.createData();

			using (IStatelessSession s = Sfi.OpenStatelessSession())
			{
				Assert.AreEqual(1, s.CreateQuery("from Contact c join fetch c.Org join fetch c.Org.Country").List<Contact>().Count);
			}

			testData.cleanData();
		}
	}
}