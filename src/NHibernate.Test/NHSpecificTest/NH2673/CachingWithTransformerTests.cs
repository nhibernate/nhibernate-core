using System;
using System.Collections;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2673
{
	public class CachingWithTransformerTests: TestCaseMappingByCode
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
					                   .SetResultTransformer(new DistinctRootEntityResultTransformer())
					                   .List<Blog>();
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
					                   .Cacheable()
					                   .List<Blog>();
					tx.Commit();
				}
			}
		}

		private class BlogAuthorDto
		{
			public string BlogName { get; set; }
			public string AuthorName { get; set; }
		}

		private class BlogAuthorTransformer : IResultTransformer
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
			// during the fix of NH-2673 was faund another bug related to cacheability of criteria with projection + transformer 
			// then found reported as NH-1090
			var transformer = new BlogAuthorTransformer();
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
					                   .Cacheable()
					                   .List<BlogAuthorDto>();
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
					                   .SetCacheable(true)
					                   .List<Blog>();
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
					                   .SetCacheable(true)
					                   .Future<Blog>()
					                   .ToList();
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
					                   .SetCacheable(true)
					                   .List<Blog>();
					tx.Commit();
				}
			}
		}

		
		[Test(Description = "NH2961/3311")]
		public void CanCacheCriteriaWithLeftJoinAndResultTransformer()
		{
			Post posts = null;

			using (new Scenario(Sfi))
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
									.Left.JoinAlias(x => x.Posts, () => posts)
									.TransformUsing(new DistinctRootEntityResultTransformer())
									.Cacheable()
									.List<Blog>();
			}
		}


		[Test(Description = "NH2961/3311")]
		public void CanCacheCriteriaWithEagerLoadAndResultTransformer()
		{
			using (new Scenario(Sfi))
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
									.Fetch(x => x.Posts).Eager
									.TransformUsing(new DistinctRootEntityResultTransformer())
									.Cacheable()
									.List<Blog>();
			}
		}

		
		[Test(Description = "NH2961/3311")]
		public void CanCacheCriteriaWithLeftJoin()
		{
			Post posts = null;
			// Begins to work in 6e21608bbdec096558da956b9df41ab1d63dbd85.
			// Same as CanCacheCriteriaWithLeftJoinAndResultTransformer() but without
			// result transformer.

			using (new Scenario(Sfi))
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query = session.QueryOver<Blog>().Where(x => x.Author == "Gabriel")
				                   .Left.JoinAlias(x => x.Posts, () => posts)
				                   .Cacheable()
				                   .List<Blog>();
			}
		}
	}
}