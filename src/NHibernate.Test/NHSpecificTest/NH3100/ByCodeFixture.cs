using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3100
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual bool? Flag { get; set; }
	}

	[TestFixture]
	public class NullableBooleanFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Flag);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity { Flag = true });
				session.Save(new Entity { Flag = false });
				session.Save(new Entity { Flag = null });
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

		[Test]
		public void QueryWhereFlagIsTrue()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag == true).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void QueryWhereFlagIsFalse()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag == false).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void QueryWhereFlagIsNull()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag == null).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void QueryWhereFlagIsNotTrue()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag != true).ToList();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void QueryWhereFlagIsNotFalse()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag != false).ToList();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void QueryWhereFlagIsNotNull()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag != null).ToList();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void QueryWhereFlagIsEqual()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				// ReSharper disable once EqualExpressionComparison
				var result = session.Query<Entity>().Where(e => e.Flag == e.Flag).ToList();

				Assert.That(result, Has.Count.EqualTo(3));
			}
		}

		[Test]
		public void QueryWhereFlagIsNotEqual()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				// ReSharper disable once EqualExpressionComparison
				var result = session.Query<Entity>().Where(e => e.Flag != e.Flag).ToList();

				Assert.That(result, Is.Empty);
			}
		}

		[Test]
		public void GetValueOrDefault()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag.GetValueOrDefault()).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void GetValueOrDefaultFalse()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag.GetValueOrDefault(false)).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void GetValueOrDefaultTrue()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>().Where(e => e.Flag.GetValueOrDefault(true)).ToList();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}
	}
}
