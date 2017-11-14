using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Insertordering.FamilyModel
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "Insertordering.FamilyModel.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.DataBaseIntegration(x =>
			{
				x.BatchSize = 10;
				x.OrderInserts = true;
				// Uncomment batcher lines here and in OnSetUp and OnTearDown for debugging purpose.
				//x.Batcher<InsertOrderingFixture.StatsBatcherFactory>();
			});
		}

		protected override void OnSetUp()
		{
			/*
			InsertOrderingFixture.StatsBatcher.Reset();
			InsertOrderingFixture.StatsBatcher.StatsEnabled = true;
			*/
		}

		protected override void OnTearDown()
		{
			//InsertOrderingFixture.StatsBatcher.StatsEnabled = false;

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				tran.Commit();
			}
		}

		[Test]
		public void CircularReferences()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var jeanne = new Woman { Name = "Jeanne" };
				var paul = new Man { Name = "Paul" };

				var paulette = new Woman { Name = "Paulette", Mother = jeanne, Father = paul };
				var monique = new Woman { Name = "Monique", Mother = jeanne, Father = paul };
				var alice = new Woman { Name = "Alice", Mother = jeanne, Father = paul };
				var yves = new Man { Name = "Yves", Mother = jeanne, Father = paul };
				var denis = new Man { Name = "Denis", Mother = jeanne, Father = paul };

				var laure = new Woman { Name = "Laure", Mother = paulette };
				var valerie = new Woman { Name = "Valérie", Mother = paulette };
				var caroline = new Woman { Name = "Caroline", Mother = monique };
				var cathy = new Woman { Name = "Cathy", Father = yves };
				var helene = new Woman { Name = "Hélène", Father = yves };
				var nicolas = new Man { Name = "Nicolas", Mother = monique };
				var frederic = new Man { Name = "Frédéric", Mother = monique };
				var arnaud = new Man { Name = "Arnaud", Father = denis };

				session.Save(alice);
				session.Save(laure);
				session.Save(valerie);
				session.Save(caroline);
				session.Save(cathy);
				session.Save(helene);
				session.Save(nicolas);
				session.Save(frederic);
				session.Save(arnaud);

				Assert.DoesNotThrow(() => { tran.Commit(); });
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				Assert.AreEqual(9, session.Query<Woman>().Count());
				Assert.AreEqual(6, session.Query<Man>().Count());
			}
		}
	}
}