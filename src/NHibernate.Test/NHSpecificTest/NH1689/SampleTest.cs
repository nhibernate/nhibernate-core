namespace NHibernate.Test.NHSpecificTest.NH1689
{
	using System.Collections.Generic;
	using Dialect;
	using NUnit.Framework;

	[TestFixture]
	public class SampleTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = this.OpenSession())
			{
				DomainClass entity = new DomainClass();
				entity.Id = 1;
				session.Save(entity);
				session.Flush();
				session.Evict(entity);
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

		[Test]
		public void ShouldBeAbleToCallGenericMethod()
		{
			using (ISession session = this.OpenSession())
			{
				DomainClass entity = session.Load<DomainClass>(1);

				IList<string> inputStrings = entity.GetListOfTargetType<string>("arg");
				Assert.That(inputStrings.Count == 0);
			}
		}
	}

}
