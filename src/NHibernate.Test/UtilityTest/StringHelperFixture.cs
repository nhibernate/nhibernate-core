using System;

using NHibernate.Util;

using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for StringHelperFixture.
	/// </summary>
	[TestFixture]
	public class StringHelperFixture
	{
		[Test]
		public void GetClassnameFromFQType() 
		{
			string typeName = "ns1.ns2.classname, as1.as2., pk, lang";
			string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void GetClassnameFromFQClass() 
		{
			string typeName = "ns1.ns2.classname";
			string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}
	}
}
