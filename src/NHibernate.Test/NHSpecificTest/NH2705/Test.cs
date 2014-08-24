using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

// ReSharper disable InconsistentNaming

namespace NHibernate.Test.NHSpecificTest.NH2705
{
	[TestFixture]
	public class Test : BugTestCase
	{
		private static IEnumerable<T> GetAndFetch<T>(string name, ISession session) where T : ItemBase
		{
			// this is a valid abstraction, the calling code should be able to ask that a property is eagerly loaded/available
			// without having to know how it is mapped
			return session.Query<T>()
				.Fetch(p => p.SubItem).ThenFetch(p => p.Details) // should be able to fetch .Details when used with components (NH2615)
				.Where(p => p.SubItem.Name == name).ToList();
		}

		[Test]
		public void Fetch_OnComponent_ShouldNotThrow()
		{
			using (ISession s = OpenSession())
			{
				Executing.This(() => GetAndFetch<ItemWithComponentSubItem>("hello", s)).Should().NotThrow();
			}
		}

		[Test]
		public void HqlQueryWithFetch_WhenDerivedClassesUseComponentAndManyToOne_DoesNotGenerateInvalidSql()
		{
			using (ISession s = OpenSession())
			{
				using (var log = new SqlLogSpy())
				{
					Executing.This(() => s.CreateQuery("from ItemWithComponentSubItem i left join fetch i.SubItem").List()
						).Should().NotThrow();
				}
			}
		}

		[Test]
		public void HqlQueryWithFetch_WhenDerivedClassesUseComponentAndEagerFetchManyToOne_DoesNotGenerateInvalidSql()
		{
			using (ISession s = OpenSession())
			{
				using (var log = new SqlLogSpy())
				{
					Executing.This(() => s.CreateQuery("from ItemWithComponentSubItem i left join fetch i.SubItem.Details").List()
						).Should().NotThrow();
				}
			}
		}

		[Test]
		public void LinqQueryWithFetch_WhenDerivedClassesUseComponentAndManyToOne_DoesNotGenerateInvalidSql()
		{
			using (ISession s = OpenSession())
			{
				using (var log = new SqlLogSpy())
				{
					Executing.This(() => s.Query<ItemBase>()
					                     	.Fetch(p => p.SubItem).ToList()
						).Should().NotThrow();


					// fetching second level properties should work too
					Executing.This(() => s.Query<ItemWithComponentSubItem>()
					                     	.Fetch(p => p.SubItem).ThenFetch(p => p.Details).ToList()
						).Should().NotThrow();
				}
			}
		}

		[Test, Ignore("Locked by re-linq")]
		public void LinqQueryWithFetch_WhenDerivedClassesUseComponentAndEagerFetchManyToOne_DoesNotGenerateInvalidSql()
		{
			using (ISession s = OpenSession())
			{
				using (var log = new SqlLogSpy())
				{
					Executing.This(() => s.Query<ItemWithComponentSubItem>().Fetch(p => p.SubItem.Details).ToList()).Should().NotThrow();
				}
			}
		}
	}
}