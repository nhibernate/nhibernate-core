using System;
using System.Collections;
using System.Data;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for ClassWithCompositeIdTest.
	/// </summary>
	[TestFixture]
	public class ClassWithCompositeIdTest : TestCase
	{
		public ClassWithCompositeIdTest(){}

		private DateTime firstDateTime = new DateTime(2003, 8, 16);
		private DateTime secondDateTime = new DateTime(2003, 8, 17);

					
		[SetUp]
		public void SetUp() 
		{
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "ClassWithCompositeId.hbm.xml" } );
		}

		/// <summary>
		/// Test the basic CRUD operations for a class with a Composite Identifier
		/// </summary>
		/// <remarks>
		/// The following items are tested in this Test Script
		/// <list type="">
		///		<item>
		///			<term>Save</term>
		///		</item>
		///		<item>
		///			<term>Load</term>
		///		</item>
		///		<item>
		///			<term>Criteria</term>
		///		</item>
		///		<item>
		///			<term>Update</term>
		///		</item>
		///		<item>
		///			<term>Delete</term>
		///		</item>
		///		<item>
		///			<term>Criteria - No Results</term>
		///		</item>
		/// </list>
		/// </remarks>
		[Test]
		public void TestSimpleCRUD() 
		{
			CompositeId id = new CompositeId("stringKey", 3, firstDateTime);
			CompositeId secondId = new CompositeId("stringKey2", 5, secondDateTime);
			
			// insert the new objects
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			ClassWithCompositeId theClass = new ClassWithCompositeId();
			theClass.Id = id;
			theClass.OneProperty = 5;

			ClassWithCompositeId theSecondClass = new ClassWithCompositeId();
			theSecondClass.Id = secondId;
			theSecondClass.OneProperty = 10;

			s.Save(theClass);
			s.Save(theSecondClass);

			t.Commit();
			s.Close();

			// verify they were inserted and test the SELECT

			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			ClassWithCompositeId theClass2 = (ClassWithCompositeId)s2.Load(typeof(ClassWithCompositeId), id);
			
			IList results2 = s2.CreateCriteria(typeof(ClassWithCompositeId))
					.Add(Expression.Expression.Eq("Id", secondId))
					.List();

			Assertion.AssertEquals(1, results2.Count);
			ClassWithCompositeId theSecondClass2 = (ClassWithCompositeId)results2[0];

			ClassWithCompositeId theClass2Copy = (ClassWithCompositeId)s2.Load(typeof(ClassWithCompositeId), id);
			
			// verify the same results through Criteria & Load were achieved
			Assertion.AssertSame(theClass2, theClass2Copy);

			// compare them to the objects created in the first session
			Assertion.AssertEquals(theClass.Id, theClass2.Id);
			Assertion.AssertEquals(theClass.OneProperty, theClass2.OneProperty);

			Assertion.AssertEquals(theSecondClass.Id, theSecondClass2.Id);
			Assertion.AssertEquals(theSecondClass.OneProperty, theSecondClass2.OneProperty);

			// test the update functionallity
			theClass2.OneProperty = 6;
			theSecondClass2.OneProperty = 11;

			s2.Update(theClass2);
			s2.Update(theSecondClass2);

			t2.Commit();
			s2.Close();

			// lets verify the update went through
			ISession s3 = sessions.OpenSession();
			ITransaction t3 = s3.BeginTransaction();

			ClassWithCompositeId theClass3 = (ClassWithCompositeId)s3.Load(typeof(ClassWithCompositeId), id);
			ClassWithCompositeId theSecondClass3 = (ClassWithCompositeId)s3.Load(typeof(ClassWithCompositeId), secondId);

			// check the update properties
			Assertion.AssertEquals(theClass3.OneProperty, theClass2.OneProperty);
			Assertion.AssertEquals(theSecondClass3.OneProperty, theSecondClass2.OneProperty);
			
			// test the delete method
			s3.Delete(theClass3);
			s3.Delete(theSecondClass3);

			t3.Commit();
			s3.Close();

			// lets verify the delete went through
			ISession s4 = sessions.OpenSession();

			try 
			{
				ClassWithCompositeId theClass4 = (ClassWithCompositeId)s4.Load(typeof(ClassWithCompositeId), id);
			}
			catch(ObjectNotFoundException onfe) 
			{
				// I expect this to be thrown because the object no longer exists...
			}

			IList results =  s4.CreateCriteria(typeof(ClassWithCompositeId))
				.Add(Expression.Expression.Eq("Id", secondId))
				.List();

			Assertion.AssertEquals(0, results.Count);

		}

	}
}
