using System.Collections;
using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.Ondelete
{
	[TestFixture]
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

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect.SupportsCircularCascadeDeleteConstraints;
		}

		[Test]
		public void JoinedSubclass()
		{
			IStatistics statistics = Sfi.Statistics;
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
			Assert.IsTrue(5 >= statistics.PrepareStatementCount);

			statistics.Clear();

			t = s.BeginTransaction();
			s.Delete(mark);
			t.Commit();

			Assert.AreEqual(2, statistics.EntityDeleteCount);
			Assert.AreEqual(1, statistics.PrepareStatementCount);

			t = s.BeginTransaction();
			IList names = s.CreateQuery("select p.name from Person p").List();
			Assert.AreEqual(0, names.Count);
			t.Commit();

			s.Close();
		}
	}
}
