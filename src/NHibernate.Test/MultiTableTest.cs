using System;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for MultiTableTest.
	/// </summary>
	[TestFixture]
	public class MultiTableTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{

			ExportSchema(new string[] { "Multi.hbm.xml"}, true);
		}

		[Test]
		[Ignore("Test not yet written")]
		public void TestJoins() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void SubclassCollection() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionOnly() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Queries() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Constraints() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiTable() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MutliTableGeneratedId() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiTableCollections() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiTableManyToOne() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiTableNativeId() 
		{
		}
		
		[Test]
		[Ignore("Test not yet written")]
		public void Collection() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void OneToOne() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionPointer() 
		{
		}

	}
}
