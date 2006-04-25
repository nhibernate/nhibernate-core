#if NET_2_0

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

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
			using (ISession s = sessions.OpenSession())
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void Simple()
		{
			A a = new A();
			a.Name = "first generic type";
			a.Items = new Dictionary<string,B>();
			B firstB = new B();
			firstB.Name = "first b";
			B secondB = new B();
			secondB.Name = "second b";

			a.Items.Add( "first", firstB );
			a.Items.Add( "second", secondB );

			ISession s = OpenSession();
			s.SaveOrUpdate(a);
			// this flush should test how NH wraps a generic collection with its
			// own persistent collection
			s.Flush();
			s.Close();
			Assert.IsNotNull(a.Id);
			// should have cascaded down to B
			Assert.IsNotNull(firstB.Id);
			Assert.IsNotNull(secondB.Id);

			s = OpenSession();
			a = s.Load<A>(a.Id );
			B thirdB = new B();
			thirdB.Name = "third B";
			// ensuring the correct generic type was constructed
			a.Items.Add( "third", thirdB );
			Assert.AreEqual( 3, a.Items.Count, "3 items in the map now" );
			s.Flush();
			s.Close();
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
	}
}
#endif