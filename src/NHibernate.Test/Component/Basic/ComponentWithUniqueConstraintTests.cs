using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Component.Basic
{
	public class ComponentWithUniqueConstraintTests : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Component<Person>(comp =>
			{
				comp.Property(p => p.Name);
				comp.Property(p => p.Dob);
				comp.Unique(true); // hbm2ddl: Generate a unique constraint in the database
			});

			mapper.Class<Employee>(cm =>
			{
				cm.Id(employee => employee.Id, map => map.Generator(Generators.HighLow));
				cm.Property(employee => employee.HireDate);
				cm.Component(person => person.Person);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Employee");
				transaction.Commit();
			}
		}

		[Test]
		public void CanBePersistedWithUniqueValues()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Employee { HireDate = DateTime.Today, Person = new Person { Name = "Bill", Dob = new DateTime(2000, 1, 1) } };
				var e2 = new Employee { HireDate = DateTime.Today, Person = new Person { Name = "Hillary", Dob = new DateTime(2000, 1, 1) } };
				session.Save(e1);
				session.Save(e2);
				transaction.Commit();
			}

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var employees = session.Query<Employee>().ToList();
				Assert.That(employees.Count, Is.EqualTo(2));
				Assert.That(employees.Select(employee => employee.Person.Name).ToArray(), Is.EquivalentTo(new[] { "Hillary", "Bill" }));
			}
		}

		[Test]
		public void CannotBePersistedWithNonUniqueValues()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var e1 = new Employee { HireDate = DateTime.Today, Person = new Person { Name = "Bill", Dob = new DateTime(2000, 1, 1) } };
				var e2 = new Employee { HireDate = DateTime.Today, Person = new Person { Name = "Bill", Dob = new DateTime(2000, 1, 1) } };

				var exception = Assert.Throws<GenericADOException>(() =>
					{
						session.Save(e1);
						session.Save(e2);
						session.Flush();
					});
				Assert.That(exception.InnerException, Is.AssignableTo<DbException>());
				Assert.That(exception.InnerException.Message,
					Does.Contain("unique").IgnoreCase.And.Contains("constraint").IgnoreCase
					.Or.Contains("duplicate entry").IgnoreCase);
			}
		}
	}
}
