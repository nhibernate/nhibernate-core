using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2587
{
	public class Foo
	{
		public Foo()
		{
			Bars = new List<Bar>();
		}
		public virtual Guid Id { get; set; }
		public virtual ICollection<Bar> Bars { get; set; }
	}

	public class Bar
	{
		public virtual Guid Id { get; set; }
		public virtual Foo Foo { get; set; }
	}

	public class CachingWithLinq : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.BeforeMapClass += (inspector, type, map) => map.Id(x=> x.Generator(Generators.GuidComb));
			mapper.Class<Foo>(mc =>
			                  {
													mc.Id(x => x.Id);
													mc.Bag(x => x.Bars, map =>
													                    {
													                    	map.Inverse(true);
													                    	map.Cascade(Mapping.ByCode.Cascade.All);
																								map.Key(km =>
																								        {
																								        	km.Column("FooId");
																													km.OnDelete(OnDeleteAction.Cascade);
																								        });
													                    }, rel => rel.OneToMany());
			                  });
			mapper.Class<Bar>(mc =>
			                  {
													mc.Id(x => x.Id);
			                  	mc.ManyToOne(x=> x.Foo, map=> map.Column("FooId"));
			                  });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.Cache(x =>
			                    {
														x.Provider<HashtableCacheProvider>();
														x.UseQueryCache = true;
			                    });
		}

		public class Scenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession session = factory.OpenSession())
				using (ITransaction tx = session.BeginTransaction())
				{
					var foo1 = new Foo();
					foo1.Bars.Add(new Bar {Foo = foo1});
					foo1.Bars.Add(new Bar { Foo = foo1 });
					var foo2 = new Foo();
					foo2.Bars.Add(new Bar { Foo = foo2 });

					session.Persist(foo1);
					session.Persist(foo2);
					tx.Commit();
				}
			}

			public void Dispose()
			{
				using (ISession session = factory.OpenSession())
				using (ITransaction tx = session.BeginTransaction())
				{
					session.CreateQuery("delete from Foo").ExecuteUpdate();
					tx.Commit();
				}
			}
		}
		[Test]
		public void TestMethod1()
		{
			using (new Scenario(Sfi))
			{
				// The test provided is only about Not-Throw
				using (ISession session = OpenSession())
				using (ITransaction transaction = session.BeginTransaction())
				{

					session.Query<Foo>().Cacheable().ToList();
					session.Query<Bar>().Cacheable().ToList();
					session.Query<Foo>().Cacheable().Fetch(x => x.Bars).ToList();
					session.Query<Bar>().Cacheable().Fetch(x => x.Foo).ToList();
				}
			}
		}
	}
}