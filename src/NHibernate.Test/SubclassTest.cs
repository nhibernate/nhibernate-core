using System;
using System.Collections;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test {
	
	[TestFixture]
	public class SubclassTest : TestCase {
	
		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);

		public SubclassTest() {}
					
		[SetUp]
		public void SetUp() {
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "Subclass.hbm.xml" } );
		}

		[Test]
		public void TestCRUD() 
		{
			// test the Save
			ISession s1 = sessions.OpenSession();
			ITransaction t1 = s1.BeginTransaction();

			JoinedSubclassOne one1 = new JoinedSubclassOne();
			one1.Id = 2;
			one1.TestDateTime = new System.DateTime(2003, 10, 17);
			one1.TestString = "the test one string";
			one1.TestLong = 6;
			one1.OneTestLong = 1;

			s1.Save(one1);

			JoinedSubclassBase base1 = new JoinedSubclassBase();
			base1.Id = 1;
			base1.TestDateTime = new System.DateTime(2003, 10, 17);
			base1.TestString = "the test string";
			base1.TestLong = 5;

			s1.Save(base1);

			t1.Commit();
			s1.Close();

			// lets verify the correct classes were saved
			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();
			
			// perform a load based on the base class
			JoinedSubclassBase base2 = (JoinedSubclassBase)s2.Load(typeof(JoinedSubclassBase), 1);
			JoinedSubclassBase oneBase2 = (JoinedSubclassBase)s2.Load(typeof(JoinedSubclassBase), 2);

			// do some quick checks to make sure s2 loaded an object with the same data as s2 saved.
			ObjectAssertion.AssertPropertiesEqual(base1, base2);
			
			// the object with id=2 was loaded using the base class - lets make sure it actually loaded
			// the sublcass
			JoinedSubclassOne one2 = oneBase2 as JoinedSubclassOne;
			Assertion.AssertNotNull(one2);

			// lets update the objects
			base2.TestString = "Did it get updated";

			// update the properties from the subclass and base class
			one2.TestString = "Updated JoinedSubclassOne String";
			one2.OneTestLong = 21; 
			
			// save it through the base class reference and make sure that the
			// subclass properties get updated.
			s2.Update(base2);
			s2.Update(oneBase2);
			
			t2.Commit();
			s2.Close();

			// lets test the Criteria interface for subclassing
			ISession s3 = sessions.OpenSession();
			ITransaction t3 = s3.BeginTransaction();

			IList results3 = s3.CreateCriteria(typeof(JoinedSubclassBase))
				.Add(Expression.Expression.In("TestString", new string[] {"Did it get updated", "Updated JoinedSubclassOne String" }))
				.List();

			Assertion.AssertEquals(2, results3.Count);

			JoinedSubclassBase base3 = null;
			JoinedSubclassOne one3 = null;

			foreach(JoinedSubclassBase obj in results3) 
			{
				if(obj is JoinedSubclassOne) 
					one3 = (JoinedSubclassOne)obj;
				else 
					base3 = obj;
			}

			// verify the properties got updated
			ObjectAssertion.AssertPropertiesEqual(base2, base3);
			ObjectAssertion.AssertPropertiesEqual(one2, one3);

			s3.Delete(base3);
			s3.Delete(one3);

			t3.Commit();
			s3.Close();

		}

	}
}
