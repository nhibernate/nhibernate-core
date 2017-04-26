using System.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.Fk.Reversed.Bidirectional
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
				emp.Info = new EmployeeInfo(emp) { JobTitle = "Consul" };

				s.Save(emp);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from EmployeeInfo");
				s.Delete("from Employee");
				tx.Commit();
			}
		}

		[Test]
		public void TestOrphanedWhileManaged()
		{
			long empId = 0;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				Employee emp = empList[0];
				Assert.NotNull(emp.Info );

				empId = emp.Id;
				emp.Info = null;

				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.IsNull(emp.Info);
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual( 0, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				tx.Commit();
			}
		}
	}

	[TestFixture]
	public class DeleteOneToOneOrphansTestHbm : DeleteOneToOneOrphansTest
	{
		protected override IList Mappings
		{
			get { return new[] {"Cascade.OneToOneCascadeDelete.Fk.Reversed.Bidirectional.Mappings.hbm.xml"}; }
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
				mc.ManyToOne<EmployeeInfo>(x => x.Info, map =>
				{
					map.Column("Info_id");
					map.Unique(true);
					map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
				});
				mc.Property(x => x.Name);
			});

			mapper.Class<EmployeeInfo>(cm =>
			{
				cm.Id(x => x.Id, m =>
				{
					m.Generator(Generators.Increment);
					m.Column("Id");
				});
				cm.OneToOne<Employee>(x => x.EmployeeDetails, map =>
				{
					map.PropertyReference(x => x.Info);
				});
				cm.Property(x => x.JobTitle);
			});
			configuration.AddDeserializedMapping(mapper.CompileMappingForAllExplicitlyAddedEntities(), "TestDomain");
		}
	}
}
