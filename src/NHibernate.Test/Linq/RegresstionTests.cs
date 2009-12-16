using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class RegresstionTests : LinqTestCase
	{
		/// <summary>
		/// http://aspzone.com/tech/nhibernate-linq-troubles/
		/// </summary>
		[Test]
		public void HierarchicalQueries()
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
	}
}