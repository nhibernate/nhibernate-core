using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3124
{
	public class ByCodeFixture : TestCaseMappingByCode
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
				session.Save(new Person {Id = 1000, Name = "Test", Type = 'A'});
				transaction.Commit();
			}
		}

		[Test]
		public void LinqStatementGeneratesIncorrectCastToInteger()
		{
			using (ISession session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Query<Person>().Where(x => x.Type == 'A').ToList();
				transaction.Commit();
			}
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
	}

	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual char Type { get; set; }
	}
}