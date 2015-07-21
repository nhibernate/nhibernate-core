using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2860
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		private int objId;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var obj = new ClassA();
				obj.Text = "Test existing object";
				obj.Blob_Field = new byte[] { 0, 0, 1, 1, 2, 2 };
				objId = (int) session.Save(obj);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from ClassA");
				transaction.Commit();
			}
		}

		[Test]
		public void Can_LazyPropertyCastingException()
		{
			//following causes exception "Unable to cast object of type 'System.Object' to type 'System.Byte[]'.
			//notice that ClassA.Blob_Field s declared as lazy property
			//similar to fixed NH-2510 (?)
			//PS. Exception is beeing thrown only if object was created within the same session

			using (var session = OpenSession())
			{
				var classA = new ClassA();
				classA.Text = "new entity";
				classA.Blob_Field = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				using (var trans = session.BeginTransaction())
				{
					session.Save(classA);
					trans.Commit();
				}

				session.Refresh(classA);

				classA.Text = "updated entity";
				using (var trans = session.BeginTransaction())
				{
					session.SaveOrUpdate(classA);
					trans.Commit();
				}

				session.Refresh(classA);
			}
		}

		[Test]
		public void Can_LazyPropertyNotCastingException()
		{
			//here none exception beeing thrown
			using (var session = OpenSession())
			{
				var classA = session.Get<ClassA>(objId);
				Assert.IsNotNull(classA);

				session.Refresh(classA);

				classA.Text = "updated entity";
				using (var trans = session.BeginTransaction())
				{
					session.SaveOrUpdate(classA);
					trans.Commit();
				}

				session.Refresh(classA);
			}
		}
	}
}
