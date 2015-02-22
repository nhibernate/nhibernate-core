using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.MappingByCode.Fk.Bidirectional
{
	[TestFixture]
	public class DeleteOneToOneOrphansTest : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Employee>(mc =>
			{
				mc.Id(x => x.Id, map =>
				{
					map.Generator(Generators.Identity);
					map.Column("Id");
				});
				mc.OneToOne<EmployeeInfo>(x => x.Info, map =>
				{
					map.PropertyReference(x => x.EmployeeDetails);
					map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					map.Constrained(false);
				});
			});

			mapper.Class<EmployeeInfo>(mc =>
			{
				mc.Id(x => x.Id, map =>
				{
					map.Generator(Generators.Identity);
					map.Column("Id");
				});
				mc.ManyToOne<Employee>(x => x.EmployeeDetails, map =>
				{
					map.Column("employee_id");
					map.Unique(true);
					map.NotNullable(true);
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee();
				emp.Info = new EmployeeInfo(emp);

				s.Save(emp);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from EmployeeInfo");
				session.Delete("from Employee");
				tx.Commit();
			}
		}

		[Test]
		public void TestOrphanedWhileManaged()
		{
			long empId = 0;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var empInfoResults = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoResults.Count);

				var empResults = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empResults.Count);

				var emp = empResults[0];
				Assert.NotNull(emp);

				empId = emp.Id;
				emp.Info = null;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.Null(emp.Info);

				var empInfoResults = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(0, empInfoResults.Count);

				var empResults = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empResults.Count);

				t.Commit();
			}
		}
	}
}
