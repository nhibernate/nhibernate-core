using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.MappingByCode.Fk.Composite
{
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
				mc.ManyToOne<EmployeeInfo>(x => x.Info, map =>
				{
					// Columns have to be declared first otherwise other properties are reset.
					map.Columns(x => { x.Name("COMP_ID"); },
								x => { x.Name("PERS_ID"); });
					map.Unique(true);
					map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					map.NotFound(NotFoundMode.Exception);
				});
			});

			mapper.Class<EmployeeInfo>(mc =>
			{
				mc.ComponentAsId<EmployeeInfo.Identifier>(x => x.Id, map =>
				{
					map.Property(x => x.CompanyId, m => m.Column("COMPS_ID"));
					map.Property(x => x.PersonId, m => m.Column("PERS_ID"));
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
				emp.Info = new EmployeeInfo( 1L, 1L);

				s.Save(emp.Info);
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
				var infoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, infoList.Count );

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				var emp = empList[0];
				Assert.NotNull(emp.Info);

				empId = emp.Id;
				emp.Info = null;

				s.Update(emp);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.IsNull(emp.Info);

				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(0, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);
			}
		}
	}
}
