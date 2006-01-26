#region License
//
//  PrevalenceCache - A cache provider for NHibernate using Bamboo.Prevalence.
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

using System.Collections;
using System.IO;
using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Caches.Prevalence.Tests
{
	[TestFixture]
	public class PrevalenceCacheProviderFixture
	{
		private PrevalenceCacheProvider provider;
		private Hashtable props;
		private string testDir;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			log4net.Config.XmlConfigurator.Configure();
			props = new Hashtable();
			testDir = @"C:\temp\prevalence";
			props.Add( "prevalenceBase", testDir );
		}

		[SetUp]
		public void Setup()
		{
			provider = new PrevalenceCacheProvider();
		}

		[TearDown]
		public void Teardown()
		{
			if( Directory.Exists( testDir ) )
			{
				Directory.Delete( testDir );
			}
		}

		[Test]
		public void TestBuildCacheNullNull()
		{
			ICache cache = provider.BuildCache( null, null );
			Assert.IsNotNull( cache, "no cache returned" );
		}

		[Test]
		public void TestBuildCacheStringNull()
		{
			ICache cache = provider.BuildCache( "a_region", null );
			Assert.IsNotNull( cache, "no cache returned" );
		}

		[Test]
		public void TestBuildCacheStringICollection()
		{
			Assert.IsFalse( Directory.Exists( testDir ) );
			ICache cache = provider.BuildCache( "another_region", props );
			Assert.IsTrue( Directory.Exists( testDir ) );
			Assert.IsNotNull( cache, "no cache returned" );
		}

		[Test]
		public void TestNextTimestamp()
		{
			long ts = provider.NextTimestamp();
			Assert.IsNotNull( ts, "no timestamp returned" );
		}
	}
}
