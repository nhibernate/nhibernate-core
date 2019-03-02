using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2029
{
	public class TestClass
	{
		public virtual int Id { get; set; }
		public virtual int? NullableInt32Prop { get; set; }
		public virtual int Int32Prop { get; set; }
		public virtual long? NullableInt64Prop { get; set; }
		public virtual long Int64Prop { get; set; }
	}

	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<TestClass>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Native));
				rc.Property(x => x.NullableInt32Prop);
				rc.Property(x => x.Int32Prop);
				rc.Property(x => x.NullableInt64Prop);
				rc.Property(x => x.Int64Prop);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new TestClass
				{
					Int32Prop = int.MaxValue,
					NullableInt32Prop = int.MaxValue,
					Int64Prop = int.MaxValue,
					NullableInt64Prop = int.MaxValue
				});
				session.Save(new TestClass
				{
					Int32Prop = int.MaxValue,
					NullableInt32Prop = int.MaxValue,
					Int64Prop = int.MaxValue,
					NullableInt64Prop = int.MaxValue
				});
				session.Save(new TestClass
				{
					Int32Prop = int.MaxValue,
					NullableInt32Prop = null,
					Int64Prop = int.MaxValue,
					NullableInt64Prop = null
				});

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from TestClass").ExecuteUpdate();

				tx.Commit();
			}
		}

		[Test]
		public void NullableIntOverflow()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			using (var sqlLog = new SqlLogSpy())
			{
				var groups = session.Query<TestClass>()
					   .GroupBy(i => 1)
					   .Select(g => new
					   {
						   s = g.Sum(i => (long) i.NullableInt32Prop)
					   })
					   .ToArray();

				Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "cast"), Is.EqualTo(1));
				Assert.That(groups, Has.Length.EqualTo(1));
				Assert.That(groups[0].s, Is.EqualTo((long) int.MaxValue * 2));
			}
		}

		[Test]
		public void IntOverflow()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			using (var sqlLog = new SqlLogSpy())
			{
				var groups = session.Query<TestClass>()
									.GroupBy(i => 1)
									.Select(g => new
									{
										s = g.Sum(i => (long) i.Int32Prop)
									})
									.ToArray();

				Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "cast"), Is.EqualTo(1));
				Assert.That(groups, Has.Length.EqualTo(1));
				Assert.That(groups[0].s, Is.EqualTo((long) int.MaxValue * 3));
			}
		}

		[Test]
		public void NullableInt64NoCast()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			using (var sqlLog = new SqlLogSpy())
			{
				var groups = session.Query<TestClass>()
				                    .GroupBy(i => 1)
				                    .Select(g => new {
					                    s = g.Sum(i => i.NullableInt64Prop)
				                    })
				                    .ToArray();

				Assert.That(sqlLog.GetWholeLog(), Does.Not.Contains("cast"));
				Assert.That(groups, Has.Length.EqualTo(1));
				Assert.That(groups[0].s, Is.EqualTo((long) int.MaxValue * 2));
			}
		}

		[Test]
		public void Int64NoCast()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			using (var sqlLog = new SqlLogSpy())
			{
				var groups = session.Query<TestClass>()
				                    .GroupBy(i => 1)
				                    .Select(g => new {
					                    s = g.Sum(i => i.Int64Prop)
				                    })
				                    .ToArray();

				Assert.That(sqlLog.GetWholeLog(), Does.Not.Contains("cast"));
				Assert.That(groups, Has.Length.EqualTo(1));
				Assert.That(groups[0].s, Is.EqualTo((long) int.MaxValue * 3));
			}
		}

		private int FindAllOccurrences(string source, string substring)
		{
			if (source == null)
			{
				return 0;
			}
			int n = 0, count = 0;
			while ((n = source.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1)
			{
				n += substring.Length;
				++count;
			}
			return count;
		}
	}
}
