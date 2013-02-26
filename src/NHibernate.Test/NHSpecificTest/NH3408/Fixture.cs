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
				var query = from c in session.Query<Country>()
							select new { c.Picture, c.NationalHolidays };

				var result = query.ToList();
			}
		}
	}
}