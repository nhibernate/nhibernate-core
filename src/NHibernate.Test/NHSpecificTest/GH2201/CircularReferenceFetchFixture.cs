using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2201
{

	[TestFixture]
	public class CircularReferenceFetchFixture : BaseFetchFixture
	{
		public CircularReferenceFetchFixture() : base(-1)
		{
		}

		[Test]
		public void QueryOver()
		{
			using (var session = OpenSession())
			{
				var result = session.QueryOver<Entity>().Where(e => e.EntityNumber == "Bob").SingleOrDefault();

				Verify(result);
			}
		}

		[Test]
		public void Get()
		{
			using (var session = OpenSession())
			{
				var result = session.Get<Entity>(_id);

				Verify(result);
			}
		}
	}
}
