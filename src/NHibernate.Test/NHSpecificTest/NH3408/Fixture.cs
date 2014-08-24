using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3408
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void ProjectAnonymousTypeWithArrayProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query = from c in session.Query<Country>()
							select new { c.Picture, c.NationalHolidays };

				Assert.DoesNotThrow(() => { query.ToList(); });
			}
		}

		[Test]
		public void ProjectAnonymousTypeWithArrayPropertyWhenByteArrayContains()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var pictures = new List<byte[]>();
				var query = from c in session.Query<Country>()
							where pictures.Contains(c.Picture)
							select new { c.Picture, c.NationalHolidays };

				Assert.DoesNotThrow(() => { query.ToList(); });
			}
		}

		[Test]
		public void SelectBytePropertyWithArrayPropertyWhenByteArrayContains()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var pictures = new List<byte[]>();
				var query = from c in session.Query<Country>()
							where pictures.Contains(c.Picture)
							select c.Picture;

				Assert.DoesNotThrow(() => { query.ToList(); });
			}
		}
	}
}
