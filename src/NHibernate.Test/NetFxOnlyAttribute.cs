using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHibernate.Test
{
	public class NetFxOnlyAttribute : Attribute, ITestAction
	{
		public ActionTargets Targets => ActionTargets.Default;

		public void AfterTest(ITest test) { }

		public void BeforeTest(ITest test)
		{
#if !NETFX
			Assert.Ignore("This test is only for NETFX.");
#endif
		}
	}
}
