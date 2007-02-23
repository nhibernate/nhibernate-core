using System;
using NUnit.Framework;

namespace NHibernate.Test.ExceptionsTest
{
	[TestFixture]
	public class PropertyAccessExceptionFixture
	{
		/// <summary>
		/// Verifying that NH-358 has been fixed.
		/// </summary>
		[Test]
		public void MessageWithoutTypeCtor()
		{
			PropertyAccessException exc = new PropertyAccessException(null, "notype", true, null, "PropName");
			Assert.AreEqual("notype setter of UnknownType.PropName", exc.Message);
		}
	}
}