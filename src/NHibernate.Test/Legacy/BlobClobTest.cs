using System;

using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	/// <summary>
	/// Summary description for BlobClobTest.
	/// </summary>
	[TestFixture]
	public abstract class BlobClobTest : TestCase
	{
		[Test]
		[Ignore("BLOB/CLOB not implmented like h2.0.3 - http://jira.nhibernate.org:8080/browse/NH-19")]
		public void BlobClob() 
		{
		}
	}
}
