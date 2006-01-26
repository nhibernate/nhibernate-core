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

using System.Xml;
using NUnit.Framework;

namespace NHibernate.Caches.SysCache.Tests
{
	[TestFixture]
	public class SysCacheSectionHandlerFixture
	{
		private SysCacheSectionHandler handler;
		private XmlNode section;
		private string xml = "<syscache><cache region=\"foo\" expiration=\"500\" priority=\"4\" /></syscache>";

		[SetUp]
		public void Init()
		{
			handler = new SysCacheSectionHandler();
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
			Assert.IsTrue( result is CacheConfig[] );
			CacheConfig[] caches = result as CacheConfig[];
			Assert.AreEqual( 0, caches.Length );
		}

		[Test]
		public void TestGetConfigFromFile()
		{
			object result = handler.Create( null, null, section );
			Assert.IsNotNull( result );
			Assert.IsTrue( result is CacheConfig[] );
			CacheConfig[] caches = result as CacheConfig[];
			Assert.AreEqual( 1, caches.Length );
		}
	}
}
