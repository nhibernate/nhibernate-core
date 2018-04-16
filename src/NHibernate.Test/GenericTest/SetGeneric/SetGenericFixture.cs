using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NHibernate.Test.GenericTest.SetGeneric
{
	[TestFixture]
	public class SetGenericFixture : TestCase
	{

		protected override System.Collections.IList Mappings
		{
			get { return new string[] { "GenericTest.SetGeneric.SetGenericFixture.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
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
			a.Items = new HashSet<B>();
			B firstB = new B();
			firstB.Name = "first b";
			B secondB = new B();
			secondB.Name = "second b";

			a.Items.Add(firstB);
			a.Items.Add(secondB);

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
			a = s.Load<A>(a.Id);
			B thirdB = new B();
			thirdB.Name = "third B";
			// ensuring the correct generic type was constructed
			a.Items.Add(thirdB);
			Assert.AreEqual(3, a.Items.Count, "3 items in the set now");
			Assert.IsTrue( a.Items is NHibernate.Collection.Generic.PersistentGenericSet<B> );
			s.Flush();
			s.Close();
		}

		[Test]
		public void Copy()
		{
			A a = new A();
			a.Name = "original A";
			a.Items = new HashSet<B>();

			B b1 = new B();
			b1.Name = "b1";
			a.Items.Add( b1 );

			B b2 = new B();
			b2.Name = "b2";
			a.Items.Add( b2 );

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
				A loadedA = s.Get<A>( copiedA.Id );
				Assert.IsNotNull( loadedA );
				s.Delete( loadedA );
				t.Commit();
			}
		}
	}
}
