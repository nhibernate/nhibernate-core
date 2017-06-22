using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	[TestFixture]
	public class IdBagFixture : TestCase
	{
		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "CollectionTest.IdBagFixture.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using( ISession s = Sfi.OpenSession() )
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
			a.Items = new List<string>();
			a.Items.Add( "first string" );
			a.Items.Add( "second string" );

			ISession s = OpenSession();
			s.SaveOrUpdate( a );
			// this flush should test how NH wraps a generic collection with its
			// own persistent collection
			s.Flush();
			s.Close();
			Assert.IsNotNull( a.Id );
			Assert.AreEqual( "first string", ( string ) a.Items[ 0 ] );

			s = OpenSession();
			a = ( A ) s.Load( typeof( A ), a.Id );
			Assert.AreEqual( "first string", ( string ) a.Items[ 0 ], "first item should be 'first string'" );
			Assert.AreEqual( "second string", ( string ) a.Items[ 1 ], "second item should be 'second string'" );
			// ensuring the correct generic type was constructed
			a.Items.Add( "third string" );
			Assert.AreEqual( 3, a.Items.Count, "3 items in the list now" );

			a.Items[ 1 ] = "new second string";
			s.Flush();
			s.Close();
		}
	}
}
