using System;
using System.Collections;
using System.Linq;
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

		protected override string[] Mappings
		{
			get { return new string[] {"CompositeId.ClassWithCompositeId.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			id = new Id("stringKey", 3, firstDateTime);
			secondId = new Id("stringKey2", 5, secondDateTime);
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from ClassWithCompositeId");
				s.Flush();
				t.Commit();
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

		[Test]
		public void HqlInClause()
		{
			var id1 = id;
			var id2 = secondId;
			var id3 = new Id(id.KeyString, id.GetKeyShort(), id2.KeyDateTime);

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ClassWithCompositeId(id1) {OneProperty = 5});
				s.Save(new ClassWithCompositeId(id2) {OneProperty = 10});
				s.Save(new ClassWithCompositeId(id3));

				t.Commit();
			}

			using (var s = OpenSession())
			{
				var results1 = s.CreateQuery("from ClassWithCompositeId x where x.Id in (:id1, :id2)")
								.SetParameter("id1", id1)
								.SetParameter("id2", id2)
								.List<ClassWithCompositeId>();
				var results2 = s.CreateQuery("from ClassWithCompositeId x where  x.Id in (:id1)")
								.SetParameter("id1", id1)
								.List<ClassWithCompositeId>();
				var results3 = s.CreateQuery("from ClassWithCompositeId x where  x.Id not in (:id1)")
								.SetParameter("id1", id1)
								.List<ClassWithCompositeId>();
				var results4 = s.CreateQuery("from ClassWithCompositeId x where x.Id not in (:id1, :id2)")
								.SetParameter("id1", id1)
								.SetParameter("id2", id2)
								.List<ClassWithCompositeId>();

				Assert.Multiple(
					() =>
					{
						Assert.That(results1.Count, Is.EqualTo(2), "in multiple ids");
						Assert.That(results1.Select(x => x.Id), Is.EquivalentTo(new[] {id1, id2}), "in multiple ids");
						Assert.That(results2.Count, Is.EqualTo(1), "in single id");
						Assert.That(results2.Single().Id, Is.EqualTo(id1), "in single id");
						Assert.That(results3.Count, Is.EqualTo(2), "not in single id");
						Assert.That(results3.Select(x => x.Id), Is.EquivalentTo(new[] {id2, id3}), "not in single id");
						Assert.That(results4.Count, Is.EqualTo(1), "not in multiple ids");
						Assert.That(results4.Single().Id, Is.EqualTo(id3), "not in multiple ids");
					});
			}
		}

		[Test]
		public void QueryOverInClauseSubquery()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
			{
					Assert.Ignore();
			}

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ClassWithCompositeId(id) {OneProperty = 5});
				s.Save(new ClassWithCompositeId(secondId) {OneProperty = 10});
				s.Save(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime)));

				t.Commit();
			}

			using (var s = OpenSession())
			{
				var results = s.QueryOver<ClassWithCompositeId>().WithSubquery.WhereProperty(p => p.Id).In(QueryOver.Of<ClassWithCompositeId>().Where(p => p.Id.KeyString == id.KeyString).Select(p => p.Id)).List();
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void HqlInClauseSubquery()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
				Assert.Ignore();

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ClassWithCompositeId(id) {OneProperty = 5});
				s.Save(new ClassWithCompositeId(secondId) {OneProperty = 10});
				s.Save(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime)));

				t.Commit();
			}

			using (var s = OpenSession())
			{
				var results = s.CreateQuery("from ClassWithCompositeId x where  x.Id in (select s.Id from ClassWithCompositeId s where s.Id.KeyString = :keyString)")
								.SetParameter("keyString", id.KeyString).List();
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}

		//GH-1376
		[Test]
		public void HqlInClauseSubquery_ForEntity()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
				Assert.Ignore();

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ClassWithCompositeId(id) {OneProperty = 5});
				s.Save(new ClassWithCompositeId(secondId) {OneProperty = 10});
				s.Save(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime)));

				t.Commit();
			}

			using (var s = OpenSession())
			{
				var results = s.CreateQuery("from ClassWithCompositeId x where x in (select s from ClassWithCompositeId s where s.Id.KeyString = :keyString)")
								.SetParameter("keyString", id.KeyString).List();
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}

		//NH-2926 (GH-1103)
		[Test]
		public void QueryOverOrderByAndWhereWithIdProjectionDoesntThrow()
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

			using (ISession s = OpenSession())
			{
				var results = s.QueryOver<ClassWithCompositeId>()
								.Select(Projections.Id())
								.Where(Restrictions.Eq(Projections.Id(), id))
								.OrderBy(Projections.Id()).Desc.List<Id>();
				Assert.That(results.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void CriteriaGroupProjection()
		{
			using (ISession s = OpenSession())
			{
				s.CreateCriteria<ClassWithCompositeId>()
				.SetProjection(Projections.GroupProperty(Projections.Id()))
				.Add(Restrictions.Eq(Projections.Id(), id))
				.List<Id>();
			}
		}

		[Test]
		public void QueryOverInClause()
		{
			// insert the new objects
			var id1 = id;
			var id2 = secondId;
			var id3 = new Id(id1.KeyString, id1.GetKeyShort(), id2.KeyDateTime);

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ClassWithCompositeId(id1) {OneProperty = 5});
				s.Save(new ClassWithCompositeId(id2) {OneProperty = 10});
				s.Save(new ClassWithCompositeId(id3));

				t.Commit();
			}

			using (var s = OpenSession())
			{
				var results1 = s.QueryOver<ClassWithCompositeId>().WhereRestrictionOn(p => p.Id).IsIn(new[] {id1, id2}).List();
				var results2 = s.QueryOver<ClassWithCompositeId>().WhereRestrictionOn(p => p.Id).IsIn(new[] {id1}).List();
				var results3 = s.QueryOver<ClassWithCompositeId>().WhereRestrictionOn(p => p.Id).Not.IsIn(new[] {id1, id2}).List();
				var results4 = s.QueryOver<ClassWithCompositeId>().WhereRestrictionOn(p => p.Id).Not.IsIn(new[] {id1}).List();

				Assert.Multiple(
					() =>
					{
						Assert.That(results1.Count, Is.EqualTo(2), "in multiple ids");
						Assert.That(results1.Select(r => r.Id), Is.EquivalentTo(new[] {id1, id2}), "in multiple ids");
						Assert.That(results2.Count, Is.EqualTo(1), "in single id");
						Assert.That(results2.Select(r => r.Id), Is.EquivalentTo(new[] {id1}), "in single id");
						Assert.That(results3.Count, Is.EqualTo(1), "not in multiple ids");
						Assert.That(results3.Select(r => r.Id), Is.EquivalentTo(new[] {id3}), "not in multiple ids");
						Assert.That(results4.Count, Is.EqualTo(2), "not in single id");
						Assert.That(results4.Select(r => r.Id), Is.EquivalentTo(new[] {id2, id3}), "not in single id");
					});
			}
		}
	}
}
