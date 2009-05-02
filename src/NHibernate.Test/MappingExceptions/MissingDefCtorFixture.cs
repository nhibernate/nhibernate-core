using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// A TestFixture to verify the Exception thrown when a mapped class is missing the default
	/// ctor is readable and understandable.
	/// </summary>
	[TestFixture]
	public class MissingDefCtorFixture
	{
		[Test]
		public void ClassMissingDefaultCtor()
		{
			// add a resource that doesn't exist
			string resource = "NHibernate.Test.MappingExceptions.MissingDefCtor.hbm.xml";
			Configuration cfg = new Configuration();
			cfg.AddResource(resource, this.GetType().Assembly);
			Assert.Throws<InstantiationException>(() =>cfg.BuildSessionFactory());
		}
	}
}