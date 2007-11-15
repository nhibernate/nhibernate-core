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
			bool excCaught = false;

			// add a resource that doesn't exist
			string resource = "NHibernate.Test.MappingExceptions.MissingDefCtor.hbm.xml";
			Configuration cfg = new Configuration();
			try
			{
				cfg.AddResource(resource, this.GetType().Assembly);
				cfg.BuildSessionFactory();
			}
			catch (MappingException me)
			{
				Assert.AreEqual(
					"Object class NHibernate.Test.MappingExceptions.MissingDefCtor must declare a default (no-argument) constructor",
					me.Message);
				excCaught = true;
			}

			Assert.IsTrue(excCaught, "Should have caught the MappingException about default ctor being missing.");
		}
	}
}