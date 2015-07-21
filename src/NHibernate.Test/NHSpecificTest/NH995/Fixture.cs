using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH995
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get
			{
				return "NH995";
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from ClassC");
				s.Delete("from ClassB");
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public void Test()
		{
			int a_id;
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				// Create an A and save it
				ClassA a = new ClassA();
				a.Name = "a1";
				s.Save(a);

				// Create a B and save it
				ClassB b = new ClassB();
				b.Id = new ClassBId("bbb", a);
				b.SomeProp = "Some property";
				s.Save(b);

				// Create a C and save it
				ClassC c = new ClassC();
				c.B = b;
				s.Save(c);

				tx.Commit();

				a_id = a.Id;
			}

			// Clear the cache
			sessions.Evict(typeof(ClassA));
			sessions.Evict(typeof(ClassB));
			sessions.Evict(typeof(ClassC));
			
			using(ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Load a so we can use it to load b
				ClassA a = s.Get<ClassA>(a_id);

				// Load b so b will be in cache
				ClassB b = s.Get<ClassB>(new ClassBId("bbb", a));

				tx.Commit();
			}
			
			using(ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					IList<ClassC> c_list = s.CreateCriteria(typeof (ClassC)).List<ClassC>();
					// make sure we initialize B
					NHibernateUtil.Initialize(c_list[0].B);

					Assert.AreEqual(1, sqlLogSpy.Appender.GetEvents().Length,
					                "Only one SQL should have been issued");
				}

				tx.Commit();
			}
		}
	}
}
