using System;
using System.Collections;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for UnsavedValueTest.
	/// </summary>
	[TestFixture]
	public class UnsavedValueFixture : TestCase
	{
		public static int newId = 0;

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.UnsavedType.hbm.xml"});
		}

		[Test]
		public void TestCRUD() 
		{
			// make a new object outside of the Session
			UnsavedType unsavedToSave = new UnsavedType();
			unsavedToSave.TypeName = "Simple UnsavedValue";

			// open the first session to SaveOrUpdate it - should be Save
			ISession s1 = OpenSession();
			ITransaction t1 = s1.BeginTransaction();
			s1.SaveOrUpdate(unsavedToSave);
			t1.Commit();
			s1.Close();

			// simple should have been inserted - generating a new key for it
			Assert.IsTrue(unsavedToSave.Id != 0, "Id should not be zero");

			// use the ICriteria interface to get another instance in a different
			// session
			ISession s2 = OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			IList results2 = s2.CreateCriteria(typeof(UnsavedType))
				.Add(Expression.Expression.Eq("Id", unsavedToSave.Id))
				.List();
			
			Assert.AreEqual(1, results2.Count, "Should have found a match for the new Id");
			
			UnsavedType unsavedToUpdate = (UnsavedType)results2[0];

			// make sure it has the same Id
			Assert.AreEqual(unsavedToSave.Id, unsavedToUpdate.Id, "Should have the same Id");

			t2.Commit();
			s2.Close();

			// passing it to the UI for modification
			unsavedToUpdate.TypeName = "ui changed it";

			// create a new session for the Update
			ISession s3 = OpenSession();
			ITransaction t3 = s3.BeginTransaction();

			s3.SaveOrUpdate(unsavedToUpdate);

			t3.Commit();
			s3.Close();

			// make sure it has the same Id - if the Id has changed then that means it
			// was inserted.
			Assert.AreEqual(unsavedToSave.Id, unsavedToUpdate.Id, "Should have the same Id");

			// lets get a list of all the rows in the table to make sure 
			// that there has not been any extra inserts
			ISession s4 = OpenSession();
			ITransaction t4 = s4.BeginTransaction();

			IList results4 = s4.CreateCriteria(typeof(UnsavedType)).List();
			Assert.AreEqual(1, results4.Count, "Should only be one item");

			// lets make sure the object was updated
			UnsavedType unsavedToDelete = (UnsavedType)results4[0];
			Assert.AreEqual(unsavedToUpdate.TypeName, unsavedToDelete.TypeName);

			s4.Delete(unsavedToDelete);
 
			t4.Commit();
			s4.Close();

			// lets make sure the object was deleted

			ISession s5 = OpenSession();
			try 
			{
				UnsavedType unsavedNull = (UnsavedType)s5.Load(typeof(UnsavedType), unsavedToDelete.Id);
				Assert.IsNull(unsavedNull);
			}
			catch(ObjectNotFoundException onfe) 
			{
				// do nothing it was expected
				Assert.IsNotNull(onfe); //getting ride of 'onfe' is never used compile warning
			}

			s5.Close();

		}

	}
}
