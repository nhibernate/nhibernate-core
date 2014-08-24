using System.Data;
using log4net;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		// The problem is the same using a default sort="natural" collection for Children
		// and there is no problem using a default HashSet.
		// look a the implementation of Children

		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int parentId;
		int doctorId;
		
		protected override ISession OpenSession()
		{
			var session = base.OpenSession();
			session.FlushMode = FlushMode.Commit;

			return session;
		}

		protected override void OnSetUp()
		{
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				var parent = new Parent {Address = "A street, A town, A country"};

				// If you add a child all work fine.
				//var child = new Child {Age = 2, Parent = parent};
				//parent.Children.AddChild(child);
				
				sess.Save(parent);

				var doctor = new Doctor {DoctorNumber = 123, MedicalRecord = parent.MedicalRecord};

				sess.Save(doctor);
				tx.Commit();

				parentId = parent.Id;
				doctorId = doctor.Id;
			}
		}

		[Test]
		public void Test()
		{
			Log.Debug("Entering test");

			using (ISession sess = OpenSession())
			{
				Log.Debug("Loading doctor");
				var doctor = sess.Get<Doctor>(doctorId);		// creates a proxy of the medical record
				
				Log.Debug("Loading parent");
				var parent = sess.Get<Parent>(parentId);
				
				Log.Debug("Adding new child to parent");
				parent.Children.AddChild(new Child { Age = 10, Parent = parent });		// does NOT cause Child.GetHashCode() to be called

				using (ITransaction tx = sess.BeginTransaction(IsolationLevel.ReadCommitted))
				{
					Log.Debug("Saving parent");
					sess.Update(parent);

					Log.Debug("Committing transaction");
					tx.Commit();								// triggers Child.GetHashCode() to be called in flush machiney, leading to CNPBF exception
				}
			}			

			Log.Debug("Exiting test");
		}

		protected override void OnTearDown()
		{
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Delete("from Doctor");
				sess.Delete("from Parent");
				sess.Delete("from Child");
				sess.Delete("from MedicalRecord");
				sess.Delete("from Disease");
				tx.Commit();
			}
		}
	}
}
