#region License

//
//  SysCache - A cache provider for NHibernate using System.Web.Caching.Cache.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// CLOVER:OFF
//

#endregion

using System;
using System.Collections;

using log4net.Config;

using NHibernate.Cache;

using NUnit.Framework;

namespace NHibernate.Caches.SysCache.Tests
{
	[TestFixture]
	public class SysCacheProviderFixture
	{
		private ICacheProvider provider;
		private Hashtable props;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			XmlConfigurator.Configure();
			props = new Hashtable();
			props.Add("expiration", 120);
			props.Add("priority", 2);
		}

		[SetUp]
		public void Setup()
		{
			provider = new SysCacheProvider();
		}

		[Test]
		public void TestBuildCacheFromConfig()
		{
			ICache cache = provider.BuildCache("foo", null);
			Assert.IsNotNull(cache, "pre-configured cache not found");
		}

		[Test]
		public void TestBuildCacheNullNull()
		{
			ICache cache = provider.BuildCache(null, null);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestBuildCacheStringNull()
		{
			ICache cache = provider.BuildCache("a_region", null);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestBuildCacheStringICollection()
		{
			ICache cache = provider.BuildCache("another_region", props);
			Assert.IsNotNull(cache, "no cache returned");
		}

		[Test]
		public void TestNextTimestamp()
		{
			long ts = provider.NextTimestamp();
			Assert.IsNotNull(ts, "no timestamp returned");
		}
	}
}