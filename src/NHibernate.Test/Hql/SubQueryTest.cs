using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	[TestFixture]
	public class SubQueryTest : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Root>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.RootName);
					rc.ManyToOne(x => x.Branch);
				});

			mapper.Class<Branch>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.BranchName);
					rc.Bag(x => x.Leafs, cm => cm.Cascade(Mapping.ByCode.Cascade.All), x => x.OneToMany());
				});
			mapper.Class<Leaf>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Native));
					rc.Property(x => x.LeafName);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[TestCase("SELECT CASE WHEN l.id IS NOT NULL THEN (SELECT COUNT(r.id) FROM Root r) ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE WHEN (SELECT COUNT(r.id) FROM Root r) > 1 THEN 1 ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE WHEN  l.id > 1 THEN 1 ELSE (SELECT COUNT(r.id) FROM Root r) END FROM Leaf l")]
		[TestCase("SELECT CASE (SELECT COUNT(r.id) FROM Root r) WHEN  1 THEN 1 ELSE 0 END FROM Leaf l")]
		[TestCase("SELECT CASE l.id WHEN (SELECT COUNT(r.id) FROM Root r) THEN 1 ELSE 0 END FROM Leaf l")]
		public void TestSubQuery(string query)
		{
			if (!Dialect.SupportsScalarSubSelects)
			{
				Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Simple syntax check
				session.CreateQuery(query).List();
				transaction.Commit();
			}
		}
	}
}
