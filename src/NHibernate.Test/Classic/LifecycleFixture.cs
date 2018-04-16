using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Classic
{
	[TestFixture]
	public class LifecycleFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Classic.EntityWithLifecycle.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void Save()
		{
			Sfi.Statistics.Clear();
			using (ISession s = OpenSession())
			{
				s.Save(new EntityWithLifecycle());
				s.Flush();
			}
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(0));

			var v = new EntityWithLifecycle("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Delete(v);
				s.Flush();
			}
		}

		[Test]
		public void Update()
		{
			var v = new EntityWithLifecycle("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
			}

			// update detached
			Sfi.Statistics.Clear();
			v.Heigth = 0;
			using (ISession s = OpenSession())
			{
				s.Update(v);
				s.Flush();
			}
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(0));

			// cleanup
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityWithLifecycle").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void Merge()
		{
			var v = new EntityWithLifecycle("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
			}
			v.Heigth = 0;
			Sfi.Statistics.Clear();
			using (ISession s = OpenSession())
			{
				s.Merge(v);
				s.Flush();
			}
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(0));

			var v1 = new EntityWithLifecycle("Shinobi", 0, 10);
			using (ISession s = OpenSession())
			{
				s.Merge(v1);
				s.Flush();
			}
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(0));


			// cleanup
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityWithLifecycle").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void Delete()
		{
			var v = new EntityWithLifecycle("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
				Sfi.Statistics.Clear();
				v.Heigth = 0;
				s.Delete(v);
				s.Flush();
				Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(0));
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityWithLifecycle").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}