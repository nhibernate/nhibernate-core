using System;
using System.Collections;

using NHibernate.DomainModel;

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
										   "Vetoer.hbm.xml",
										   "Holder.hbm.xml",
										   "ParentChild.hbm.xml",
										   "Simple.hbm.xml",
										   "Container.hbm.xml",
										   "Circular.hbm.xml",
										   "Stuff.hbm.xml"
									   } );
		}

		[Test]
		public void CollectionQuery() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			Simple s1 = new Simple();
			s1.Name = "s";
			s1.Count = 0;
			Simple s2 = new Simple();
			s2.Count = 2;
			Simple s3 = new Simple();
			s3.Count = 3;
			s.Save( s1, (long)1 );
			s.Save( s2, (long)2 );
			s.Save( s3, (long)3 );
			Container c = new Container();
			IList l = new ArrayList();
			l.Add(s1);
			l.Add(s3);
			l.Add(s2);
			c.OneToMany = l;
			l = new ArrayList();
			l.Add(s1);
			l.Add(null);
			l.Add(s2);
			c.ManyToMany = l;
			s.Save(c);

			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX, s in class Simple where c.OneToMany[2] = s").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX, s in class Simple where c.ManyToMany[2] = s").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX, s in class Simple where s = c.OneToMany[2]").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX, s in class Simple where s = c.ManyToMany[2]").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.OneToMany[0].Name = 's'").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.ManyToMany[0].Name = 's'").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where 's' = c.OneToMany[2 - 2].Name").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where 's' = c.ManyToMany[(3+1)/4-1].Name").Count );
			if( !(dialect is Dialect.MySQLDialect) ) 
			{
				Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.ManyToMany[ c.ManyToMany.maxIndex ].Count = 2").Count );
				Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.ManyToMany[ maxindex(c.ManyToMany) ].Count = 2").Count );
			}
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.OneToMany[ c.ManyToMany[0].Count ].Name = 's'").Count );
			Assert.AreEqual( 1, s.Find("select c from c in class ContainerX where c.ManyToMany[ c.OneToMany[0].Count ].Name = 's'").Count );
			
			s.Delete(c);
			s.Delete(s1);
			s.Delete(s2);
			s.Delete(s3);

			t.Commit();
			s.Close();

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
