using System.Collections;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.Pk.Unidirectional
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
				s.Save(emp);
				var info = new EmployeeInfo(emp.Id);
				emp.Info = info;

				s.Save(info);
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
			long empId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				Employee emp = empList[0];
				Assert.NotNull(emp.Info);

				var empAndInfoList = s.CreateQuery("from Employee e, EmployeeInfo i where e.Info = i").List();
				Assert.AreEqual(1, empAndInfoList.Count);

				var result = (object[])empAndInfoList[0];

				emp = result[0] as Employee;

				Assert.NotNull(result[1]);
				Assert.AreSame(emp.Info, result[1]);

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
				Assert.AreEqual(0, empInfoList.Count);

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
			get { return new[] { "Cascade.OneToOneCascadeDelete.Pk.Unidirectional.Mappings.hbm.xml" }; }
		}
	}

	[TestFixture]
	public class DeleteOneToOneOrphansTestByCode : DeleteOneToOneOrphansTest
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}
		
		protected override void AddMappings(Cfg.Configuration configuration)
		{
			var mapper = new ModelMapper();

			mapper.Class<Employee>(mc =>
			{
				mc.Id(x => x.Id, m =>
				{
					m.Generator(Generators.Increment);
					m.Column("Id");
				});
				mc.OneToOne<EmployeeInfo>(x => x.Info, map =>
				{
					map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					map.Constrained(false);
				});
				mc.Property(x => x.Name);
			});

			mapper.Class<EmployeeInfo>(mc =>
			{
				mc.Id(x => x.Id, map =>
				{
					map.Generator(Generators.Assigned);
					map.Column("Id");
				});
			});
			configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
		}
	}
}
