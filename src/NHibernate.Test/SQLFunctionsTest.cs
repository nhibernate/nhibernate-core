using System;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for SQLFunctionsTest.
	/// </summary>
	[TestFixture]
	public class SQLFunctionsTest : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"//,
										   //"Blobber.hbm.xml"
									   } );
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void SetProperties() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NothingToUpdate() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void SQLFunctions() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void BlobClob() 
		{
		}


	}
}
