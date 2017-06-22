using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NHibernate.Test.GenericTest.IdBagGeneric
{
	[TestFixture]
	public class IdBagGenericFixture : TestCase
	{
		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "GenericTest.IdBagGeneric.IdBagGenericFixture.hbm.xml" }; }
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
			Assert.AreEqual( "first string", a.Items[ 0 ] );

			s = OpenSession();
			a = s.Load<A>( a.Id );
			Assert.AreEqual( "first string", a.Items[ 0 ], "first item should be 'first string'" );
			Assert.AreEqual( "second string", a.Items[ 1 ], "second item should be 'second string'" );
			// ensuring the correct generic type was constructed
			a.Items.Add( "third string" );
			Assert.AreEqual( 3, a.Items.Count, "3 items in the list now" );

			a.Items[ 1 ] = "new second string";
			s.Flush();
			s.Close();
		}

		[Test]
		public void Copy()
		{
			A a = new A();
			a.Name = "original A";
			a.Items = new List<string>();

			a.Items.Add( "b1" );
			a.Items.Add( "b2" );

			A copiedA;
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				copiedA = s.Merge(a);
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				A loadedA = s.Get<A>(copiedA.Id);
				Assert.IsNotNull( loadedA );
				s.Delete( loadedA );
				t.Commit();
			}
		}
	}
}
