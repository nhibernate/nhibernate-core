using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3324
{
	public class FixtureByCode : TestCaseMappingByCode
	{
		[Test]
		public void LeftOuterJoin()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				const string hql = "FROM Entity e LEFT OUTER JOIN FETCH e.Children";

				var query = session.CreateQuery(hql);
				var result = query.List(); // does work
				Assert.That(result, Has.Count.GreaterThan(0));
			}
		}

		[Test]
		public void LeftOuterJoinSetMaxResults()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				const string hql = "FROM Entity e LEFT OUTER JOIN FETCH e.Children";

				var query = session.CreateQuery(hql);
				query.SetMaxResults(5);
				var result = query.List(); // does not work
				Assert.That(result, Has.Count.GreaterThan(0));
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(rc =>
				{
					rc.Id(x => x.Id,
						  m =>
							  {
								  m.Generator(Generators.Identity);
								  m.Length(4);
							  });
					rc.Property(x => x.Name);
					rc.Bag(x => x.Children,
						   c =>
							   {
								   c.Key(k =>
									   {
										   k.Column("EntityId");
										   k.NotNullable(false);
										   k.ForeignKey("Id");
									   });
								   c.Cascade(Mapping.ByCode.Cascade.All);
								   c.Table("ChildEntity");
								   c.Inverse(true);
							   },
						   t => t.OneToMany());
				});

			mapper.Class<ChildEntity>(rc =>
				{
					rc.Id(x => x.Id,
						  m =>
							  {
								  m.Generator(Generators.Identity);
								  m.Length(4);
							  });
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				e1.Children.Add(new ChildEntity { Name = "Bob's Child" });
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				e2.Children.Add(new ChildEntity { Name = "Sally's Child" });
				session.Save(e2);

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
	}
}
