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
		public void ParentChild() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent();
			Child c = new Child();
			c.Parent = p;
			p.Child = c;
			s.Save(p);
			s.Save(c);
			t.Commit();
			s.Flush();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Child)s.Load( typeof(Child), c.Id );
			p = c.Parent;
			Assert.IsNotNull( p, "1-1 parent" );
			c.Count = 32;
			p.Count = 66;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Child)s.Load( typeof(Child), c.Id );
			p = c.Parent;
			Assert.AreEqual( 66, p.Count, "1-1 update" );
			Assert.AreEqual( 32, c.Count, "1-1 update" );
			Assert.AreEqual( 1, s.Find("from c in class NHibernate.DomainModel.Child where c.Parent.Count=66").Count, "1-1 query" );
			Assert.AreEqual( 2, ((object[])s.Find("from Parent p join p.Child c where p.Count=66")[0]).Length, "1-1 query" );

			s.Find("select c, c.Parent from c in class NHibernate.DomainModel.Child order by c.Parent.Count");
			s.Find("select c, c.Parent from c in class NHibernate.DomainModel.Child where c.Parent.Count=66 order by c.Parent.Count");
			s.Enumerable("select c, c.Parent, c.Parent.Count from c in class NHibernate.DomainModel.Child order by c.Parent.Count");
			Assert.AreEqual( 1, s.Find("FROM p in CLASS NHibernate.DomainModel.Parent WHERE p.Count=?", (int)66, NHibernate.Int32).Count, "1-1 query" );
			s.Delete(c);
			s.Delete(p);
			t.Commit();
			s.Close();
		}

		[Test]
		public void ParentNullChild() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Parent p = new Parent();
			s.Save(p);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			p = (Parent)s.Load( typeof(Parent), p.Id );
			Assert.IsNull( p.Child );
			p.Count = 66;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			p = (Parent)s.Load( typeof(Parent), p.Id );
			Assert.AreEqual( 66, p.Count, "null 1-1 update" );
			Assert.IsNull( p.Child );
			s.Delete(p);
			t.Commit();
			s.Close();

		}

		[Test]
		public void ManyToMany() 
		{
			// if( dialect is Dialect.HSQLDialect) return;

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Container c = new Container();
			c.ManyToMany = new ArrayList();
			c.Bag = new ArrayList();
			Simple s1 = new Simple();
			Simple s2 = new Simple();
			s1.Count = 123;
			s2.Count = 654;
			Contained c1 = new Contained();
			c1.Bag = new ArrayList();
			c1.Bag.Add(c);
			c.Bag.Add(c1);
			c.ManyToMany.Add(s1);
			c.ManyToMany.Add(s2);
			object cid = s.Save(c);
			s.Save( s1, (long)12 );
			s.Save( s2, (long)-1 );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container) s.Load( typeof(Container), cid );
			Assert.AreEqual( 1, c.Bag.Count );
			Assert.AreEqual( 2, c.ManyToMany.Count );
			foreach(object obj in c.Bag) 
			{
				c1 = (Contained)obj;
				break;
			}
			c.Bag.Remove(c1);
			c1.Bag.Remove(c);
			Assert.IsNotNull( c.ManyToMany[0] );
			c.ManyToMany.RemoveAt(0);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container) s.Load( typeof(Container), cid );
			Assert.AreEqual( 0, c.Bag.Count );
			Assert.AreEqual( 1, c.ManyToMany.Count );
			c1 = (Contained) s.Load( typeof(Contained), c1.Id );
			Assert.AreEqual( 0, c1.Bag.Count );
			Assert.AreEqual( 1, s.Delete("from c in class ContainerX") );
			Assert.AreEqual( 1, s.Delete("from c in class Contained") );
			Assert.AreEqual( 2, s.Delete("from s in class Simple") );
			t.Commit();
			s.Close();
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Container() 
		{
		}
		
		[Test]
		//[Ignore("Test not yet written")]
		public void CascadeCompositeElements() 
		{
			Container c = new Container();
			IList list = new ArrayList();
			c.Cascades = list;
			Container.ContainerInnerClass cic = new Container.ContainerInnerClass();
			cic.Many = new Many();
			cic.One = new One();
			list.Add(cic);
			ISession s = sessions.OpenSession();
			s.Save(c);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			foreach(Container obj in s.Enumerable("from c in class ContainerX")) 
			{
				c = obj;
				break;
			}
			foreach(Container.ContainerInnerClass obj in c.Cascades) 
			{
				cic = obj;
				break;
			}
			Assert.IsNotNull(cic.Many);
			Assert.IsNotNull(cic.One);
			Assert.AreEqual( 1, c.Cascades.Count );
			s.Delete(c);
			s.Flush();
			s.Close();

			c = new Container();
			s = sessions.OpenSession();
			s.Save(c);
			list = new ArrayList();
			c.Cascades = list;
			cic = new Container.ContainerInnerClass();
			cic.Many = new Many();
			cic.One = new One();
			list.Add(cic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			foreach( Container obj in s.Enumerable("from c in class ContainerX") ) 
			{
				c = obj;
			}
			foreach( Container.ContainerInnerClass obj in c.Cascades ) 
			{
				cic = obj;
			}
			Assert.IsNotNull( cic.Many );
			Assert.IsNotNull( cic.One );
			Assert.AreEqual( 1, c.Cascades.Count );
			s.Delete(c);
			s.Flush();
			s.Close();

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
