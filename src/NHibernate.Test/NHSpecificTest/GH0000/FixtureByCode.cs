﻿using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0000
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var e1 = new Entity { Name = "Bob" };
			session.Save(e1);

			var e2 = new Entity { Name = "Sally" };
			session.Save(e2);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			// The HQL delete does all the job inside the database without loading the entities, but it does
			// not handle delete order for avoiding violating constraints if any. Use
			// session.Delete("from System.Object");
			// instead if in need of having NHbernate ordering the deletes, but this will cause
			// loading the entities in the session.
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void YourTestName()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var result = session
				.Query<Entity>()
				.Where(e => e.Name == "Bob");

			Assert.That(result.ToList(), Has.Count.EqualTo(1));

			transaction.Commit();
		}
	}
}
