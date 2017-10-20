using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3909
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<ParentEntity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			mapper.Class<ChildEntity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent, m =>
				{
					m.Column("Parent");
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new ParentEntity { Name = "Parent" };
				session.Save(e1);

				var e2 = new ChildEntity { Name = "ChildWithParent", Parent = e1 };
				session.Save(e2);

				var e3 = new ChildEntity { Name = "ChildWithoutParent", Parent = null };
				session.Save(e3);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from ParentEntity");
				session.Delete("from ChildEntity");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void YourTestName()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var q = from b in session.Query<ChildEntity>()
						select new
						{
							Id = b.Id,
							Name = b.Name,
							Parent = b.Parent
						};
				//select b;

				Assert.AreEqual(2, q.ToList().Count);

			}
		}
	}
}
