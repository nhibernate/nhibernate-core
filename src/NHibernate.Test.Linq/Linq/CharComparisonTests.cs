using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class CharEqualityTests : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(ca =>
			{
				ca.Id(x => x.Id, map => map.Generator(Generators.Assigned));
				ca.Property(x => x.Name, map => map.Length(150));
				ca.Property(x => x.Type, map => map.Length(1));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Person { Id = 1000, Name = "Person Type A", Type = 'A' });
				session.Save(new Person { Id = 1001, Name = "Person Type B", Type = 'B' });
				session.Save(new Person { Id = 1002, Name = "Person Type C", Type = 'C' });
				transaction.Commit();
			}
		}

		[Test]
		public void CharPropertyEqualToCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type == 'C'));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharLiteralEqualToCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'C' == x.Type));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharPropertyEqualToCharVariable()
		{
			char value = 'C';
			var results = Execute(session => session.Query<Person>().Where(x => x.Type == value));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharVariableEqualToCharProperty()
		{
			char value = 'C';
			var results = Execute(session => session.Query<Person>().Where(x => value == x.Type));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharPropertyNotEqualToCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type != 'C'));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		[Test]
		public void CharLiteralNotEqualToCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'C' != x.Type));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		[Test]
		public void CharPropertyNotEqualToCharVariable()
		{
			char value = 'C';
			var results = Execute(session => session.Query<Person>().Where(x => x.Type != value));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		[Test]
		public void CharVariableNotEqualToCharProperty()
		{
			char value = 'C';
			var results = Execute(session => session.Query<Person>().Where(x => value != x.Type));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		[Test]
		public void CharPropertyGreaterThanCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type > 'B'));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharLiteralLessThanCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'B' < x.Type));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type C"));
		}

		[Test]
		public void CharPropertyGreaterThanOrEqualToCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type >= 'B'));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type B", "Person Type C" }));
		}

		[Test]
		public void CharLiteralLessThanOrEqualToCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'B' <= x.Type));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type B", "Person Type C" }));
		}

		[Test]
		public void CharPropertyLessThanCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type < 'B'));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type A"));
		}

		[Test]
		public void CharLiteralGreaterThanCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'B' > x.Type));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Name, Is.EqualTo("Person Type A"));
		}

		[Test]
		public void CharPropertyLessThanOrEqualToCharLiteral()
		{
			var results = Execute(session => session.Query<Person>().Where(x => x.Type <= 'B'));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		[Test]
		public void CharLiteralGreaterThanOrEqualToCharProperty()
		{
			var results = Execute(session => session.Query<Person>().Where(x => 'B' >= x.Type));
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results.Select(p => p.Name), Is.EquivalentTo(new[] { "Person Type A", "Person Type B" }));
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from Person");
				transaction.Commit();
			}
		}

		private IList<Person> Execute(Func<IStatelessSession, IQueryable<Person>> query)
		{
			using (var session = Sfi.OpenStatelessSession())
			using (session.BeginTransaction())
			{
				return query(session).ToList();
			}
		}
	}

	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual char Type { get; set; }
	}
}