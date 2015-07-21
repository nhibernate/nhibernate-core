using System;
using System.Collections.Generic;
using NHibernate.Mapping;
using NUnit.Framework;
using NHibernate.Cfg;

namespace NHibernate.Test.FilterTest
{
	[TestFixture]
	public class FilterSecondPassArgsFixture
	{
		public class FakeFilterable: IFilterable
		{
			public void AddFilter(string name, string condition)
			{
				throw new NotImplementedException();
			}

			public IDictionary<string, string> FilterMap
			{
				get { throw new NotImplementedException(); }
			}
		}
		[Test]
		public void CtorProtection()
		{
			Assert.Throws<ArgumentNullException>(() => new FilterSecondPassArgs(null, ""));
			Assert.Throws<ArgumentNullException>(() => new FilterSecondPassArgs(null, "a>1"));
			Assert.Throws<ArgumentNullException>(() => new FilterSecondPassArgs(new FakeFilterable(), null));
			Assert.Throws<ArgumentNullException>(() => new FilterSecondPassArgs(new FakeFilterable(), ""));
			Assert.DoesNotThrow(() => new FilterSecondPassArgs(new FakeFilterable(), "a>1"));
		}
	}
}
