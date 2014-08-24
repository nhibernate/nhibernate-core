using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH298 {

	[TestFixture]
	public class IndexedBidirectionalOneToManyTest : BugTestCase {

		protected override void OnSetUp() {
			base.OnSetUp();
			using ( ISession session = this.OpenSession() ) {
				Category root = new Category( 1, "Root", null );
				root.SubCategories.Add( new Category( 2, "First", root ) );
				root.SubCategories.Add( new Category( 3, "Second", root ) );
				root.SubCategories.Add( new Category( 4, "Third", root ) );

				session.Save( root );
				session.Flush();
			}
		}

		protected override void OnTearDown() {
			base.OnTearDown();
			using ( ISession session = this.OpenSession() ) {
				session.Delete( "from System.Object" );
				//session.CreateSQLQuery( "delete from Category" ).List();
				session.Flush();
			}
		}

		[Test]
		public void SubItemMovesCorrectly() {
			Category root1 = null, itemToMove = null;

			using ( ISession session = this.OpenSession() ) {
				root1 = session.Get<Category>( 1 );
				itemToMove = root1.SubCategories[1]; //get the middle item
				root1.SubCategories.Remove( itemToMove ); //remove the middle item
				root1.SubCategories.Add( itemToMove ); //re-add it to the end

				session.Update( root1 );
				session.Flush();
			}

			using ( ISession session = this.OpenSession() ) {
				Category root2 = session.Get<Category>( 1 );
				Assert.AreEqual( root1.SubCategories.Count, root2.SubCategories.Count );
				Assert.AreEqual( root1.SubCategories[1].Id, root2.SubCategories[1].Id );
				Assert.AreEqual( root1.SubCategories[2].Id, root2.SubCategories[2].Id );
				Assert.AreEqual( itemToMove.Id, root1.SubCategories[2].Id );
			}
		}

		[Test]
		public void RemoveAtWorksCorrectly() {
			Category root1 = null;

			using ( ISession session = this.OpenSession() ) {
				root1 = session.Get<Category>( 1 );
				root1.SubCategories.RemoveAt( 1 );

				session.Update( root1 );
				session.Flush();
			}

			using ( ISession session = this.OpenSession() ) {
				Category root2 = session.Get<Category>( 1 );
				Assert.AreEqual( root1.SubCategories.Count, root2.SubCategories.Count );
			}
		}
	}
}
