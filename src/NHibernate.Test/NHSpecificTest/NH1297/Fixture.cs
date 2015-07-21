using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1297
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1297"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Model");
				tx.Commit();
			}
		}

		[Test]
		public void ItemsCanBeSavedAndUpdatedInTheSameSession()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Model m = new Model();
				m.Name = "model";
				m.Items.Add(new Item()); // New Item added here
				s.Save(m);
				s.Flush(); // Push changes to database; otherwise, bug does not manifest itself

				m.Items[0].Name = "new Name"; // Same new item updated here
				tx.Commit(); // InvalidCastException would be thrown here before the bug fix
			}
		}
	}
}