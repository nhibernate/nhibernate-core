﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NUnit.Framework;
using Single=NHibernate.DomainModel.Single;

namespace NHibernate.Test.Legacy
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <summary>
	/// Summary description for SQLLoaderTest.
	/// </summary>
	[TestFixture]
	public class SQLLoaderTestAsync : TestCase
	{
		protected override string[] Mappings
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
		public async Task TSAsync()
		{
			if (Dialect is Oracle8iDialect)
			{
				return;
			}

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			Simple sim = new Simple();
			sim.Date = DateTime.Today; // NB We don't use Now() due to the millisecond alignment problem with SQL Server
			await (session.SaveAsync(sim, 1L));
			IQuery q = session.CreateSQLQuery("select {sim.*} from Simple {sim} where {sim}.date_ = ?")
				.AddEntity("sim", typeof(Simple));
			q.SetDateTime(0, sim.Date);
			Assert.AreEqual(1, (await (q.ListAsync())).Count, "q.List.Count");
			await (session.DeleteAsync(sim));
			await (txn.CommitAsync());
			session.Close();
		}

		[Test]
		public async Task TSNamedAsync()
		{
			if (Dialect is Oracle8iDialect)
			{
				return;
			}

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			Simple sim = new Simple();
			sim.Date = DateTime.Today; // NB We don't use Now() due to the millisecond alignment problem with SQL Server
			await (session.SaveAsync(sim, 1L));
			IQuery q =
				session.CreateSQLQuery("select {sim.*} from Simple {sim} where {sim}.date_ = :fred")
				.AddEntity("sim", typeof(Simple));
			q.SetDateTime("fred", sim.Date);
			Assert.AreEqual(1, (await (q.ListAsync())).Count, "q.List.Count");
			await (session.DeleteAsync(sim));
			await (txn.CommitAsync());
			session.Close();
		}

		[Test]
		public async Task FindBySQLStarAsync()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			await (session.SaveAsync(s));

			Simple simple = new Simple();
			simple.Init();
			await (session.SaveAsync(simple, nextLong++));

			A a = new A();
			await (session.SaveAsync(a));

			//B b = new B();
			//session.Save( b );

			await (session.CreateSQLQuery("select {category.*} from Category {category}")
				.AddEntity("category", typeof(Category)).ListAsync());
			await (session.CreateSQLQuery("select {simple.*} from Simple {simple}")
				.AddEntity("simple", typeof(Simple)).ListAsync());
			await (session.CreateSQLQuery("select {a.*} from A {a}")
				.AddEntity("a", typeof(A)).ListAsync());

			await (session.DeleteAsync(s));
			await (session.DeleteAsync(simple));
			await (session.DeleteAsync(a));
			//session.Delete( b );
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task FindBySQLPropertiesAsync()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			await (session.SaveAsync(s));

			s = new Category();
			s.Name = "WannaBeFound";
			await (session.FlushAsync());

			IQuery query =
				session.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = :Name")
				.AddEntity("category",typeof(Category));
			query.SetProperties(s);

			await (query.ListAsync());

			await (session.DeleteAsync("from Category"));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task FindBySQLDictionaryAsync()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var s = new Category { Name = nextLong.ToString() };
				nextLong++;
				await (session.SaveAsync(s));

				s = new Category { Name = "WannaBeFound" };
				await (session.FlushAsync());

				var query =
					session.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = :Name")
					       .AddEntity("category", typeof(Category));
				var parameters = new Dictionary<string, object>
				{
					{ nameof(s.Name), s.Name }
				};
				query.SetParameters(parameters);
				var results = await (query.ListAsync());
				Assert.That(results, Is.Empty);

				await (session.DeleteAsync("from Category"));
				await (tran.CommitAsync());
			}
		}

		[Test]
		public async Task FindBySQLDynamicAsync()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var s = new Category { Name = nextLong.ToString() };
				nextLong++;
				await (session.SaveAsync(s));

				s = new Category { Name = "WannaBeFound" };
				await (session.FlushAsync());

				var query =
					session.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = :Name")
					       .AddEntity("category", typeof(Category));
				dynamic parameters = new ExpandoObject();
				parameters.Name = s.Name;
				// Extension methods do not support dynamic, we must call it explicitly
				QueryExtensions.SetParameters(query, parameters);
				var results = await (query.ListAsync());
				Assert.That(results, Is.Empty);

				await (session.DeleteAsync("from Category"));
				await (tran.CommitAsync());
			}
		}

		[Test]
		public async Task FindBySQLAssociatedObjectAsync()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			s.Close();

			s = OpenSession();
			IList list = await (s.CreateSQLQuery("select {category.*} from Category {category}")
				.AddEntity("category", typeof(Category)).ListAsync());
			Assert.AreEqual(1, list.Count, "Count differs");

			await (s.DeleteAsync("from Assignable"));
			await (s.DeleteAsync("from Category"));
			await (s.FlushAsync());
			s.Close();
		}

		[Test]
		public async Task FindBySQLMultipleObjectAsync()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());

			c = new Category();
			c.Name = "NAME2";
			assn = new Assignable();
			assn.Id = "i.d.2";
			assn.Categories = new List<Category> { c };
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());

			assn = new Assignable();
			assn.Id = "i.d.3";
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			s.Close();

			s = OpenSession();

			if (!(Dialect is MySQLDialect))
			{
				IList list =
					await (s.CreateSQLQuery("select {category.*}, {assignable.*} from Category {category}, \"assign able\" {assignable}")
					.AddEntity("category",   typeof(Category))
					.AddEntity("assignable", typeof(Assignable))
					.ListAsync());
				Assert.AreEqual(6, list.Count, "Count differs"); // cross-product of 2 categories x 3 assignables;
				Assert.IsTrue(list[0] is object[]);
			}

			await (s.DeleteAsync("from Assignable"));
			await (s.DeleteAsync("from Category"));
			await (s.FlushAsync());
			s.Close();
		}

		[Test]
		public async Task FindBySQLParametersAsync()
		{
			ISession s = OpenSession();
			Category c = new Category();
			c.Name = "Good";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> { c };
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			c = new Category();
			c.Name = "Best";
			assn = new Assignable();
			assn.Id = "i.d.2";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			c = new Category();
			c.Name = "Better";
			assn = new Assignable();
			assn.Id = "i.d.7";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			await (s.SaveAsync(assn));
			await (s.FlushAsync());

			assn = new Assignable();
			assn.Id = "i.d.3";
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			s.Close();

			s = OpenSession();
			IQuery basicParam =
				s.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = 'Best'")
				.AddEntity("category", typeof(Category));
			IList list = await (basicParam.ListAsync());
			Assert.AreEqual(1, list.Count);

			IQuery unnamedParam =
				s.CreateSQLQuery("select {category.*} from Category {category} where {category}.Name = ? or {category}.Name = ?")
				.AddEntity("category", typeof(Category));
			unnamedParam.SetString(0, "Good");
			unnamedParam.SetString(1, "Best");
			list = await (unnamedParam.ListAsync());
			Assert.AreEqual(2, list.Count);

			IQuery namedParam =
				s.CreateSQLQuery(
					"select {category.*} from Category {category} where ({category}.Name=:firstCat or {category}.Name=:secondCat)")
					.AddEntity("category", typeof(Category));
			namedParam.SetString("firstCat", "Better");
			namedParam.SetString("secondCat", "Best");
			list = await (namedParam.ListAsync());
			Assert.AreEqual(2, list.Count);

			await (s.DeleteAsync("from Assignable"));
			await (s.DeleteAsync("from Category"));
			await (s.FlushAsync());
			s.Close();
		}

		[Test]
		public async Task DoubleAliasingAsync()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore("Dialect does not support scalar sub-select, used by Map formula in B (C1 and C2) mapping");

			ISession session = OpenSession();

			A savedA = new A();
			savedA.Name = "Max";
			await (session.SaveAsync(savedA));

			B savedB = new B();
			await (session.SaveAsync(savedB));
			await (session.FlushAsync());

			int count = (await (session.CreateQuery("from A").ListAsync())).Count;
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
			IList list = await (query.ListAsync());

			Assert.IsNotNull(list);

			Assert.AreEqual(2, list.Count);

			await (session.DeleteAsync("from A"));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task EmbeddedCompositePropertiesAsync()
		{
			ISession session = OpenSession();

			Single s = new Single();
			s.Id = "my id";
			s.String = "string 1";
			await (session.SaveAsync(s));
			await (session.FlushAsync());
			session.Clear();

			IQuery query = session.CreateSQLQuery("select {sing.*} from Single {sing}")
				.AddEntity("sing", typeof(Single));

			IList list = await (query.ListAsync());

			Assert.IsTrue(list.Count == 1);

			session.Clear();

			query = session.CreateSQLQuery("select {sing.*} from Single {sing} where sing.Id = ?")
				.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = await (query.ListAsync());

			Assert.IsTrue(list.Count == 1);

			session.Clear();

			query =
				session.CreateSQLQuery(
					"select s.id as {sing.Id}, s.string_ as {sing.String}, s.prop as {sing.Prop} from Single s where s.Id = ?")
					.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = await (query.ListAsync());

			Assert.IsTrue(list.Count == 1);

			session.Clear();

			query =
				session.CreateSQLQuery(
					"select s.id as {sing.Id}, s.string_ as {sing.String}, s.prop as {sing.Prop} from Single s where s.Id = ?")
					.AddEntity("sing", typeof(Single));
			query.SetString(0, "my id");
			list = await (query.ListAsync());

			Assert.IsTrue(list.Count == 1);

			await (session.DeleteAsync(list[0]));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task ComponentStarAsync()
		{
			await (ComponentTestAsync("select {comp.*} from Componentizable comp"));
		}

		[Test]
		public async Task ComponentNoStarAsync()
		{
			await (ComponentTestAsync(
				"select comp.Id as {comp.id}, comp.nickName as {comp.NickName}, comp.Name as {comp.Component.Name}, comp.SubName as {comp.Component.SubComponent.SubName}, comp.SubName1 as {comp.Component.SubComponent.SubName1} from Componentizable comp"));
		}

		private async Task ComponentTestAsync(string sql, CancellationToken cancellationToken = default(CancellationToken))
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

			await (session.SaveAsync(c, cancellationToken));

			await (session.FlushAsync(cancellationToken));

			session.Clear();

			IQuery q = session.CreateSQLQuery(sql).AddEntity("comp", typeof(Componentizable));
			IList list = await (q.ListAsync(cancellationToken));

			Assert.AreEqual(list.Count, 1);

			Componentizable co = (Componentizable) list[0];

			Assert.AreEqual(c.NickName, co.NickName);
			Assert.AreEqual(c.Component.Name, co.Component.Name);
			Assert.AreEqual(c.Component.SubComponent.SubName, co.Component.SubComponent.SubName);

			await (session.DeleteAsync(co, cancellationToken));
			await (session.FlushAsync(cancellationToken));
			session.Close();
		}

		[Test]
		public async Task FindSimpleBySQLAsync()
		{
			if (Dialect is MySQLDialect) return;
			ISession session = OpenSession();
			Category s = new Category();

			nextLong++;
			s.Name = nextLong.ToString();
			await (session.SaveAsync(s));
			await (session.FlushAsync());

			IQuery query =
				session.CreateSQLQuery(
					"select s.category_key_col as {category.id}, s.Name as {category.Name}, s.\"assign able id\" as {category.Assignable} from {category} s")
					.AddEntity("category", typeof(Category));
			IList list = await (query.ListAsync());

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
			Assert.IsTrue(list[0] is Category);
			await (session.DeleteAsync(list[0]));
			await (session.FlushAsync());
			session.Close();
			// How do we handle objects with composite id's ? (such as Single)
		}

		[Test]
		public async Task FindBySQLSimpleByDiffSessionsAsync()
		{
			if (Dialect is MySQLDialect) return;

			ISession session = OpenSession();
			Category s = new Category();
			nextLong++;
			s.Name = nextLong.ToString();
			await (session.SaveAsync(s));
			await (session.FlushAsync());
			session.Close();

			session = OpenSession();

			IQuery query =
				session.CreateSQLQuery(
					"select s.category_key_col as {category.id}, s.Name as {category.Name}, s.\"assign able id\" as {category.Assignable} from {category} s")
					.AddEntity("category", typeof(Category));
			IList list = await (query.ListAsync());

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
			Assert.IsTrue(list[0] is Category);

			// How do we handle objects that does not have id property (such as Simple ?)
			// How do we handle objects with composite id's ? (such as Single)
			await (session.DeleteAsync(list[0]));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task FindBySQLDiscriminatorSameSessionAsync()
		{
			if (Dialect is MySQLDialect) return;

			ISession session = OpenSession();
			A savedA = new A();
			await (session.SaveAsync(savedA));

			B savedB = new B();
			await (session.SaveAsync(savedB));
			await (session.FlushAsync());

			IQuery query =
				session.CreateSQLQuery(
					"select identifier_column as {a.id}, clazz_discriminata as {a.class}, name as {a.Name}, count_ as {a.Count} from {a} s")
					.AddEntity("a", typeof(A));
			IList list = await (query.ListAsync());

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

			await (session.DeleteAsync("from A"));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task FindBySQLDiscriminatedDiffSessionsAsync()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore("Dialect does not support scalar sub-select, used by Map formula in B (C1 and C2) mapping");

			ISession session = OpenSession();
			A savedA = new A();
			await (session.SaveAsync(savedA));

			B savedB = new B();
			await (session.SaveAsync(savedB));
			await (session.FlushAsync());

			int count = (await (session.CreateQuery("from A").ListAsync())).Count;
			session.Close();

			session = OpenSession();

			IQuery query = session.CreateSQLQuery(
				"select identifier_column as {a.id}, clazz_discriminata as {a.class}, count_ as {a.Count}, name as {a.Name}, anothername as {a.AnotherName} from A")
				.AddEntity("a", typeof(A));
			IList list = await (query.ListAsync());

			Assert.IsNotNull(list);
			Assert.AreEqual(count, list.Count);

			await (session.DeleteAsync("from A"));
			await (session.FlushAsync());
			session.Close();
		}

		[Test]
		public async Task NamedSQLQueryAsync()
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
			await (s.SaveAsync(assn));
			await (s.FlushAsync());
			s.Close();

			s = OpenSession();
			IQuery q = s.GetNamedQuery("namedsql");
			Assert.IsNotNull(q, "should have found 'namedsql'");
			IList list = await (q.ListAsync());
			Assert.IsNotNull(list, "executing query returns list");

			object[] values = list[0] as object[];
			Assert.IsNotNull(values[0], "index 0 should not be null");
			Assert.IsNotNull(values[1], "index 1 should not be null");

			Assert.AreEqual(typeof(Category), values[0].GetType(), "should be a Category");
			Assert.AreEqual(typeof(Assignable), values[1].GetType(), "should be Assignable");
			await (s.DeleteAsync("from Category"));
			await (s.DeleteAsync("from Assignable"));
			await (s.FlushAsync());
			s.Close();
		}
	}
}
