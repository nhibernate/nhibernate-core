using System.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.Fk.Bidirectional
{
	public abstract class DeleteOneToOneOrphansTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee { Name = "Julius Caesar" };
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
			long empId;

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

	[TestFixture]
	public class DeleteOneToOneOrphansTestHbm : DeleteOneToOneOrphansTest
	{
		protected override IList Mappings
		{
			get { return new[] { "Cascade.OneToOneCascadeDelete.Fk.Bidirectional.Mappings.hbm.xml" }; }
		}
	}

	[TestFixture]
	public class DeleteOneToOneOrphansTestByCode : DeleteOneToOneOrphansTest
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		protected override void AddMappings(Configuration configuration)
		{
			var mapper = new ModelMapper();

			mapper.Class<Employee>(mc =>
			{
				mc.Id(x => x.Id, map =>
				{
					map.Generator(Generators.Increment);
					map.Column("Id");
				});
				mc.OneToOne<EmployeeInfo>(x => x.Info, map =>
				{
					map.PropertyReference(x => x.EmployeeDetails);
					map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					map.Constrained(false);
				});
				mc.Property(x => x.Name);
			});

			mapper.Class<EmployeeInfo>(mc =>
			{
				mc.Id(x => x.Id, map =>
				{
					map.Generator(Generators.Increment);
					map.Column("Id");
				});
				mc.ManyToOne<Employee>(x => x.EmployeeDetails, map =>
				{
					map.Column("employee_id");
					map.Unique(true);
					map.NotNullable(true);
				});
			});

			configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
		}
	}
}
