using System;

using NUnit.Framework;

using NHibernate;
using NHibernate.DomainModel;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for BlobClobTest.
	/// </summary>
	[TestFixture]
	public class BlobClobTest : TestCase
	{
		[Test]
		[Ignore("BLOB/CLOB not implmented like h2.0.3 - http://jira.nhibernate.org:8080/browse/NH-19")]
		public void BlobClob() 
		{
		}
	}
}
