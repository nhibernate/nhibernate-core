using System;
using System.Collections;
using NUnit.Framework;

using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class VersionTest
	{
		[Test]
		public void UnsavedNegativeIntOrShort()
		{
			Cascades.VersionValue negative = Cascades.VersionValue.VersionNegative;

			Assert.AreEqual(true, negative.IsUnsaved( (short) -1 ) );
			Assert.AreEqual(true, negative.IsUnsaved( -1  ) );
			Assert.AreEqual(true, negative.IsUnsaved( -1L ) );
		}
	}
}
