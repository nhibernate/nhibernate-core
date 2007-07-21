using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate.Collection.Generic;

namespace NHibernate.Test.GenericTest.MapGeneric
{
	[TestFixture]
	public class MapGenericFixture : TestCase
	{
		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "GenericTest.MapGeneric.MapGenericFixture.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using( ISession s = sessions.OpenSession() )
			{
				s.Delete( "from A" );
				s.Flush();
			}
		}

		[Test]
		public void Simple()
		{
			A a = new A();
			a.Name = "first generic type";
			a.Items = new Dictionary<string, B>();
			B firstB = new B();
			firstB.Name = "first b";
			B secondB = new B();
			secondB.Name = "second b";

			a.Items.Add( "first", firstB );
			a.Items.Add( "second", secondB );

			using (ISession s = OpenSession())
			{
				s.SaveOrUpdate(a);
				// this flush should test how NH wraps a generic collection with its
				// own persistent collection
				s.Flush();
			}

			Assert.IsNotNull( a.Id );
			// should have cascaded down to B
			Assert.IsNotNull( firstB.Id );
			Assert.IsNotNull( secondB.Id );

			using (ISession s = OpenSession())
			{
				a = s.Load<A>(a.Id);
				B thirdB = new B();
				thirdB.Name = "third B";
				// ensuring the correct generic type was constructed
				a.Items.Add("third", thirdB);
				Assert.AreEqual(3, a.Items.Count, "3 items in the map now");
				s.Flush();
			}

			// NH-839
			using (ISession s = OpenSession())
			{
				a = s.Load<A>( a.Id );
				a.Items["second"] = a.Items["third"];
				s.Flush();
			}
		}

		// NH-669
		[Test]
		public void UpdatesToSimpleMap()
		{
			A a = new A();
			a.Name = "A";

			using( ISession s = OpenSession() )
			{
				s.Save( a );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				a = s.Load<A>( a.Id );
				a.SortedList.Add("abc", 10);
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void Copy()
		{
			A a = new A();
			a.Name = "original A";
			a.Items = new Dictionary<string, B>();

			B b1 = new B();
			b1.Name = "b1";
			a.Items[ "b1" ] = b1;

			B b2 = new B();
			b2.Name = "b2";
			a.Items[ "b2" ] = b2;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.SaveOrUpdateCopy( a );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				A loadedA = s.Get<A>( a.Id );
				Assert.IsNotNull( loadedA );
				s.Delete( loadedA );
				t.Commit();
			}
		}

		[Test]
		public void SortedCollections()
		{
			A a = new A();
			a.SortedDictionary = new SortedDictionary<string, int>();
			a.SortedList = new SortedList<string, int>();

			a.SortedDictionary[ "10" ] = 5;
			a.SortedList[ "20" ] = 10;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.Save( a );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				a = s.Load<A>( a.Id );
				PersistentGenericMap<string, int> sd = a.SortedDictionary as PersistentGenericMap<string, int>;
				Assert.IsNotNull( sd );
				// This is a hack to check that the internal collection is a SortedDictionary<,>.
				// The hack works because PersistentGenericMap.Entries() returns the internal collection
				// casted to IEnumerable
				Assert.IsTrue( sd.Entries() is SortedDictionary<string, int> );

				PersistentGenericMap<string, int> sl = a.SortedList as PersistentGenericMap<string, int>;
				Assert.IsNotNull( sl );
				// This is a hack, see above
				Assert.IsTrue( sl.Entries() is SortedList<string, int> );

				t.Commit();
			}
		}
	}
}
