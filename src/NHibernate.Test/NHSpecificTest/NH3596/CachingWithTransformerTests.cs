using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3596
{
	public class EntityBase
	{
		public virtual int Id { get; set; }
	}

	public class Role : EntityBase
	{
		public virtual string Name { get; set; }
		public virtual Role Parent { get; set; }
		public virtual IList<Role> Children { get; set; }
	}

	public class RoleMap : ClassMapping<Role>
	{
		public RoleMap()
		{
			Cache(x => x.Usage(CacheUsage.ReadOnly));
			Id(x => x.Id, x => x.Generator(Generators.Identity));
			Property(x => x.Name);
			ManyToOne(x => x.Parent, x =>
			{
				x.Column("ParentId");
				x.NotNullable(false);
			});
			Bag(x => x.Children, x =>
			{
				x.Cascade(Mapping.ByCode.Cascade.All);
				x.Key(y => y.Column("ParentId"));
			}, x => x.OneToMany());
		}
	}

	public class CachingWithTransformerTests: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<RoleMap>();
			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mapping;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var parentRoles = new List<Role>()
                {
                    new Role() { Name = "Admin", Children = null, Parent = null },
                    new Role() { Name = "Manager", Children = null, Parent = null },
                    new Role() { Name = "Support", Children = null, Parent = null }
                };

				var childRoles = new List<Role>()
                {
                    new Role() { Name = "Manager-Secretary", Children = null, Parent = parentRoles.FirstOrDefault(x => x.Name == "Manager") },
                    new Role() { Name = "Superviser", Children = null, Parent = parentRoles.FirstOrDefault(x => x.Name == "Manager") }
                };

				foreach (var parentRole in parentRoles)
				{
					parentRole.Children = new List<Role>(childRoles.Where(x => x.Parent == parentRole));

					session.Save(parentRole);
				}

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

		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);

			configuration.Cache(x =>
								{
									x.Provider<HashtableCacheProvider>();
									x.UseQueryCache = true;
								});
		}

		[Test]
		public void UsingQueryOverToFutureWithCacheAndTransformerDoesntThrow()
		{
			//NH-3596
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				Role children = null;
				Role parent = null;

				//store values in cache
				session
					.QueryOver<Role>()
					.Where(x => x.Parent == null)
					.Left.JoinAlias(x => x.Children, () => children)
					.Left.JoinAlias(x => x.Parent, () => parent)
					.TransformUsing(Transformers.DistinctRootEntity)
					.Cacheable()
					.CacheMode(CacheMode.Normal)
					.Future();

				//get values from cache
				var roles = session
					.QueryOver<Role>()
					.Where(x => x.Parent == null)
					.Left.JoinAlias(x => x.Children, () => children)
					.Left.JoinAlias(x => x.Parent, () => parent)
					.TransformUsing(Transformers.DistinctRootEntity)
					.Cacheable()
					.CacheMode(CacheMode.Normal)
					.Future();

				var result = roles.ToList();

				Assert.AreEqual(3, result.Count);
			}
		}

		[Test]
		public void UsingHqlToFutureWithCacheAndTransformerDoesntThrow()
		{
			//NH-3596
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				//store values in cache
				session
					.CreateQuery("select r from Role r left join r.Children left join r.Parent where r.Parent is null")
					.SetResultTransformer(Transformers.DistinctRootEntity)
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Normal)
					.Future<Role>();

				//get values from cache
				var roles = session
					.CreateQuery("select r from Role r left join r.Children left join r.Parent where r.Parent is null")
					.SetResultTransformer(Transformers.DistinctRootEntity)
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Normal)
					.Future<Role>();

				var result = roles.ToList();

				Assert.AreEqual(3, result.Count);
			}
		}
	}
}