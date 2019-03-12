using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3033
{
	[TestFixture]
	public class QueryTest : TestCaseMappingByCode
	{
		private Guid _companyId;

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from Company");
				transaction.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using(var session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var company = new Company();

				var ayende = new Employee("Ayende");
				ayende.WorksIn(company);

				var fabian = new Employee("Fabian");
				fabian.WorksIn(company);

				var daniel = new ExEmployee("Daniel");
				daniel.HasWorkedIn(company);

				var peter = new ExEmployee("Peter");
				peter.HasWorkedIn(company);

				session.Save(company);
				_companyId = company.Id;

				session.Save(ayende);
				session.Save(fabian);
				session.Save(daniel);
				session.Save(peter);

				transaction.Commit();
			}
		}

		[Test]
		public void WorksWithFutures()
		{
			using(new SqlLogSpy())
			using (var session = OpenSession())
			{
				PersonDTO personDTO = null;

				var employeeCount = session.QueryOver<Employee>()
														.Where(t => t.Company.Id == _companyId)
														.Select(Projections.RowCount())
														.FutureValue<int>();

				var exEmployeeCount = session.QueryOver<ExEmployee>()
														.Where(t => t.Company.Id == _companyId)
														.Select(Projections.RowCount())
														.FutureValue<int>();

				PersonDTO result = session.QueryOver<Company>()
										.Where(o => o.Id == _companyId)
										.Select(
											Projections.Constant(employeeCount.Value)
														.WithAlias(() => personDTO.NumberOfEmployees),
											Projections.Constant(exEmployeeCount.Value)
														.WithAlias(() => personDTO.NumberOfExEmployees))
										.TransformUsing(Transformers.AliasToBean<PersonDTO>())
										.SingleOrDefault<PersonDTO>();

				Assert.AreEqual(2, result?.NumberOfEmployees);
				Assert.AreEqual(2, result?.NumberOfExEmployees);
			}
		}

		[Test]
		public void WorksWithSubQueries()
		{
			using(new SqlLogSpy())
			using (var session = OpenSession())
			{
				PersonDTO personDTO = null;

				var employeeQuery = QueryOver.Of<Employee>()
											.Where(t => t.Company.Id == _companyId)
											.Select(Projections.RowCount());

				var exEmployeeQuery = QueryOver.Of<ExEmployee>()
												.Where(t => t.Company.Id == _companyId)
												.Select(Projections.RowCount());

				PersonDTO result = session.QueryOver<Company>()
										.Where(o => o.Id == _companyId)
										.Select(
											Projections.SubQuery(employeeQuery)
														.WithAlias(() => personDTO.NumberOfEmployees),
											Projections.SubQuery(exEmployeeQuery)
														.WithAlias(() => personDTO.NumberOfExEmployees))
										.TransformUsing(Transformers.AliasToBean<PersonDTO>())
										.SingleOrDefault<PersonDTO>();

				Assert.AreEqual(2, result?.NumberOfEmployees);
				Assert.AreEqual(2, result?.NumberOfExEmployees);
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Company>(rc => { rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb)); });
			mapper.Class<Person>(
				rc => { rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb)); });
			mapper.JoinedSubclass<Employee>(rc => rc.ManyToOne(x => x.Company, m => m.NotNullable(true)));
			mapper.JoinedSubclass<ExEmployee>(rc => rc.ManyToOne(x => x.Company, m => m.NotNullable(true)));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}

	public class PersonDTO
	{
		public int NumberOfEmployees { get; set; }

		public int NumberOfExEmployees { get; set; }
	}
}
