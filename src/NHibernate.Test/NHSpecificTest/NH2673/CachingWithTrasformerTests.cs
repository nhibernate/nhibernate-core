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

namespace NHibernate.Test.NHSpecificTest.NH2673
{
	public class CachingWithTrasformerTests: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ConventionModelMapper();
			mapper.BeforeMapClass += (inspector, type, map) => map.Id(x => x.Generator(Generators.HighLow));
			mapper.BeforeMapClass += (inspector, type, map) => map.Cache(x => x.Usage(CacheUsage.ReadWrite));
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
		public void WhenQueryThenNotThrows()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.CreateQuery("from Blog b where b.Author = : author")
						.SetString("author", "Gabriel")
						.SetCacheable(true)
						.SetResultTransformer(new DistinctRootEntityResultTransformer());
					query.Executing(q=> q.List<Blog>()).NotThrows();
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenCriteriaThenNotThrows()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
						.TransformUsing(new DistinctRootEntityResultTransformer())
						.Cacheable();
					query.Executing(q => q.List<Blog>()).NotThrows();
					tx.Commit();
				}
			}
		}

		private class BlogAuthorDto
		{
			public string BlogName { get; set; }
			public string AuthorName { get; set; }
		}

		private class BlogAuthorTrasformer : IResultTransformer
		{
			public object TransformTuple(object[] tuple, string[] aliases)
			{
				return new BlogAuthorDto { BlogName = tuple[0].ToString(), AuthorName = tuple[1].ToString() };
			}

			public IList TransformList(IList collection)
			{
				return collection;
			}
		}

		[Test]
		public void WhenCriteriaProjectionThenNotThrows()
		{
			// during the fix of NH-2673 was faund another bug related to cacheability of criteria with projection + trasformer 
			// then found reported as NH-1090
			var transformer = new BlogAuthorTrasformer();
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.QueryOver<Blog>().Select(x=> x.Author, x=> x.Name).Where(x => x.Author == "Gabriel")
						.TransformUsing(transformer)
						.Cacheable();
					query.List<BlogAuthorDto>();
					tx.Commit();
				}
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.QueryOver<Blog>().Select(x => x.Author, x => x.Name).Where(x => x.Author == "Gabriel")
						.TransformUsing(transformer)
						.Cacheable();
					query.Executing(q => q.List<BlogAuthorDto>()).NotThrows();
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenEagerLoadingWithCriteriaThenNotThrows()
		{
			// reported in dev-list instead on JIRA
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.CreateCriteria<Blog>()
						.SetFetchMode("Posts", FetchMode.Eager)
						.SetCacheable(true);
					query.Executing(q => q.List<Blog>()).NotThrows();
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenEagerLoadingWithMultiCriteriaThenNotThrows()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.CreateCriteria<Blog>()
						.SetFetchMode("Posts", FetchMode.Eager)
						.SetCacheable(true);
					query.Executing(q => q.Future<Blog>().ToList()).NotThrows();
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenEagerLoadingWithHqlThenNotThrows()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction())
				{
					var query = session.CreateQuery("select b from Blog b join fetch b.Posts where b.Author = : author")
						.SetString("author", "Gabriel")
						.SetCacheable(true);
					query.Executing(q => q.List<Blog>()).NotThrows();
					tx.Commit();
				}
			}
		}
	}
}