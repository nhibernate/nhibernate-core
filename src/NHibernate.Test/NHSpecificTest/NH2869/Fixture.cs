using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH2869
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
			base.Configure(configuration);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = this.OpenSession())
			{
				var entity = new DomainClass();
				entity.Id = 1;
				entity.Name = "Test";
				session.Save(entity);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = this.OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect as MsSql2008Dialect != null;
		}

		[Test]
		public void CustomExtensionWithConstantArgumentShouldBeIncludedInHqlProjection()
		{
			using (ISession session = this.OpenSession())
			{
				var projectionValue = (from c in session.Query<DomainClass>() where c.Name.IsOneInDbZeroInLocal("test") == 1 select c.Name.IsOneInDbZeroInLocal("test")).FirstOrDefault();
				//If the value is 0, the call was done in .NET, if it's 1 it has been projected correctly
				Assert.AreEqual(1, projectionValue);
			}
		}
	}
}