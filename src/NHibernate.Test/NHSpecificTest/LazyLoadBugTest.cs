using System;
using System.Collections;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test 
{
	[TestFixture]
	public class LazyLoadBugTest : TestCase 
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.LazyLoadBug.hbm.xml"} );
		}

		[Test]
		[ExpectedException(typeof(LazyInitializationException))]
		public void TestLazyLoad() 
		{
			int parentId = 0;

			using( ISession s1 = sessions.OpenSession() )
			using( ITransaction t1 = s1.BeginTransaction() )
			{
				// create a new
				LLParent parent = new LLParent();
				LLChild child = new LLChild();
				child.Parent = parent;

				s1.Save( parent );
				parentId = (int)s1.GetIdentifier( parent );
				
				t1.Commit();
			}

			// try to Load the object to get the exception
			using( ISession s2 = sessions.OpenSession() )
			using( ITransaction t2 = s2.BeginTransaction() )
			{
				LLParent parent2 = (LLParent)s2.Load( typeof(LLParent), parentId );

				// this should throw an exception
				int count = parent2.Children.Count;
			}
		}

		[Test]
		public void TestLazyLoadNoAdd() 
		{
			int parentId = 0;

			using( ISession s1 = sessions.OpenSession() )
			using( ITransaction t1 = s1.BeginTransaction() )
			{
				// create a new
				LLParent parent = new LLParent();
				LLChildNoAdd child = new LLChildNoAdd();
				parent.ChildrenNoAdd.Add( child );
				child.Parent = parent;

				s1.Save( parent );
				parentId = (int)s1.GetIdentifier( parent );
				
				t1.Commit();
			}

			// try to Load the object to make sure the save worked
			using( ISession s2 = sessions.OpenSession() )
			using( ITransaction t2 = s2.BeginTransaction() )
			{
				LLParent parent2 = (LLParent)s2.Load( typeof( LLParent ), parentId );
				Assert.AreEqual( 1, parent2.ChildrenNoAdd.Count );
			}
		}
	}
}
