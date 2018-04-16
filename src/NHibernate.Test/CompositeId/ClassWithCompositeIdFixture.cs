using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.CompositeId
{
	/// <summary>
	/// Summary description for ClassWithCompositeIdFixture.
	/// </summary>
	[TestFixture]
	public class ClassWithCompositeIdFixture : TestCase
	{
		private DateTime firstDateTime = new DateTime(2003, 8, 16);
		private DateTime secondDateTime = new DateTime(2003, 8, 17);
		private Id id;
		private Id secondId;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"CompositeId.ClassWithCompositeId.hbm.xml"}; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Dialect.FirebirdDialect); // Firebird has no CommandTimeout, and locks up during the tear-down of this fixture
		}

		protected override void OnSetUp()
		{
			id = new Id("stringKey", 3, firstDateTime);
			secondId = new Id("stringKey2", 5, secondDateTime);
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from ClassWithCompositeId");
				s.Flush();
			}
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
			ClassWithCompositeId theClass;
			ClassWithCompositeId theSecondClass;

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
			    theClass = new ClassWithCompositeId(id);
			    theClass.OneProperty = 5;

			    theSecondClass = new ClassWithCompositeId(secondId);
			    theSecondClass.OneProperty = 10;

			    s.Save(theClass);
			    s.Save(theSecondClass);

			    t.Commit();
			}

			// verify they were inserted and test the SELECT
			ClassWithCompositeId theClass2;
			ClassWithCompositeId theSecondClass2;
			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				theClass2 = (ClassWithCompositeId) s2.Load(typeof(ClassWithCompositeId), id);
				Assert.AreEqual(id, theClass2.Id);

				IList results2 = s2.CreateCriteria(typeof(ClassWithCompositeId))
				                   .Add(Expression.Eq("Id", secondId))
				                   .List();

				Assert.AreEqual(1, results2.Count);
				theSecondClass2 = (ClassWithCompositeId) results2[0];

				ClassWithCompositeId theClass2Copy = (ClassWithCompositeId) s2.Load(typeof(ClassWithCompositeId), id);

				// verify the same results through Criteria & Load were achieved
				Assert.AreSame(theClass2, theClass2Copy);

				// compare them to the objects created in the first session
				Assert.AreEqual(theClass.Id, theClass2.Id);
				Assert.AreEqual(theClass.OneProperty, theClass2.OneProperty);

				Assert.AreEqual(theSecondClass.Id, theSecondClass2.Id);
				Assert.AreEqual(theSecondClass.OneProperty, theSecondClass2.OneProperty);

				// test the update functionallity
				theClass2.OneProperty = 6;
				theSecondClass2.OneProperty = 11;

				s2.Update(theClass2);
				s2.Update(theSecondClass2);

				t2.Commit();
			}

			// lets verify the update went through
			using (ISession s3 = OpenSession())
			using (ITransaction t3 = s3.BeginTransaction())
			{
				ClassWithCompositeId theClass3 = (ClassWithCompositeId) s3.Load(typeof(ClassWithCompositeId), id);
				ClassWithCompositeId theSecondClass3 = (ClassWithCompositeId) s3.Load(typeof(ClassWithCompositeId), secondId);

				// check the update properties
				Assert.AreEqual(theClass3.OneProperty, theClass2.OneProperty);
				Assert.AreEqual(theSecondClass3.OneProperty, theSecondClass2.OneProperty);

				// test the delete method
				s3.Delete(theClass3);
				s3.Delete(theSecondClass3);

				t3.Commit();
			}

			// lets verify the delete went through
			using (ISession s4 = OpenSession())
			{
				try
				{
					ClassWithCompositeId theClass4 = (ClassWithCompositeId) s4.Load(typeof(ClassWithCompositeId), id);
				}
				catch (ObjectNotFoundException)
				{
					// I expect this to be thrown because the object no longer exists...
				}

				IList results = s4.CreateCriteria(typeof(ClassWithCompositeId))
				                  .Add(Expression.Eq("Id", secondId))
				                  .List();

				Assert.AreEqual(0, results.Count);
			}
		}

		[Test]
		public void Criteria()
		{
			Id id = new Id("stringKey", 3, firstDateTime);
			ClassWithCompositeId cId = new ClassWithCompositeId(id);
			cId.OneProperty = 5;

			// add the new instance to the session so I have something to get results 
			// back for
			using (ISession s = OpenSession())
			{
				s.Save(cId);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				ICriteria c = s.CreateCriteria(typeof(ClassWithCompositeId));
				c.Add(Expression.Eq("Id", id));

				// right now just want to see if the Criteria is valid
				IList results = c.List();

				Assert.AreEqual(1, results.Count);
			}
		}

		[Test]
		public void Hql()
		{
			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				ClassWithCompositeId theClass = new ClassWithCompositeId(id);
				theClass.OneProperty = 5;

				ClassWithCompositeId theSecondClass = new ClassWithCompositeId(secondId);
				theSecondClass.OneProperty = 10;

				s.Save(theClass);
				s.Save(theSecondClass);

				t.Commit();
			}

			using (ISession s2 = OpenSession())
			{
				IQuery hql = s2.CreateQuery("from ClassWithCompositeId as cwid where cwid.Id.KeyString = :keyString");

				hql.SetString("keyString", id.KeyString);

				IList results = hql.List();

				Assert.AreEqual(1, results.Count);
			}
		}
	}
}
