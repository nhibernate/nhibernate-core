using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NUnit.Framework;
using Single=NHibernate.DomainModel.Single;

namespace NHibernate.Test.Legacy
{
	/// <summary>
	/// Summary description for SQLLoaderTest.
	/// </summary>
	[TestFixture]
	public class SQLLoaderTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"ABC.hbm.xml",
						"Category.hbm.xml",
						"Simple.hbm.xml",
						"Fo.hbm.xml",
						"SingleSeveral.hbm.xml",
						"Componentizable.hbm.xml"
					};
			}
		}

		private static long nextLong = 1;

		[Test]
		public void TS()
		{
			if (Dialect is Oracle8iDialect)
			{
				return;
			}

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			Simple sim = new Simple();
			sim.Date = DateTime.Today; // NB We don't use Now() due to the millisecond alignment problem with SQL Server
			session.Save(sim, 1L);
			IQuery q = session.CreateSQLQuery("select {sim.*} from Simple {sim} where {sim}.date_ = ?")
				.AddEntity("sim", typeof(Simple));
			q.SetTimestamp(0, sim.Date);
			Assert.AreEqual(1, q.List().Count, "q.List.Count");
			session.Delete(sim);
			txn.Commit();
			session.Close();
		}

		[Test]
		public void TSNamed()
		{
			if (Dialect is Oracle8iDialect)
			{
				return;
			}

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			Simple sim = new Simple();
			sim.Date = DateTime.Today; // NB We don't use Now() due to the millisecond alignment problem with SQL Server
			session.Save(sim, 1L);
			IQuery q =
				session.CreateSQLQuery("select {sim.*} from Simple {sim} where {sim}.date_ = :fred")
				.AddEntity("sim", typeof(Simple));
			q.SetTimestamp("fred", sim.Date);
			Assert.AreEqual(1, q.List().Count, "q.List.Count");
			session.Delete(sim);
			txn.Commit();
			session.Close();
		}

		[Test]
		public void FindBySQLStar()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			session.Save(s);

			Simple simple = new Simple();
			simple.Init();
			session.Save(simple, nextLong++);

			A a = new A();
			session.Save(a);

			//B b = new B();
			//session.Save( b );

			session.CreateSQLQuery("select {category.*} from Category {category}")
				.AddEntity("category", typeof(Category)).List();
			session.CreateSQLQuery("select {simple.*} from Simple {simple}")
				.AddEntity("simple", typeof(Simple)).List();
			session.CreateSQLQuery("select {a.*} from A {a}")
				.AddEntity("a", typeof(A)).List();

			session.Delete(s);
			session.Delete(simple);
			session.Delete(a);
			//session.Delete( b );
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLProperties()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			session.Save(s);

			s = new Category();
			s.Name = "WannaBeFound";
			session.Flush();

			IQuery query =
				session.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = :Name")
				.AddEntity("category",typeof(Category));
			query.SetProperties(s);

			query.List();

			session.Delete("from Category");
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLAssociatedObject()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList list = s.CreateSQLQuery("select {category.*} from Category {category}")
				.AddEntity("category", typeof(Category)).List();
			Assert.AreEqual(1, list.Count, "Count differs");

			s.Delete("from Assignable");
			s.Delete("from Category");
			s.Flush();
			s.Close();
		}

		[Test]
		public void FindBySQLMultipleObject()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();

			c = new Category();
			c.Name = "NAME2";
			assn = new Assignable();
			assn.Id = "i.d.2";
			assn.Categories = new List<Category> { c };
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();

			assn = new Assignable();
			assn.Id = "i.d.3";
			s.Save(assn);
			s.Flush();
			s.Close();

			s = OpenSession();

			if (!(Dialect is MySQLDialect))
			{
				IList list =
					s.CreateSQLQuery("select {category.*}, {assignable.*} from Category {category}, \"assign able\" {assignable}")
					.AddEntity("category",   typeof(Category))
					.AddEntity("assignable", typeof(Assignable))
					.List();
				Assert.AreEqual(6, list.Count, "Count differs"); // cross-product of 2 categories x 3 assignables;
				Assert.IsTrue(list[0] is object[]);
			}

			s.Delete("from Assignable");
			s.Delete("from Category");
			s.Flush();
			s.Close();
		}

		[Test]
		public void FindBySQLParameters()
		{
			ISession s = OpenSession();
			Category c = new Category();
			c.Name = "Good";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> { c };
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			c = new Category();
			c.Name = "Best";
			assn = new Assignable();
			assn.Id = "i.d.2";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			c = new Category();
			c.Name = "Better";
			assn = new Assignable();
			assn.Id = "i.d.7";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();

			assn = new Assignable();
			assn.Id = "i.d.3";
			s.Save(assn);
			s.Flush();
			s.Close();

			s = OpenSession();
			IQuery basicParam =
				s.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = 'Best'")
				.AddEntity("category", typeof(Category));
			IList list = basicParam.List();
			Assert.AreEqual(1, list.Count);

			IQuery unnamedParam =
				s.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = ? or {category}.Name = ?")
				.AddEntity("category", typeof(Category));
			unnamedParam.SetString(0, "Good");
			unnamedParam.SetString(1, "Best");
			list = unnamedParam.List();
			Assert.AreEqual(2, list.Count);

			IQuery namedParam =
				s.CreateSQLQuery(
					"select {category.*} from Category {category} where ({category}.Name=:firstCat or {category}.Name=:secondCat)")
					.AddEntity("category", typeof(Category));
			namedParam.SetString("firstCat", "Better");
			namedParam.SetString("secondCat", "Best");
			list = namedParam.List();
			Assert.AreEqual(2, list.Count);

			s.Delete("from Assignable");
			s.Delete("from Category");
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Escaping not implemented. Need to test with ODBC/OLEDB when implemented.")]
		public void EscapedODBC()
		{
			if (Dialect is MySQLDialect || Dialect is PostgreSQLDialect) return;

			ISession session = OpenSession();

			A savedA = new A();
			savedA.Name = "Max";
			session.Save(savedA);

			B savedB = new B();
			session.Save(savedB);
			session.Flush();

			session.Close();

			session = OpenSession();

			IQuery query;

			query =
				session.CreateSQLQuery(
					"select identifier_column as {a.id}, clazz_discriminata as {a.class}, count_ as {a.Count}, name as {a.Name} from A where {fn ucase(Name)} like {fn ucase('max')}")
					.AddEntity("a", typeof(A));

			// NH: Replaced the whole if by the line above
			/*
			if( dialect is Dialect.TimesTenDialect) 
			{
				// TimesTen does not permit general expressions (like UPPER) in the second part of a LIKE expression,
				// so we execute a similar test 
				query = session.CreateSQLQuery("select identifier_column as {a.id}, clazz_discriminata as {a.class}, count_ as {a.Count}, name as {a.Name} from A where {fn ucase(name)} like 'MAX'", "a", typeof( A ));
			}
			else 
			{
				query = session.CreateSQLQuery("select identifier_column as {a.id}, clazz_discriminata as {a.class}, count_ as {a.Count}, name as {a.Name} from A where {fn ucase(name)} like {fn ucase('max')}", "a", typeof( A ));
			}
			*/

			IList list = query.List();

			Assert.IsNotNull(list);
			Assert.AreEqual(1, list.Count);

			session.Delete("from A");
			session.Flush();
			session.Close();
		}

		[Test]
		public void DoubleAliasing()
		{
			if (Dialect is MySQLDialect) return;
			if (Dialect is FirebirdDialect) return; // See comment below

			ISession session = OpenSession();

			A savedA = new A();
			savedA.Name = "Max";
			session.Save(savedA);

			B savedB = new B();
			session.Save(savedB);
			session.Flush();

			int count = session.CreateQuery("from A").List().Count;
			session.Close();

			session = OpenSession();

			IQuery query =
				session.CreateSQLQuery(
					"select a.identifier_column as {a1.id}, a.clazz_discriminata as {a1.class}, a.count_ as {a1.Count}, a.name as {a1.Name}, a.anothername as {a1.AnotherName} " +
					", b.identifier_column as {a2.id}, b.clazz_discriminata as {a2.class}, b.count_ as {a2.Count}, b.name as {a2.Name}, b.anothername as {a2.AnotherName} " +
					" from A a, A b" +
					" where a.identifier_column = b.identifier_column")
					.AddEntity("a1", typeof(A))
					.AddEntity("a2",typeof(A));
			IList list = query.List();

			Assert.IsNotNull(list);

			Assert.AreEqual(2, list.Count);
			// On Firebird the list has 4 elements, I don't understand why.

			session.Delete("from A");
			session.Flush();
			session.Close();
		}

		[Test]
		public void EmbeddedCompositeProperties()
		{
			ISession session = OpenSession();

			Single s = new Single();
			s.Id = "my id";
			s.String = "string 1";
			session.Save(s);
			session.Flush();
			session.Clear();

			IQuery query = session.CreateSQLQuery("select {sing.*} from Single {sing}")
				.AddEntity("sing", typeof(Single));

			IList list = query.List();

			Assert.IsTrue(list.Count == 1);

			session.Clear();

			query = session.CreateSQLQuery("select {sing.*} from Single {sing} where sing.Id = ?")
				.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = query.List();

			Assert.IsTrue(list.Count == 1);

			session.Clear();

			query =
				session.CreateSQLQuery(
					"select s.id as {sing.Id}, s.string_ as {sing.String}, s.prop as {sing.Prop} from Single s where s.Id = ?")
					.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = query.List();

			Assert.IsTrue(list.Count == 1);


			session.Clear();

			query =
				session.CreateSQLQuery(
					"select s.id as {sing.Id}, s.string_ as {sing.String}, s.prop as {sing.Prop} from Single s where s.Id = ?")
					.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = query.List();

			Assert.IsTrue(list.Count == 1);

			session.Delete(list[0]);
			session.Flush();
			session.Close();
		}

		[Test]
		public void ComponentStar()
		{
			ComponentTest("select {comp.*} from Componentizable comp");
		}

		[Test]
		public void ComponentNoStar()
		{
			ComponentTest(
				"select comp.Id as {comp.id}, comp.nickName as {comp.NickName}, comp.Name as {comp.Component.Name}, comp.SubName as {comp.Component.SubComponent.SubName}, comp.SubName1 as {comp.Component.SubComponent.SubName1} from Componentizable comp");
		}

		private void ComponentTest(string sql)
		{
			ISession session = OpenSession();

			Componentizable c = new Componentizable();
			c.NickName = "Flacky";
			NHibernate.DomainModel.Component component = new NHibernate.DomainModel.Component();
			component.Name = "flakky comp";
			SubComponent subComponent = new SubComponent();
			subComponent.SubName = "subway";
			component.SubComponent = subComponent;

			c.Component = component;

			session.Save(c);

			session.Flush();

			session.Clear();

			IQuery q = session.CreateSQLQuery(sql).AddEntity("comp", typeof(Componentizable));
			IList list = q.List();

			Assert.AreEqual(list.Count, 1);

			Componentizable co = (Componentizable) list[0];

			Assert.AreEqual(c.NickName, co.NickName);
			Assert.AreEqual(c.Component.Name, co.Component.Name);
			Assert.AreEqual(c.Component.SubComponent.SubName, co.Component.SubComponent.SubName);

			session.Delete(co);
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindSimpleBySQL()
		{
			if (Dialect is MySQLDialect) return;
			ISession session = OpenSession();
			Category s = new Category();

			nextLong++;
			s.Name = nextLong.ToString();
			session.Save(s);
			session.Flush();

			IQuery query =
				session.CreateSQLQuery(
					"select s.category_key_col as {category.id}, s.Name as {category.Name}, s.\"assign able id\" as {category.Assignable} from {category} s")
					.AddEntity("category", typeof(Category));
			IList list = query.List();

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
			Assert.IsTrue(list[0] is Category);
			session.Delete(list[0]);
			session.Flush();
			session.Close();
			// How do we handle objects with composite id's ? (such as Single)
		}

		[Test]
		public void FindBySQLSimpleByDiffSessions()
		{
			if (Dialect is MySQLDialect) return;

			ISession session = OpenSession();
			Category s = new Category();
			nextLong++;
			s.Name = nextLong.ToString();
			session.Save(s);
			session.Flush();
			session.Close();

			session = OpenSession();

			IQuery query =
				session.CreateSQLQuery(
					"select s.category_key_col as {category.id}, s.Name as {category.Name}, s.\"assign able id\" as {category.Assignable} from {category} s")
					.AddEntity("category", typeof(Category));
			IList list = query.List();

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
			Assert.IsTrue(list[0] is Category);

			// How do we handle objects that does not have id property (such as Simple ?)
			// How do we handle objects with composite id's ? (such as Single)
			session.Delete(list[0]);
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLDiscriminatorSameSession()
		{
			if (Dialect is MySQLDialect) return;

			ISession session = OpenSession();
			A savedA = new A();
			session.Save(savedA);

			B savedB = new B();
			session.Save(savedB);
			session.Flush();

			IQuery query =
				session.CreateSQLQuery(
					"select identifier_column as {a.id}, clazz_discriminata as {a.class}, name as {a.Name}, count_ as {a.Count} from {a} s")
					.AddEntity("a", typeof(A));
			IList list = query.List();

			Assert.IsNotNull(list);
			Assert.AreEqual(2, list.Count);

			A a1 = (A) list[0];
			A a2 = (A) list[1];

			Assert.IsTrue((a2 is B) || (a1 is B));
			Assert.IsFalse(a1 is B && a2 is B);

			if (a1 is B)
			{
				Assert.AreSame(a1, savedB);
				Assert.AreSame(a2, savedA);
			}
			else
			{
				Assert.AreSame(a2, savedB);
				Assert.AreSame(a1, savedA);
			}

			session.Delete("from A");
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLDiscriminatedDiffSessions()
		{
			if (Dialect is MySQLDialect) return;

			ISession session = OpenSession();
			A savedA = new A();
			session.Save(savedA);

			B savedB = new B();
			session.Save(savedB);
			session.Flush();

			int count = session.CreateQuery("from A").List().Count;
			session.Close();

			session = OpenSession();

			IQuery query = session.CreateSQLQuery(
				"select identifier_column as {a.id}, clazz_discriminata as {a.class}, count_ as {a.Count}, name as {a.Name}, anothername as {a.AnotherName} from A")
				.AddEntity("a", typeof(A));
			IList list = query.List();

			Assert.IsNotNull(list);
			Assert.AreEqual(count, list.Count);

			session.Delete("from A");
			session.Flush();
			session.Close();
		}

		[Test]
		public void NamedSQLQuery()
		{
			if (Dialect is MySQLDialect)
			{
				return;
			}

			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			s.Close();

			s = OpenSession();
			IQuery q = s.GetNamedQuery("namedsql");
			Assert.IsNotNull(q, "should have found 'namedsql'");
			IList list = q.List();
			Assert.IsNotNull(list, "executing query returns list");

			object[] values = list[0] as object[];
			Assert.IsNotNull(values[0], "index 0 should not be null");
			Assert.IsNotNull(values[1], "index 1 should not be null");

			Assert.AreEqual(typeof(Category), values[0].GetType(), "should be a Category");
			Assert.AreEqual(typeof(Assignable), values[1].GetType(), "should be Assignable");
			s.Delete("from Category");
			s.Delete("from Assignable");
			s.Flush();
			s.Close();
		}
	}
}