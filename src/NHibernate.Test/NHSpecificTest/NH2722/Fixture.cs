using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2722
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		private readonly string[] _mappings = new[] {"NHSpecificTest.NH2722.Mappings.hbm.xml"};

		protected override IList Mappings
		{
			get { return _mappings; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.ShowSql, "true");
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity {Property = null});
				session.Save(new Entity {Property = 1});
				session.Save(new Entity {Property = 1});
				session.Save(new Entity {Property = 2});
				session.Save(new Entity {Property = 3});
				session.Save(new Entity {Property = 4});

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(x => x.Property)
					.Distinct()
					.Count();

				Assert.That(result, Is.EqualTo(4));
			}
		}

		[Test]
		public void CountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(x => x.Property)
					.Count();

				Assert.That(result, Is.EqualTo(5));
			}
		}

		[Test]
		public void Count_ReturnsNumberOfRecords()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Count();

				Assert.That(result, Is.EqualTo(6));
			}
		}

		[Test]
		public void LongCountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(x => x.Property)
					.Distinct()
					.LongCount();

				Assert.That(result, Is.EqualTo(4));
			}
		}

		[Test]
		public void LongCountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(x => x.Property)
					.LongCount();

				Assert.That(result, Is.EqualTo(5));
			}
		}

		[Test]
		public void LongCount_ReturnsNumberOfRecords()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.LongCount();

				Assert.That(result, Is.EqualTo(6));
			}
		}
	}
}