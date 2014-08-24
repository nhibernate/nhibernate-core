using System;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class SoftLimitMRUCacheFixture
	{
		[Test]
		public void DontFillUp()
		{
			// NH-1671
			const int count = 32;
			var s = new SoftLimitMRUCache(count);
			for (int i = 0; i < count+10; i++)
			{
				s.Put(new object(), new object());
			}
			Assert.That(s.Count, Is.EqualTo(count));

			GC.Collect();
			s.Put(new object(), new object());

			Assert.That(s.SoftCount, Is.EqualTo(count + 1));
		}
	}
}