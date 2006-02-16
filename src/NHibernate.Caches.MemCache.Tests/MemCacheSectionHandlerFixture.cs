#region License
//
//  MemCache - A cache provider for NHibernate using the .NET client
//  (http://sourceforge.net/projects/memcacheddotnet) for memcached,
//  which is located at http://www.danga.com/memcached/.
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

using System.Xml;
using NUnit.Framework;

namespace NHibernate.Caches.MemCache.Tests
{
	[TestFixture]
	public class MemCacheSectionHandlerFixture
	{
		private MemCacheSectionHandler handler;
		private XmlNode section;
		private string xml = "<memcache><memcached host=\"192.168.1.105\" port=\"11211\" weight=\"4\" /></memcache>";

		[SetUp]
		public void Init()
		{
			handler = new MemCacheSectionHandler();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );
			section = doc.DocumentElement;
		}

		[Test]
		public void TestGetConfigNullSection()
		{
			section = new XmlDocument();
			object result = handler.Create( null, null, section );
			Assert.IsNotNull( result );
			Assert.IsTrue( result is MemCacheConfig[] );
			MemCacheConfig[] caches = result as MemCacheConfig[];
			Assert.AreEqual( 0, caches.Length );
		}

		[Test]
		public void TestGetConfigFromFile()
		{
			object result = handler.Create( null, null, section );
			Assert.IsNotNull( result );
			Assert.IsTrue( result is MemCacheConfig[] );
			MemCacheConfig[] caches = result as MemCacheConfig[];
			Assert.AreEqual( 1, caches.Length );
		}
	}
}
