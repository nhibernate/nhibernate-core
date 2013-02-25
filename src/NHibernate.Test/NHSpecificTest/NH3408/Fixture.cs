using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3408
{
	using System.Linq;

	public class Fixture : BugTestCase
	{
		[Test]
		public void ProjectAnonymousTypeWithArrayProperty()
		{
			using (var session = OpenSession())
			{
				var query = from p in session.Query<Person>()
							select new { p.Photo };

				var result = query.ToList();
			}
		}
	}
}