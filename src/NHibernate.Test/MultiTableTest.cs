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
		[Ignore("Problems generating correct sql: http://jira.nhibernate.org:8080/browse/NH-83")]
		public void TestJoins() 
		{
			// the commented out hql is what does not work because of 83
			ISession s = sessions.OpenSession();
			//s.Find("from SubMulti sm join sm.Children smc where smc.Name > 'a'");
			s.Find("select s, ya from LessSimple s join s.YetAnother ya");
			s.Find("from LessSimple s1 join s1.Bag s2");
			s.Find("from LessSimple s1 left join s1.Bag s2");
			s.Find("select s, a from LessSimple s join s.Another a");
			s.Find("select s, a from LessSimple s left join s.Another a");
			s.Find("from Simple s, LessSimple ls");
			s.Find("from LessSimple ls join ls.Set s where s.Name > 'a'");
			//s.Find("from Po po join po.List sm where sm.Name > 'a'");
			s.Find("from LessSimple ls inner join ls.Another s where s.Name is not null");
			s.Find("from LessSimple ls where ls.Other.Another.Name is not null");
			s.Close();
		}

		[Test]
		[Ignore("Problems generating correct sql: http://jira.nhibernate.org:8080/browse/NH-83")]
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
			sm1.Parent = sm;
			sm2.Parent = sm;
			object id = s.Save(sm);
			s.Save(sm1);
			s.Save(sm2);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			// TODO: I don't understand why h2.0.3 issues a select statement here
			
			// Invalid column "Name" - 83
			//Assert.AreEqual( 2, s.Find("select s from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null").Count );
			Assert.AreEqual( 2, s.Find("select elements(sm.Children) from SubMulti as sm").Count );
			// Invalid column "Name" - 83
			//Assert.AreEqual( 1, s.Find("select distinct sm from SubMulti as sm join sm.Children as s where s.Amount>-1 and s.Name is null").Count );
			sm = (SubMulti)s.Load( typeof(SubMulti), id );
			Assert.AreEqual( 2, sm.Children.Count );

			//TODO: code for a Filter here

			IEnumerator enumer = s.Enumerable("select distinct s from s in class SubMulti where s.MoreChildren[1].Amount < 1.0").GetEnumerator();
			// TODO: no rows returned here...
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreSame( sm, enumer.MoveNext() );
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
		[Ignore("Problems generating correct sql: http://jira.nhibernate.org:8080/browse/NH-83")]
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
			// bug 83
			//s.Find("from s in class LessSimple where s.YetAnother.Name='name'");
			//s.Find("from s in class LessSimple where s.YetAnother.Name='name' and s.YetAnother.Foo is null");
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
