using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH1845
{
	public class Fixture : TestCaseMappingByCode
	{

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Category>(rc =>
								   {
									   rc.Id(x => x.Id, map => map.Generator(Generators.Native));
									   rc.Property(x => x.Name);
									   rc.ManyToOne(x => x.Parent, map => map.Column("ParentId"));
									   rc.Bag(x => x.Subcategories, map =>
																	{
																		map.Access(Accessor.NoSetter);
																		map.Key(km => km.Column("ParentId"));
																		map.Cascade(Mapping.ByCode.Cascade.All.Include(Mapping.ByCode.Cascade.DeleteOrphans));
																	}, rel => rel.OneToMany());
								   });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		[Test]
		public void LazyLoad_Initialize_AndEvict()
		{
			Category category = new Category("parent");
			category.AddSubcategory(new Category("child"));
			SaveCategory(category);

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Category loaded = session.Load<Category>(category.Id);
				NHibernateUtil.Initialize(loaded.Subcategories[0]);
				session.Evict(loaded);
				transaction.Commit();
				Assert.AreEqual("child", loaded.Subcategories[0].Name, "cannot access child");
			}
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				// first delete children
				session.CreateQuery("delete from Category where Parent != null").ExecuteUpdate();
				// then the rest
				session.CreateQuery("delete from Category").ExecuteUpdate();
				transaction.Commit();
			}
		}

		private void SaveCategory(Category category)
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.SaveOrUpdate(category);
				transaction.Commit();
			}
		}
	}
}