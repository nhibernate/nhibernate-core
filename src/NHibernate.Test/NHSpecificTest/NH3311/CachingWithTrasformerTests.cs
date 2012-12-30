using System;
using System.Collections;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3311
{
	public class CachingWithTrasformerTests: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ConventionModelMapper();
			mapper.BeforeMapClass += (inspector, type, map) => map.Id(x => x.Generator(Generators.HighLow));
			mapper.BeforeMapClass += (inspector, type, map) => map.Cache(x => x.Usage(CacheUsage.ReadWrite));
			mapper.BeforeMapClass += (inspector, type, map) => map.Table(type.Name + "s");
			mapper.BeforeMapSet += (inspector, property, map) =>
								   {
									map.Cascade(Mapping.ByCode.Cascade.All);
									map.Cache(x => x.Usage(CacheUsage.ReadWrite));
								   };
			var mapping = mapper.CompileMappingFor(new[] { typeof(Blog), typeof(Post), typeof(Comment) });
			return mapping;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.Cache(x =>
								{
									x.Provider<HashtableCacheProvider>();
									x.UseQueryCache = true;
								});
		}

		private class Scenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (var session= factory.OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var blog = new Blog { Author = "Gabriel", Name = "Keep on running" };
					blog.Posts.Add(new Post { Title = "First post", Body = "Some text" });
					blog.Posts.Add(new Post { Title = "Second post", Body = "Some other text" });
					blog.Posts.Add(new Post { Title = "Third post", Body = "Third post text" });


					blog.Comments.Add(new Comment { Title = "First comment", Body = "Some text" });
					blog.Comments.Add(new Comment { Title = "Second comment", Body = "Some other text" });
					session.Save(blog);
					tx.Commit();
				}
			}

			public void Dispose()
			{
				using (var session = factory.OpenSession())
				using (var tx = session.BeginTransaction())
				{
					session.CreateQuery("delete from Comment").ExecuteUpdate();
					session.CreateQuery("delete from Post").ExecuteUpdate();
					session.CreateQuery("delete from Blog").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

        [Test]
        public void WhenLeftJoinDistinctRootEntityCriteriaThenThrow()
        {
            Post posts = null;

            using (new Scenario(Sfi))
            {
                using (var session = OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var query = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
                        .Left.JoinAlias(x => x.Posts, () => posts)
                        .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Cacheable();
                    query.Executing(q => q.List<Blog>()).NotThrows();
                    tx.Commit();
                }
            }
        }

        [Test]
        public void WhenEagerLoadingDistinctRootEntityCriteriaThenThrow()
        {
            using (new Scenario(Sfi))
            {
                using (var session = OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var query = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
                        .Fetch(x => x.Posts).Eager
                        .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Cacheable();
                    query.Executing(q => q.List<Blog>()).NotThrows();
                    tx.Commit();
                }
            }
        }

        [Test]
        public void WhenLeftJoinCriteriaThenNotThrow()
        {
            Post posts = null;

            using (new Scenario(Sfi))
            {
                using (var session = OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var query = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
                        .Left.JoinAlias(x => x.Posts, () => posts)
                        // .TransformUsing(new DistinctRootEntityResultTransformer())
                        .Cacheable();
                    query.Executing(q => q.List<Blog>()).NotThrows();
                    tx.Commit();
                }
            }
        }
	}
}