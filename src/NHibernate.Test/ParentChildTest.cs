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
		public void Container() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Container c = new Container();
			Simple x = new Simple();
			x.Count = 123;
			Simple y = new Simple();
			y.Count = 456;
			s.Save( x, (long)1 );
			s.Save( y, (long)0 );
			IList o2m = new ArrayList();
			o2m.Add(x);
			o2m.Add(null);
			o2m.Add(y);
			IList m2m = new ArrayList();
			m2m.Add(x);
			m2m.Add(null);
			m2m.Add(y);
			c.OneToMany = o2m;
			c.ManyToMany = m2m;
			IList comps = new ArrayList();
			Container.ContainerInnerClass ccic = new Container.ContainerInnerClass();
			ccic.Name = "foo";
			ccic.Simple = x;
			comps.Add(ccic);
			comps.Add(null);
			ccic = new Container.ContainerInnerClass();
			ccic.Name = "bar";
			ccic.Simple = y;
			comps.Add(ccic);

			IDictionary compos = new Hashtable();
			object emptyObj = new object();
			compos.Add( ccic, emptyObj );
			c.Composites = compos;
			c.Components = comps;
			One one = new One();
			Many many = new Many();
			IDictionary manies = new Hashtable();
			manies.Add( many, emptyObj );
			one.Manies = manies;
			many.One = one;
			ccic.Many = many;
			ccic.One = one;
			s.Save(one);
			s.Save(many);
			s.Save(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Load( typeof(Container), c.Id );
			
			ccic = (Container.ContainerInnerClass)c.Components[2];
			Assert.AreEqual( ccic.One, ccic.Many.One );
			Assert.AreEqual( 3, c.Components.Count );
			Assert.AreEqual( 1, c.Composites.Count );
			Assert.AreEqual( 3, c.OneToMany.Count );
			Assert.AreEqual( 3, c.ManyToMany.Count );

			for( int i=0; i<3; i++ ) 
			{
				Assert.AreEqual( c.ManyToMany[i], c.OneToMany[i] );
			}
			object o1 = c.OneToMany[0];
			object o2 = c.OneToMany[2];
			c.OneToMany.RemoveAt(2);
			c.OneToMany[0] = o2;
			c.OneToMany[1] = o1;
			o1 = c.Components[2];
			c.Components.RemoveAt(2);
			c.Components[0] = o1;
			c.ManyToMany[0] = c.ManyToMany[2];
			c.Composites.Add(o1, emptyObj);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Load( typeof(Container), c.Id );
			Assert.AreEqual( 1, c.Components.Count ); //WAS: 2 - h2.0.3 comment
			Assert.AreEqual( 2, c.Composites.Count );
			Assert.AreEqual( 2, c.OneToMany.Count );
			Assert.AreEqual( 3, c.ManyToMany.Count );
			Assert.IsNotNull( c.OneToMany[0] );
			Assert.IsNotNull( c.OneToMany[1] );

			( (Container.ContainerInnerClass)c.Components[0]).Name = "a different name";
			IEnumerator enumer = c.Composites.Keys.GetEnumerator();
			enumer.MoveNext();
			( (Container.ContainerInnerClass)enumer.Current).Name = "once again";
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Load( typeof(Container), c.Id );
			Assert.AreEqual( 1, c.Components.Count ); //WAS: 2 -> h2.0.3 comment
			Assert.AreEqual( 2, c.Composites.Count );
			Assert.AreEqual( "a different name", ((Container.ContainerInnerClass)c.Components[0]).Name );
			enumer = c.Composites.Keys.GetEnumerator();
			bool found = false;
			while( enumer.MoveNext() ) 
			{
				if( ( (Container.ContainerInnerClass)enumer.Current).Name.Equals("once again") ) 
				{
					found = true;
				}
			}

			Assert.IsTrue(found);
			c.OneToMany.Clear();
			c.ManyToMany.Clear();
			c.Composites.Clear();
			c.Components.Clear();
			s.Delete("from s in class Simple");
			s.Delete("from m in class Many");
			s.Delete("from o in class One");
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Load( typeof(Container), c.Id );
			Assert.AreEqual( 0, c.Components.Count );
			Assert.AreEqual( 0, c.Composites.Count );
			Assert.AreEqual( 0, c.OneToMany.Count );
			Assert.AreEqual( 0, c.ManyToMany.Count );
			s.Delete(c);
			t.Commit();
			s.Close();



		}
		
		[Test]
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
		public void Bag() 
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Container c = new Container();
			Contained c1 = new Contained();
			Contained c2 = new Contained();
			c.Bag = new ArrayList();
			c.Bag.Add(c1);
			c.Bag.Add(c2);
			c1.Bag.Add(c);
			c2.Bag.Add(c);
			s.Save(c);
			c.Bag.Add(c2);
			c2.Bag.Add(c);
			c.LazyBag.Add(c1);
			c1.LazyBag.Add(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Find("from c in class ContainerX")[0];
			Assert.AreEqual( 1, c.LazyBag.Count );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Find("from c in class ContainerX")[0];
			Contained c3 = new Contained();
			// commented out in h2.0.3 also
			//c.Bag.Add(c3);
			//c3.Bag.Add(c);
			c.LazyBag.Add(c3);
			c3.LazyBag.Add(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Find("from c in class ContainerX")[0];
			Contained c4 = new Contained();
			c.LazyBag.Add(c4);
			c4.LazyBag.Add(c);
			Assert.AreEqual( 3, c.LazyBag.Count ); //forces initialization
			// s.Save(c4); commented in h2.0.3 also
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Container)s.Find("from c in class ContainerX")[0];
			int j = 0;
			foreach(object obj in c.Bag) 
			{
				Assert.IsNotNull(obj);
				j++;
			}

			Assert.AreEqual( 3, j);
			Assert.AreEqual( 3, c.LazyBag.Count );
			s.Delete(c);
			c.Bag.Remove(c2);

			j = 0;
			foreach(object obj in c.Bag) 
			{
				j++;
				s.Delete(obj);
			}

			Assert.AreEqual( 2, j );
			s.Delete( s.Load( typeof(Contained), c4.Id ) );
			s.Delete( s.Load( typeof(Contained), c3.Id ) );
			t.Commit();
			s.Close();

		}

		[Test]
		public void CircularCascade() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Circular c = new Circular();
			c.Clazz = typeof(Circular);
			c.Other = new Circular();
			c.Other.Other = new Circular();
			c.Other.Other.Other = c;
			c.AnyEntity = c.Other;
			string id = (string)s.Save(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Circular)s.Load( typeof(Circular), id );
			c.Other.Other.Clazz = typeof(Foo);
			t.Commit();
			s.Close();

			c.Other.Clazz = typeof(Qux);
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.SaveOrUpdate(c);
			t.Commit();
			s.Close();

			c.Other.Other.Clazz = typeof(Bar);
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.SaveOrUpdate(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Circular)s.Load( typeof(Circular), id );
			Assert.AreEqual( typeof(Bar), c.Other.Other.Clazz );
			Assert.AreEqual( typeof(Qux), c.Other.Clazz );
			Assert.AreEqual( c, c.Other.Other.Other );
			Assert.AreEqual( c.Other , c.AnyEntity );
			Assert.AreEqual( 3, s.Delete("from o in class Universe") );
			t.Commit();
			s.Close();

		}

		[Test]
		public void DeleteEmpty() 
		{
			ISession s = sessions.OpenSession();
			Assert.AreEqual( 0, s.Delete("from s in class Simple") );
			Assert.AreEqual( 0, s.Delete("from o in class Universe") );
			s.Close();
		}

		[Test]
		public void Locking() 
		{
			ISession s = sessions.OpenSession();
			ITransaction tx = s.BeginTransaction();
			Simple s1 = new Simple();
			s1.Count = 1;
			Simple s2 = new Simple();
			s2.Count = 2;
			Simple s3 = new Simple();
			s3.Count = 3;
			Simple s4 = new Simple();
			s4.Count = 4;

			s.Save(s1, (long)1);
			s.Save(s2, (long)2);
			s.Save(s3, (long)3);
			s.Save(s4, (long)4);
			Assert.AreEqual( LockMode.Write, s.GetCurrentLockMode(s1) );
			tx.Commit();
			s.Close();

			s = sessions.OpenSession();
			tx = s.BeginTransaction();
			s1 = (Simple)s.Load( typeof(Simple), (long)1, LockMode.None );
			Assert.AreEqual( LockMode.Read, s.GetCurrentLockMode(s1) );
			s2 = (Simple)s.Load( typeof(Simple), (long)2, LockMode.Read );
			Assert.AreEqual( LockMode.Read, s.GetCurrentLockMode(s2) );
			s3 = (Simple)s.Load( typeof(Simple), (long)3, LockMode.Upgrade );
			Assert.AreEqual( LockMode.Upgrade, s.GetCurrentLockMode(s3) );
			s4 = (Simple)s.Load( typeof(Simple), (long)4, LockMode.UpgradeNoWait );
			Assert.AreEqual( LockMode.UpgradeNoWait, s.GetCurrentLockMode(s4) );

			s1 = (Simple)s.Load( typeof(Simple), (long)1, LockMode.Upgrade ); //upgrade
			Assert.AreEqual( LockMode.Upgrade, s.GetCurrentLockMode(s1) );
			s2 = (Simple)s.Load( typeof(Simple), (long)2, LockMode.None );
			Assert.AreEqual( LockMode.Read, s.GetCurrentLockMode(s2) );
			s3 = (Simple)s.Load( typeof(Simple), (long)3, LockMode.Read );
			Assert.AreEqual( LockMode.Upgrade, s.GetCurrentLockMode(s3) );
			s4 = (Simple)s.Load( typeof(Simple), (long)4, LockMode.Upgrade );
			Assert.AreEqual( LockMode.UpgradeNoWait, s.GetCurrentLockMode(s4) );

			s.Lock( s2, LockMode.Upgrade ); //upgrade
			Assert.AreEqual( LockMode.Upgrade, s.GetCurrentLockMode(s2) );
			s.Lock( s3, LockMode.Upgrade );
			Assert.AreEqual( LockMode.Upgrade , s.GetCurrentLockMode(s3) );
			s.Lock( s1, LockMode.UpgradeNoWait );
			s.Lock( s4, LockMode.None );
			Assert.AreEqual( LockMode.UpgradeNoWait, s.GetCurrentLockMode(s4) );

			tx.Commit();
			tx = s.BeginTransaction();

			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s3) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s1) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s2) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s4) );

			s.Lock( s1, LockMode.Read ); //upgrade
			Assert.AreEqual( LockMode.Read, s.GetCurrentLockMode(s1) );
			s.Lock( s2, LockMode.Upgrade ); //upgrade
			Assert.AreEqual( LockMode.Upgrade, s.GetCurrentLockMode(s2) );
			s.Lock( s3, LockMode.UpgradeNoWait ); //upgrade
			Assert.AreEqual( LockMode.UpgradeNoWait , s.GetCurrentLockMode(s3) );
			s.Lock( s4, LockMode.None );
			Assert.AreEqual( LockMode.None , s.GetCurrentLockMode(s4) );

			s4.Name = "s4";
			s.Flush();
			Assert.AreEqual( LockMode.Write, s.GetCurrentLockMode(s4) );
			tx.Commit();

			tx = s.BeginTransaction();
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s3) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s1) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s2) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(s4) );

			s.Delete(s1);
			s.Delete(s2);
			s.Delete(s3);
			s.Delete(s4);
			tx.Commit();
			s.Close();
			

		}

		[Test]
		public void ObjectType() 
		{
			ISession s = sessions.OpenSession();
			Parent g = new Parent();
			Foo foo = new Foo();
			g.Any = foo;
			s.Save(g);
			s.Save(foo);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (Parent)s.Load( typeof(Parent), g.Id );
			Assert.IsNotNull( g.Any );
			Assert.IsTrue( g.Any is FooProxy );
			s.Delete( g.Any );
			s.Delete(g);
			s.Flush();
			s.Close();
		}


	}
}
