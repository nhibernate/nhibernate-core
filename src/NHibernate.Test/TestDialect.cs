using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test
{
	/// <summary>
	/// Like NHibernate's Dialect class, but for differences only important during testing.
	/// Defaults to true for all support.  Users of different dialects can turn support
	/// off if the unit tests fail.
	/// </summary>
	public class TestDialect
	{
		public static TestDialect GetTestDialect(Dialect.Dialect dialect)
		{
			string testDialectTypeName = "NHibernate.Test.TestDialects." + dialect.GetType().Name.Replace("Dialect", "TestDialect");
			System.Type testDialectType = System.Type.GetType(testDialectTypeName);
			if (testDialectType != null)
				return (TestDialect)Activator.CreateInstance(testDialectType);
			return new TestDialect();
		}

		public virtual bool SupportsOperatorAll { get { return true; } }
		public virtual bool SupportsOperatorSome { get { return true; } }
		public virtual bool SupportsLocate { get { return true; } }
	}
}
