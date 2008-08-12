using System;
using System.Linq;
using NUnit.Framework;
using NHibernate.Linq.Test.Model;

namespace NHibernate.Linq.Test
{
	[TestFixture]
	public class SelectTest : BaseTest
	{
		[Test]
		[Ignore("this doesn't work yet")]
		public void CanSelectAnimals()
		{
			var animals = session.Linq<Animal>();
			Assert.IsNotNull(animals);
		}
	}
}
