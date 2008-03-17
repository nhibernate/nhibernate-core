using System.Collections;
using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.Ondelete
{
	[TestFixture, Ignore("Not supported yet")]
	public class OnDeleteFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Ondelete.Person.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			cfg.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		[Test]
		public void JoinedSubclass()
		{
			if (!Dialect.SupportsCircularCascadeDeleteConstraints)
			{
				Assert.Ignore("The dialect don't support 'on delete cascade'");
			}

			IStatistics statistics = sessions.Statistics;
			statistics.Clear();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Salesperson mark = new Salesperson();
			mark.Name = "Mark";
			mark.Title = "internal sales";
			mark.Sex = 'M';
			mark.Address.address = "buckhead";
			mark.Address.zip = "30305";
			mark.Address.country = "USA";

			Person joe = new Person();
			joe.Name = "Joe";
			joe.Address.address = "San Francisco";
			joe.Address.zip = "XXXXX";
			joe.Address.country = "USA";
			joe.Sex = 'M';
			joe.Salesperson = mark;
			mark.Customers.Add(joe);

			s.Save(mark);
			t.Commit();

			Assert.AreEqual(2, statistics.EntityInsertCount);
			Assert.AreEqual(5, statistics.PrepareStatementCount);

			statistics.Clear();

			t = s.BeginTransaction();
			s.Delete(mark);
			t.Commit();

			Assert.AreEqual(2, statistics.EntityDeleteCount);
			Assert.AreEqual(1, statistics.PrepareStatementCount);

			t = s.BeginTransaction();
			IList names = s.CreateQuery("select name from Person").List();
			Assert.AreEqual(0, names.Count);
			t.Commit();

			s.Close();
		}
	}
}
