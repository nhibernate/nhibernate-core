using System;
using System.Collections;

using NHibernate.DomainModel;

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
		public void TestJoins() 
		{
			ISession s = sessions.OpenSession();
			s.Find("from SubMulti sm join sm.Children smc where smc.Name > 'a'");
			s.Find("select s, ya from LessSimple s join s.YetAnother ya");
			s.Find("from LessSimple s1 join s1.Bag s2");
			s.Find("from LessSimple s1 left join s1.Bag s2");
			s.Find("select s, a from LessSimple s join s.Another a");
			s.Find("select s, a from LessSimple s left join s.Another a");
			s.Find("from Simple s, LessSimple ls");
			s.Find("from LessSimple ls join ls.Set s where s.Name > 'a'");
			s.Find("from Po po join po.List sm where sm.Name > 'a'");
			s.Find("from LessSimple ls inner join ls.Another s where s.Name is not null");
			s.Find("from LessSimple ls where ls.Other.Another.Name is not null");
			s.Close();
		}

		[Test]
		[Ignore("Filter Not Working http://jira.nhibernate.org:8080/browse/NH-80")]
		public void SubclassCollection() 
		{
			ISession s = sessions.OpenSession();
			SubMulti sm = new SubMulti();
			SubMulti sm1 = new SubMulti();
			SubMulti sm2 = new SubMulti();
			IList list = new ArrayList();
			IList anotherList = new ArrayList();
			sm.Children = list;
			sm.MoreChildren = anotherList;
			list.Add(sm1);
			list.Add(sm2);
			anotherList.Add(sm1);
			anotherList.Add(sm2);
			sm1.Parent = sm;
			sm2.Parent = sm;
			object id = s.Save(sm);
			s.Save(sm1);
			s.Save(sm2);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			// TODO: I don't understand why h2.0.3 issues a select statement here
			
			Assert.AreEqual( 2, s.Find("select s from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null").Count );
			Assert.AreEqual( 2, s.Find("select elements(sm.Children) from SubMulti as sm").Count );
			Assert.AreEqual( 1, s.Find("select distinct sm from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null").Count );
			sm = (SubMulti)s.Load( typeof(SubMulti), id );
			Assert.AreEqual( 2, sm.Children.Count );

			//TODO: code for a Filter here
			
			IEnumerator enumer = s.Enumerable("select distinct s from s in class SubMulti where s.MoreChildren[1].Amount < 1.0").GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreSame( sm, enumer.Current );
			Assert.AreEqual( 2, sm.MoreChildren.Count );
			s.Delete(sm);

			foreach(object obj in sm.Children) 
			{
				s.Delete(obj);
			}
			s.Flush();
			s.Close();


		}

		[Test]
		public void CollectionOnly() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Mono m = new Mono();
			long id = (long)s.Save(m);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Update(m, id);
			s.Flush();
			m.Address = "foo bar";
			s.Flush();
			s.Delete(m);
			t.Commit();
			s.Close();
		}

		[Test]
		public void Queries() 
		{
			ISession s = sessions.OpenSession();
			long id = 1L;

			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				id = (long)s.Save( new TrivialClass() );
			}
			else 
			{
				s.Save( new TrivialClass(), id );
			}

			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			TrivialClass tc = (TrivialClass)s.Load( typeof(TrivialClass), id );
			s.Find("from s in class TrivialClass where s.id = 2");
			s.Find("select s.Count from s in class Simple");
			s.Find("from s in class LessSimple where s.Another.Name='name'");
			s.Find("from s in class LessSimple where s.YetAnother.Name='name'");
			s.Find("from s in class LessSimple where s.YetAnother.Name='name' and s.YetAnother.Foo is null");
			s.Find("from s in class Simple where s.Count=1");
			s.Find("select s.Count from s in class Simple, ls in class LessSimple where ls.Another=s");
			s.Find("select ls.Bag.elements, ls.Set.elements from ls in class LessSimple");
			s.Enumerable("from s in class LessSimple");
			s.Enumerable("from s in class Simple");
			s.Delete(tc);
			s.Flush();
			s.Close();

		}

		[Test]
		public void Constraints() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				s.Save(sm);
			}
			else 
			{
				s.Save(sm, (long)2);
			}
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			s.Delete( "from sm in class SubMulti" );
			t = s.BeginTransaction();
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTable() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Simple simp = new Simple();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;
			object sid;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				mid = s.Save(multi);
				sid = s.Save(simp);
			}
			else 
			{
				mid = 123L;
				sid = 1234L;
				s.Save(multi, mid);
				s.Save(simp, sid);
			}
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			object smid;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				smid = s.Save(sm);
			}
			else 
			{
				smid = 2L;
				s.Save(sm, smid);
			}
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi.ExtraProp =  multi.ExtraProp + "2";
			multi.Name = "new name";
			s.Update( multi, mid );
			simp.Name = "new name";
			s.Update( simp, sid );
			sm.Amount = 456.7f;
			s.Update( sm, smid );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Multi), mid );
			Assert.AreEqual( "extra2", multi.ExtraProp );
			multi.ExtraProp = multi.ExtraProp + "3";
			Assert.AreEqual( "new name", multi.Name );
			multi.Name = "newer name";
			sm = (SubMulti) s.Load( typeof(SubMulti), smid );
			Assert.AreEqual( 456.7f, sm.Amount );
			sm.Amount = 23423f;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Simple), mid );
			simp = (Simple)s.Load( typeof(Simple), sid );
			Assert.IsFalse( simp is Multi );
			Assert.IsTrue( multi is Multi );
			Assert.AreEqual( "extra23", multi.ExtraProp );
			Assert.AreEqual( "newer name", multi.Name );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumer = s.Enumerable("select\n\ns from s in class Simple where s.Count>0").GetEnumerator();
			bool foundSimp = false;
			bool foundMulti = false;
			bool foundSubMulti = false;
			while( enumer.MoveNext() ) 
			{
				object o = enumer.Current;
				if( (o is Simple) && !(o is Multi) ) foundSimp = true;
				if( (o is Multi) && !(o is SubMulti) ) foundMulti = true;
				if( o is SubMulti ) foundSubMulti = true;
			}
			Assert.IsTrue( foundSimp );
			Assert.IsTrue( foundMulti );
			Assert.IsTrue( foundSubMulti );
			
			s.Find("from m in class Multi where m.Count>0 and m.ExtraProp is not null");
			s.Find("from m in class Simple where m.Count>0 and m.Name is not null");
			s.Find("from m in class LessSimple where m.Other is not null");
			s.Find("from m in class Multi where m.Other.id = 1");
			s.Find("from m in class SubMulti where m.Amount > 0.0");
			
			Assert.AreEqual( 2, s.Find("from m in class Multi").Count );

			//if( !(dialect is Dialect.HSQLDialect) ) 
			//{
			Assert.AreEqual( 1, s.Find("from m in class Multi where m.class = SubMulti").Count );
			Assert.AreEqual( 1, s.Find("from m in class Simple where m.class = Multi").Count );
			//}

			Assert.AreEqual( 3, s.Find("from s in class Simple").Count );
			Assert.AreEqual( 0, s.Find("from ls in class LessSimple").Count );
			Assert.AreEqual( 1, s.Find("from sm in class SubMulti").Count );

			s.Find("from ls in class LessSimple, s in ls.Bag.elements where s.id is not null");
			s.Find("from ls in class LessSimple, s in ls.Set.elements where s.id is not null");
			s.Find("from sm in class SubMulti where exists sm.Children.elements");
			
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Simple), mid, LockMode.Upgrade );
			simp = (Simple)s.Load( typeof(Simple), sid );
			s.Lock( simp, LockMode.UpgradeNoWait );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Update( multi, mid );
			s.Delete(multi);
			Assert.AreEqual( 2, s.Delete("from s in class Simple") );
			t.Commit();
			s.Close();


		}

		[Test]
		public void MutliTableGeneratedId() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Simple simp = new Simple();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object multiId = s.Save(multi);
			object simpId = s.Save(simp);
			SubMulti sm = new SubMulti();
			sm.Amount = 66.5f;
			object smId = s.Save(sm);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi.ExtraProp += "2";
			multi.Name = "new name";
			s.Update( multi, multiId );
			simp.Name = "new name";
			s.Update( simp, simpId );
			sm.Amount = 456.7f;
			s.Update( sm, smId );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Multi), multiId );
			Assert.AreEqual( "extra2", multi.ExtraProp );
			multi.ExtraProp += "3";
			Assert.AreEqual( "new name", multi.Name );
			multi.Name = "newer name";
			sm = (SubMulti)s.Load( typeof(SubMulti), smId );
			Assert.AreEqual( 456.7f, sm.Amount );
			sm.Amount = 23423f;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Simple), multiId );
			simp = (Simple)s.Load( typeof(Simple), simpId );
			Assert.IsFalse( simp is Multi );
			Assert.IsTrue( multi is Multi );
			Assert.AreEqual( "extra23", multi.ExtraProp );
			Assert.AreEqual( "newer name", multi.Name );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IEnumerable enumer = s.Enumerable("select\n\ns from s in class Simple where s.Count>0");
			bool foundSimp = false;
			bool foundMulti = false;
			bool foundSubMulti = false;

			foreach(object obj in enumer) 
			{
				if( (obj is Simple) && !(obj is Multi) ) foundSimp = true;
				if( (obj is Multi) && !(obj is SubMulti) ) foundMulti = true;
				if( obj is SubMulti ) foundSubMulti = true;
			}
			Assert.IsTrue(foundSimp);
			Assert.IsTrue(foundMulti);
			Assert.IsTrue(foundSubMulti);

			s.Find("from m in class Multi where m.Count>0 and m.ExtraProp is not null");
			s.Find("from m in class Simple where m.Count>0 and m.Name is not null");
			s.Find("from m in class LessSimple where m.Other is not null");
			s.Find("from m in class Multi where m.Other.id = 1");
			s.Find("from m in class SubMulti where m.Amount > 0.0");
			
			Assert.AreEqual( 2, s.Find("from m in class Multi").Count );
			Assert.AreEqual( 3, s.Find("from s in class Simple").Count );
			Assert.AreEqual( 0, s.Find("from s in class LessSimple").Count );
			Assert.AreEqual( 1, s.Find("from sm in class SubMulti").Count );

			s.Find("from ls in class LessSimple, s in ls.Bag.elements where s.id is not null");
			s.Find("from sm in class SubMulti where exists sm.Children.elements");
			
			t.Commit();
			s.Close();
			
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			multi = (Multi)s.Load( typeof(Simple), multiId, LockMode.Upgrade );
			simp = (Simple)s.Load( typeof(Simple), simpId );
			s.Lock( simp, LockMode.UpgradeNoWait );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Update( multi, multiId );
			s.Delete(multi);
			Assert.AreEqual( 2, s.Delete("from s in class Simple") );
			t.Commit();
			s.Close();
		}

		[Test]
		public void MultiTableCollections() 
		{
			//if( dialect is Dialect.HSQLDialect) return;

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Assert.AreEqual( 0, s.Find("from s in class Simple").Count );
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Simple simp = new Simple();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;
			object sid;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				mid = s.Save(multi);
				sid = s.Save(simp);
			}
			else 
			{
				mid = 123L;
				sid = 1234L;
				s.Save(multi, mid);
				s.Save(simp, sid);
			}
			LessSimple ls = new LessSimple();
			ls.Other = ls;
			ls.Another = ls;
			ls.YetAnother = ls;
			ls.Name = "Less Simple";
			IDictionary dict = new Hashtable();
			ls.Set = dict;
			dict.Add( multi, new object() );
			dict.Add( simp, new object() );
			object id;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				id = s.Save(ls);
			}
			else 
			{
				id = 2L;
				s.Save(ls, id);
			}
			t.Commit();
			s.Close();
			Assert.AreSame( ls, ls.Other );
			Assert.AreSame( ls, ls.Another );
			Assert.AreSame( ls, ls.YetAnother );

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			ls = (LessSimple)s.Load( typeof(LessSimple), id );
			Assert.AreSame( ls, ls.Other );
			Assert.AreSame( ls, ls.Another );
			Assert.AreSame( ls, ls.YetAnother );
			Assert.AreEqual( 2, ls.Set.Count );

			int foundMulti = 0;
			int foundSimple = 0;

			foreach(object obj in ls.Set.Keys) 
			{
				if( obj is Simple ) foundSimple++;
				if( obj is Multi ) foundMulti++;
			}
			Assert.AreEqual( 2, foundSimple );
			Assert.AreEqual( 1, foundMulti );
			Assert.AreEqual( 3, s.Delete("from s in class Simple") );
			t.Commit();
			s.Close();


		}

		[Test]
		public void MultiTableManyToOne() 
		{
			//if( dialect is Dialect.HSQLDialect) return;

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Assert.AreEqual( 0, s.Find("from s in class Simple").Count );
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			multi.Name = "name";
			Simple simp = new Simple();
			simp.Date = DateTime.Now;
			simp.Name = "simp";
			object mid;

			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				mid = s.Save(multi);
			}
			else 
			{
				mid = 123L;
				s.Save(multi, mid);
			}
			LessSimple ls = new LessSimple();
			ls.Other = ls;
			ls.Another = multi;
			ls.YetAnother = ls;
			ls.Name = "Less Simple";
			object id;
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				id = s.Save(ls);
			}
			else 
			{
				id = 2L;
				s.Save(ls, id);
			}
			t.Commit();
			s.Close();

			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(multi, ls.Another);
			Assert.AreSame(ls, ls.YetAnother);

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			ls = (LessSimple)s.Load( typeof(LessSimple), id );
			Assert.AreSame(ls, ls.Other);
			Assert.AreSame(ls, ls.YetAnother);
			Assert.AreEqual("name", ls.Another.Name);
			Assert.IsTrue(ls.Another is Multi);
			s.Delete(ls);
			s.Delete(ls.Another);
			t.Commit();
			s.Close();
									   
		}

		[Test]
		public void MultiTableNativeId() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi = new Multi();
			multi.ExtraProp = "extra";
			object id = s.Save(multi);
			Assert.IsNotNull(id);
			s.Delete(multi);
			t.Commit();
			s.Close();
		}
		
		[Test]
		public void Collection() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Multi multi1 = new Multi();
			multi1.ExtraProp = "extra1";
			Multi multi2 = new Multi();
			multi2.ExtraProp = "extra2";
			Po po = new Po();
			multi1.Po = po;
			multi2.Po = po;
			po.Set = new Hashtable();
			po.Set.Add( multi1, new object() );
			po.Set.Add( multi2, new object() );
			po.List = new ArrayList();
			po.List.Add( new SubMulti() );
			object id = s.Save(po);
			Assert.IsNotNull(id);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			po = (Po)s.Load( typeof(Po), id );
			Assert.AreEqual( 2, po.Set.Count );
			Assert.AreEqual( 1, po.List.Count );
			s.Delete(po);
			Assert.AreEqual( 0, s.Find("from s in class Simple").Count );
			t.Commit();
			s.Close();

		}

		[Test]
		public void OneToOne() 
		{
			ISession s = sessions.OpenSession();
			LessSimple ls = new LessSimple();
			object id = s.Save(ls);
			s.Flush();
			s.Close();
			
			s = sessions.OpenSession();
			s.Load( typeof(LessSimple), id );
			s.Close();

			s = sessions.OpenSession();
			s.Delete( s.Load( typeof(LessSimple), id ) );
			s.Flush();
			s.Close();
		}

		[Test]
		public void CollectionPointer() 
		{
			ISession s = sessions.OpenSession();
			LessSimple ls = new LessSimple();
			IList list = new ArrayList();
			ls.Bag = list;
			Simple simple = new Simple();
			object id = s.Save(ls);
			s.Save(simple);
			s.Flush();
			list.Add(simple);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			ls = (LessSimple)s.Load( typeof(LessSimple), id );
			Assert.AreEqual( 1, ls.Bag.Count );
			s.Delete("from o in class System.Object");
			s.Flush();
			s.Close();
		}

	}
}
