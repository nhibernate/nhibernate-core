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

		public void TestJoins() 
		{
		}

		public void SubclassCollection() 
		{
		}

		public void CollectionOnly() 
		{
		}

		public void Queries() 
		{
		}

		public void Constraints() 
		{
		}

		public void MultiTable() 
		{
		}

		public void MutliTableGeneratedId() 
		{
		}

		public void MultiTableCollections() 
		{
		}

		public void MultiTableManyToOne() 
		{
		}

		public void MultiTableNativeId() 
		{
		}
		
		public void Collection() 
		{
		}

		public void OneToOne() 
		{
		}

		public void CollectionPointer() 
		{
		}

	}
}
