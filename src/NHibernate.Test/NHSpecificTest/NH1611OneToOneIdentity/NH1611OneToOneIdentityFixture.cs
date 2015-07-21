using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1611OneToOneIdentity
{
	[TestFixture]
	public class NH1611OneToOneIdentityFixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1611OneToOneIdentity"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using(ISession session = OpenSession())
			{
				using(ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Adjunct");
					session.Delete("from Primary");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					Primary primary = new Primary();
					primary.ID = 5;
					primary.Description = "blarg";
					Adjunct adjunct = new Adjunct();
					adjunct.ID = 5;
					adjunct.AdjunctDescription = "nuts";
					primary.Adjunct = adjunct;
					s.Save(primary);
					s.Save(adjunct);
					tx.Commit();
				}
			}
		}


		[Test]
		public void CanQueryOneToOneWithCompositeId()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					ICriteria criteria = s.CreateCriteria(typeof (Primary));
					IList<Primary> list = criteria.List<Primary>();
					Assert.AreEqual("blarg", list[0].Description);
					Assert.AreEqual("nuts", list[0].Adjunct.AdjunctDescription);

				}
			}
		}
	
	}
}