using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH369
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void KeyManyToOneAndNormalizedPersister()
		{
			Configuration cfg = new Configuration();
			cfg
				.AddClass(typeof(BaseClass))
				.AddClass(typeof(KeyManyToOneClass))
				.BuildSessionFactory().Close();
		}
	}
}