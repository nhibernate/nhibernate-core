using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3070
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override Cfg.MappingSchema.HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Employee>(ca =>
			{
				ca.Id(x => x.Id, map =>
				{
					map.Column("Id");
					map.Generator(Generators.Identity);
				});
				ca.Property(x => x.FirstName, map =>
				{
					map.Formula("(select 'something')");
					map.Lazy(true);
				});
			}); 

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void ProxyForEntityWithLazyPropertiesAndFormulaShouldEqualItself()
		{
			try
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				{
					var emp = new Employee();
					s.Save(emp);
					t.Commit();
				}

				using (var session = OpenSession())
				{
					var emps = session.QueryOver<Employee>().List();
					var emp = emps[0];

					// THIS ASSERT WILL FAIL 
					Assert.IsTrue(emp.Equals(emp), "Equals");
				}
			}
			finally
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				{
					s.Delete("from Employee");
					t.Commit();
				}
			}
		}
	}

	public class Employee
	{
		public virtual int Id { get; protected set; }
		public virtual string FirstName { get; set; }
	} 
}