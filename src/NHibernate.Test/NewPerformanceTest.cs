using System;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for NewPerformanceTest.
	/// </summary>
	[TestFixture]
	public class NewPerformanceTest : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Performance() 
		{
		}

		private void Prepare() 
		{
			//TODO: add details - not a test method
		}

		private void Delete() 
		{
			//TODO: add details - not a test method
		}

	}
}
