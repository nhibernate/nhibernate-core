using System;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for NewerPerformanceTest.
	/// </summary>
	[TestFixture]
	public class NewerPerformanceTest : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Many() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Simultaneous()
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NHibernateOnly()
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AdoOnly()
		{
		}

		private void NHibernate()
		{
			//TODO: add details - not a test method
		}

		private void DirectAdo()
		{
			//TODO: add details - not a test method
		}
	}
}
