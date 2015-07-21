using System;
using NUnit.Framework;

namespace NHibernate.Test
{
	[TestFixture]
	public class ConventionsTestCase
	{
		[Test]
		public void NHibernate_should_be_cls_compliant()
		{
			CLSCompliantAttribute[] attributes = (CLSCompliantAttribute[])typeof(ISession).Assembly.GetCustomAttributes(typeof(CLSCompliantAttribute), true);
			Assert.AreNotEqual(0, attributes.Length, "NHibernate should specify CLS Compliant attribute");
			Assert.IsTrue(attributes[0].IsCompliant, "NHibernate should be CLS Compliant");
		}
	}
}