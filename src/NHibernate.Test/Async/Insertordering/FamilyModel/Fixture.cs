﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Insertordering.FamilyModel
{
	[TestFixture]
	public class FixtureAsync : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "Insertordering.FamilyModel.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsBatchingDependentDML;
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.ByCode().DataBaseIntegration(x =>
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
		public async System.Threading.Tasks.Task CircularReferencesAsync()
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

				await (session.SaveAsync(alice));
				await (session.SaveAsync(laure));
				await (session.SaveAsync(valerie));
				await (session.SaveAsync(caroline));
				await (session.SaveAsync(cathy));
				await (session.SaveAsync(helene));
				await (session.SaveAsync(nicolas));
				await (session.SaveAsync(frederic));
				await (session.SaveAsync(arnaud));

				Assert.DoesNotThrowAsync(() => { return tran.CommitAsync(); });
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				Assert.AreEqual(9, await (session.Query<Woman>().CountAsync()));
				Assert.AreEqual(6, await (session.Query<Man>().CountAsync()));
			}
		}
	}
}
