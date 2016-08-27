using System.Linq;
using NUnit.Framework;
using NHibernate.DomainModel.Northwind.Entities;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class RegresstionTests : LinqTestCase
	{
		/// <summary>
		/// http://aspzone.com/tech/nhibernate-linq-troubles/
		/// </summary>
		[Test]
		public void HierarchicalQueries_InlineConstant()
		{
			var children = from s in db.Role
						   where s.ParentRole != null
						   select s;

			Assert.AreEqual(0, children.Count());

			var roots = from s in db.Role
						where s.ParentRole == null
						select s;

			Assert.AreEqual(2, roots.Count());
		}

		[Test]
		public void HierarchicalQueries_Variable()
		{
			Role testRole = null;
			var children = from s in db.Role
						   where s.ParentRole != testRole
						   select s;

			Assert.AreEqual(0, children.Count());

			var roots = from s in db.Role
						where s.ParentRole == testRole
						select s;

			Assert.AreEqual(2, roots.Count());
		}

		[Test]
		public void CanUseNullConstantAndRestriction()
		{
			var roots = from s in db.Role
						where s.ParentRole == null
						&& s.Name == "Admin"
						select s;

			Assert.AreEqual(1, roots.Count());
		}
	}
}