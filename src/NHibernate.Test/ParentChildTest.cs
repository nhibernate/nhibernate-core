using System;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for ParentChildTest.
	/// </summary>
	[TestFixture]
	public class ParentChildTest : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "FooBar.hbm.xml",
										   "Baz.hbm.xml",
										   "Qux.hbm.xml",
										   "Glarch.hbm.xml",
										   "Fum.hbm.xml",
										   "Fumm.hbm.xml",
										   "Fo.hbm.xml",
										   "One.hbm.xml",
										   "Many.hbm.xml",
										   "Immutable.hbm.xml",
										   "Fee.hbm.xml",
										   //"Vetoer.hbm.xml",
										   "Holder.hbm.xml",
										   "ParentChild.hbm.xml",
										   "Simple.hbm.xml",
										   "Container.hbm.xml",
										   "Circular.hbm.xml",
										   "Stuff.hbm.xml"
									   } );
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionQuery() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void ParentChild() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void ParentNullChild() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void ManyToMany() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Container() 
		{
		}
		
		[Test]
		[Ignore("Test not yet written")]
		public void CascadeCompositeElements() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Bag() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CircularCascade() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void DeleteEmpty() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Locking() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void ObjectType() 
		{
		}


	}
}
