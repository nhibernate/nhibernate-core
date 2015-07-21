using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1818
{
	[TestFixture]
	public class Fixture1818 : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				session.Save(new DomainClass { Id = 1 });
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as PostgreSQL82Dialect != null;
		}


		[Test]
		[Description("Test HQL query on a property mapped with a formula.")]
		public void ComputedPropertyShouldRetrieveDataCorrectly()
		{
			using (var session = OpenSession())
			{
				var obj = session.CreateQuery("from DomainClass dc where dc.AlwaysTrue").UniqueResult<DomainClass>();
				Assert.IsNotNull(obj);
			}
		}
	}
}