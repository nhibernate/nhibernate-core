using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	/// <summary>
	/// This test run each HQL function separately so is easy to know which function need
	/// an override in the specific dialect implementation.
	/// </summary>
	[TestFixture]
	public class HQLFunctions : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Hql.Animal.hbm.xml", "Hql.MaterialResource.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Human");
				s.Delete("from Animal");
				s.Delete("from MaterialResource");
				s.Flush();
			}
		}

		[Test]
		public void AggregateCount()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				// Count in select
				object result;
				if (TestDialect.SupportsCountDistinct)
				{
					result = s.CreateQuery("select count(distinct a.id) from Animal a").UniqueResult();
					Assert.AreEqual(typeof(long), result.GetType());
					Assert.AreEqual(2, result);
				}

				result = s.CreateQuery("select count(*) from Animal").UniqueResult();
				Assert.AreEqual(typeof(long), result.GetType());
				Assert.AreEqual(2, result);

				// Count in where
				if (TestDialect.SupportsHavingWithoutGroupBy)
				{
					result = s.CreateQuery("select count(a.id) from Animal a having count(a.id)>1").UniqueResult();
					Assert.AreEqual(typeof (long), result.GetType());
					Assert.AreEqual(2, result);
				}
			}
		}

		[Test]
		public void AggregateAvg()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				// In Select
				object result = s.CreateQuery("select avg(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(15D, result);

				// In where
				if (TestDialect.SupportsHavingWithoutGroupBy)
				{
					result = s.CreateQuery("select avg(a.BodyWeight) from Animal a having avg(a.BodyWeight)>0").UniqueResult();
					Assert.AreEqual(typeof(double), result.GetType());
					Assert.AreEqual(15D, result);
				}
			}
		}

		[Test]
		public void AggregateMax()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select max(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(20F, result);

				if (TestDialect.SupportsHavingWithoutGroupBy)
				{
					result = s.CreateQuery("select max(a.BodyWeight) from Animal a having max(a.BodyWeight)>0").UniqueResult();
					Assert.AreEqual(typeof(float), result.GetType()); //use column type
					Assert.AreEqual(20F, result);
				}
			}
		}

		[Test]
		public void AggregateMin()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select min(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(10F, result);

				if (TestDialect.SupportsHavingWithoutGroupBy)
				{
					result = s.CreateQuery("select min(a.BodyWeight) from Animal a having min(a.BodyWeight)>0").UniqueResult();
					Assert.AreEqual(typeof(float), result.GetType()); //use column type
					Assert.AreEqual(10F, result);
				}
			}
		}

		[Test]
		public void AggregateSum()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select sum(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(30D, result);

				if (TestDialect.SupportsHavingWithoutGroupBy)
				{
					result = s.CreateQuery("select sum(a.BodyWeight) from Animal a having sum(a.BodyWeight)>0").UniqueResult();
					Assert.AreEqual(typeof(double), result.GetType());
					Assert.AreEqual(30D, result);
				}
			}
		}

		[Test]
		public void AggregateSumNH1100()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a1", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				Assert.Throws<QueryException>(() => s.CreateQuery("select distinct new SummaryItem(a.Description, sum(BodyWeight)) from Animal a").List<SummaryItem>());
			}
		}

		[Test]
		public void AggregatesAndMathNH959()
		{
			using (ISession s = OpenSession())
			{
				Assert.DoesNotThrow(() => s.CreateQuery("select a.Id, sum(BodyWeight)/avg(BodyWeight) from Animal a group by a.Id having sum(BodyWeight)>0").List());
			}			
		}


		[Test]
		public void SubStringTwoParameters()
		{
			// All dialects that support the substring function should support
			// the two-parameter overload - emulating it by generating the 
			// third parameter (length) if the database requires three parameters.

			AssumeFunctionSupported("substring");

			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				string hql;

				// In the select clause.
				hql = "select substring(a.Description, 3) from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(1, lresult.Count);
				Assert.AreEqual("cdef", lresult[0]);

				// In the where clause.
				hql = "from Animal a where substring(a.Description, 4) = 'def'";
				var result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				// With parameters and nested function calls.
				hql = "from Animal a where substring(concat(a.Description, ?), :start) = 'deffoo'";
				result = (Animal) s.CreateQuery(hql)
				                   .SetParameter(0, "foo")
				                   .SetParameter("start", 4)
				                   .UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}


		[Test]
		public void SubString()
		{
			AssumeFunctionSupported("substring");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql;

				hql = "from Animal a where substring(a.Description, 2, 3) = 'bcd'";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where substring(a.Description, 2, 3) = ?";
				result = (Animal)s.CreateQuery(hql)
					.SetParameter(0, "bcd")
					.UniqueResult();
				Assert.AreEqual("abcdef", result.Description);


				// Following tests verify that parameters can be used.

				hql = "from Animal a where substring(a.Description, 2, ?) = 'bcd'";
				result = (Animal)s.CreateQuery(hql)
					.SetParameter(0, 3)
					.UniqueResult();
				Assert.AreEqual("abcdef", result.Description);


				hql = "from Animal a where substring(a.Description, ?, ?) = ?";
				result = (Animal)s.CreateQuery(hql)
					.SetParameter(0, 2)
					.SetParameter(1, 3)
					.SetParameter(2, "bcd")
					.UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "select substring(a.Description, ?, ?) from Animal a";
				IList results = s.CreateQuery(hql)
					.SetParameter(0, 2)
					.SetParameter(1, 3)
					.List();
				Assert.AreEqual(1, results.Count);
				Assert.AreEqual("bcd", results[0]);
			}
		}

		[Test]
		public void Locate()
		{
			AssumeFunctionSupported("locate");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select locate('bc', a.Description, 2) from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(2, lresult[0]);

				hql = "from Animal a where locate('bc', a.Description) = 2";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Trim()
		{
			AssumeFunctionSupported("trim");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abc   ", 1);
				Animal a2 = new Animal("   def", 2);
				Animal a3 = new Animal("___def__", 3);
				s.Save(a1);
				s.Save(a2);
				s.Save(a3);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select trim(a.Description) from Animal a where a.Description='   def'";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("def", lresult[0]);

				hql = "select trim('_' from a.Description) from Animal a where a.Description='___def__'";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("def", lresult[0]);

				hql = "select trim(trailing from a.Description) from Animal a where a.Description= 'abc   '";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("abc", lresult[0]);

				hql = "select trim(leading from a.Description) from Animal a where a.Description='   def'";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("def", lresult[0]);

				// where
				hql = "from Animal a where trim(a.Description) = 'abc'";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(1, lresult.Count);

				hql = "from Animal a where trim('_' from a.Description) = 'def'";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("___def__", result.Description);

				hql = "from Animal a where trim(trailing from a.Description) = 'abc'";
				result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual(1, result.BodyWeight); //Firebird auto rtrim VARCHAR

				hql = "from Animal a where trim(leading from a.Description) = 'def'";
				result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("   def", result.Description);

				Animal a = new Animal("   abc", 20);
				s.Save(a);
				s.Flush();
				hql = "from Animal a where trim(both from a.Description) = 'abc'";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(2, lresult.Count);
			}
		}

		[Test]
		public void Length()
		{
			AssumeFunctionSupported("length");

			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("12345", 20);
				Animal a2 = new Animal("1234", 20);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select length(a.Description) from Animal a where a.Description = '1234'";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(4, lresult[0]);

				hql = "from Animal a where length(a.Description) = 5";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("12345", result.Description);
			}
		}

		[Test]
		public void Bit_length()
		{
			AssumeFunctionSupported("bit_length");

			// test only the parser
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where bit_length(a.Description) = 24";
				IList result = s.CreateQuery(hql).List();

				hql = "select bit_length(a.Description) from Animal a";
				result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Coalesce()
		{
			AssumeFunctionSupported("coalesce");
			// test only the parser and render
			using (ISession s = OpenSession())
			{
				string hql = "select coalesce(h.NickName, h.Name.First, h.Name.Last) from Human h";
				IList result = s.CreateQuery(hql).List();

				hql = "from Human h where coalesce(h.NickName, h.Name.First, h.Name.Last) = 'max'";
				result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Nullif()
		{
			AssumeFunctionSupported("nullif");
			string hql1, hql2;
			if(!(Dialect is Oracle8iDialect))
			{
				hql1 = "select nullif(h.NickName, '1e1') from Human h";
				hql2 = "from Human h where not(nullif(h.NickName, '1e1') is null)";
			}
			else
			{
				// Oracle need same specific types
				hql1 = "select nullif(str(h.NickName), '1e1') from Human h";
				hql2 = "from Human h where not (nullif(str(h.NickName), '1e1') is null)";				
			}
			// test only the parser and render
			using (ISession s = OpenSession())
			{
				IList result = s.CreateQuery(hql1).List();
				result = s.CreateQuery(hql2).List();
			}
		}

		[Test]
		public void Abs()
		{
			AssumeFunctionSupported("abs");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("Dog", 9);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select abs(a.BodyWeight*-1) from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(9, lresult[0]);

				hql = "from Animal a where abs(a.BodyWeight*-1)>0";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(1, lresult.Count);

				if (Dialect.SupportsHavingOnGroupedByComputation)
				{
					hql = "select abs(a.BodyWeight*-1) from Animal a group by abs(a.BodyWeight*-1) having abs(a.BodyWeight*-1)>0";
					lresult = s.CreateQuery(hql).List();
					Assert.AreEqual(1, lresult.Count);
				}
			}
		}

		[Test]
		public void Ceiling()
		{
			AssumeFunctionSupported("ceiling");

			using (var s = OpenSession())
			{
				var a1 = new Animal("a1", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var ceiling = s.CreateQuery("select ceiling(a.BodyWeight) from Animal a").UniqueResult<float>();
				Assert.That(ceiling, Is.EqualTo(2));
				var count =
					s
						.CreateQuery("select count(*) from Animal a where ceiling(a.BodyWeight) = :c")
						.SetInt32("c", 2)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1));
			}
		}

		[Test]
		public void Round()
		{
			AssumeFunctionSupported("round");

			using (var s = OpenSession())
			{
				var a1 = new Animal("a1", 1.87f);
				s.Save(a1);
				var m1 = new MaterialResource("m1", "18", MaterialResource.MaterialState.Available) { Cost = 51.76m };
				s.Save(m1);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var roundF = s.CreateQuery("select round(a.BodyWeight) from Animal a").UniqueResult<float>();
				Assert.That(roundF, Is.EqualTo(2), "Selecting round(double) failed.");
				var countF =
					s
						.CreateQuery("select count(*) from Animal a where round(a.BodyWeight) = :c")
						.SetInt32("c", 2)
						.UniqueResult<long>();
				Assert.That(countF, Is.EqualTo(1), "Filtering round(double) failed.");
				
				roundF = s.CreateQuery("select round(a.BodyWeight, 1) from Animal a").UniqueResult<float>();
				Assert.That(roundF, Is.EqualTo(1.9f).Within(0.01f), "Selecting round(double, 1) failed.");
				countF =
					s
						.CreateQuery("select count(*) from Animal a where round(a.BodyWeight, 1) between :c1 and :c2")
						.SetDouble("c1", 1.89)
						.SetDouble("c2", 1.91)
						.UniqueResult<long>();
				Assert.That(countF, Is.EqualTo(1), "Filtering round(double, 1) failed.");

				var roundD = s.CreateQuery("select round(m.Cost) from MaterialResource m").UniqueResult<decimal?>();
				Assert.That(roundD, Is.EqualTo(52), "Selecting round(decimal) failed.");
				var count =
					s
						.CreateQuery("select count(*) from MaterialResource m where round(m.Cost) = :c")
						.SetInt32("c", 52)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1), "Filtering round(decimal) failed.");

				roundD = s.CreateQuery("select round(m.Cost, 1) from MaterialResource m").UniqueResult<decimal?>();
				Assert.That(roundD, Is.EqualTo(51.8m), "Selecting round(decimal, 1) failed.");

				if (TestDialect.HasBrokenDecimalType)
					// SQLite fails the equality test due to using double instead, wich requires a tolerance.
					return;

				count =
					s
						.CreateQuery("select count(*) from MaterialResource m where round(m.Cost, 1) = :c")
						.SetDecimal("c", 51.8m)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1), "Filtering round(decimal, 1) failed.");
			}
		}

		[Test]
		public void Truncate()
		{
			AssumeFunctionSupported("truncate");

			using (var s = OpenSession())
			{
				var a1 = new Animal("a1", 1.87f);
				s.Save(a1);
				var m1 = new MaterialResource("m1", "18", MaterialResource.MaterialState.Available) { Cost = 51.76m };
				s.Save(m1);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var roundF = s.CreateQuery("select truncate(a.BodyWeight) from Animal a").UniqueResult<float>();
				Assert.That(roundF, Is.EqualTo(1), "Selecting truncate(double) failed.");
				var countF =
					s
						.CreateQuery("select count(*) from Animal a where truncate(a.BodyWeight) = :c")
						.SetInt32("c", 1)
						.UniqueResult<long>();
				Assert.That(countF, Is.EqualTo(1), "Filtering truncate(double) failed.");
				
				roundF = s.CreateQuery("select truncate(a.BodyWeight, 1) from Animal a").UniqueResult<float>();
				Assert.That(roundF, Is.EqualTo(1.8f).Within(0.01f), "Selecting truncate(double, 1) failed.");
				countF =
					s
						.CreateQuery("select count(*) from Animal a where truncate(a.BodyWeight, 1) between :c1 and :c2")
						.SetDouble("c1", 1.79)
						.SetDouble("c2", 1.81)
						.UniqueResult<long>();
				Assert.That(countF, Is.EqualTo(1), "Filtering truncate(double, 1) failed.");

				var roundD = s.CreateQuery("select truncate(m.Cost) from MaterialResource m").UniqueResult<decimal?>();
				Assert.That(roundD, Is.EqualTo(51), "Selecting truncate(decimal) failed.");
				var count =
					s
						.CreateQuery("select count(*) from MaterialResource m where truncate(m.Cost) = :c")
						.SetInt32("c", 51)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1), "Filtering truncate(decimal) failed.");

				roundD = s.CreateQuery("select truncate(m.Cost, 1) from MaterialResource m").UniqueResult<decimal?>();
				Assert.That(roundD, Is.EqualTo(51.7m), "Selecting truncate(decimal, 1) failed.");

				if (TestDialect.HasBrokenDecimalType)
					// SQLite fails the equality test due to using double instead, wich requires a tolerance.
					return;

				count =
					s
						.CreateQuery("select count(*) from MaterialResource m where truncate(m.Cost, 1) = :c")
						.SetDecimal("c", 51.7m)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1), "Filtering truncate(decimal, 1) failed.");
			}
		}

		[Test]
		public void Mod()
		{
			AssumeFunctionSupported("mod");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select mod(cast(a.BodyWeight as int), 3) from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(2, lresult[0]);

				hql = "from Animal a where mod(20, 3) = 2";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where mod(cast(a.BodyWeight as int), 4)=0";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Sqrt()
		{
			AssumeFunctionSupported("sqrt");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 65536f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select sqrt(an.BodyWeight) from Animal an";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(256f, lresult[0]);

				hql = "from Animal an where sqrt(an.BodyWeight)/2 > 10";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Upper()
		{
			AssumeFunctionSupported("upper");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select upper(an.Description) from Animal an";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("ABCDEF", lresult[0]);

				hql = "from Animal an where upper(an.Description)='ABCDEF'";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
				
				if (Dialect.SupportsHavingOnGroupedByComputation)
				{
					//test only parser
					hql = "select upper(an.Description) from Animal an group by upper(an.Description) having upper(an.Description)='ABCDEF'";
					lresult = s.CreateQuery(hql).List();
				}
			}
		}

		[Test]
		public void Lower()
		{
			AssumeFunctionSupported("lower");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("ABCDEF", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select lower(an.Description) from Animal an";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("abcdef", lresult[0]);

				hql = "from Animal an where lower(an.Description)='abcdef'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("ABCDEF", result.Description);

				if (Dialect.SupportsHavingOnGroupedByComputation)
				{
					//test only parser
					hql = "select lower(an.Description) from Animal an group by lower(an.Description) having lower(an.Description)='abcdef'";
					lresult = s.CreateQuery(hql).List();
				}
			}
		}

		[Test]
		public void Ascii()
		{
			AssumeFunctionSupported("ascii");

			using (var s = OpenSession())
			{
				var m = new MaterialResource(" ", "000", MaterialResource.MaterialState.Available);
				s.Save(m);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var space = s.CreateQuery("select ascii(m.Description) from MaterialResource m").UniqueResult<int>();
				Assert.That(space, Is.EqualTo(32));
				var count =
					s
						.CreateQuery("select count(*) from MaterialResource m where ascii(m.Description) = :c")
						.SetInt32("c", 32)
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1));
			}
		}

		[Test]
		public void Chr()
		{
			AssumeFunctionSupported("chr");

			using (var s = OpenSession())
			{
				var m = new MaterialResource("Blah", "000", (MaterialResource.MaterialState)32);
				s.Save(m);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var space = s.CreateQuery("select chr(m.State) from MaterialResource m").UniqueResult<char>();
				Assert.That(space, Is.EqualTo(' '));
				var count =
					s
						.CreateQuery("select count(*) from MaterialResource m where chr(m.State) = :c")
						.SetCharacter("c", ' ')
						.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(1));
			}
		}

		[Test]
		public void Cast()
		{
			const double magicResult = 7 + 123 - 5*1.3d;

			AssumeFunctionSupported("cast");
			// The cast is used to test various cases of a function render
			// Cast was selected because represent a special case for:
			// 1) Has more then 1 argument
			// 2) The argument separator is "as" (for the other function is ',' or ' ')
			// 3) The ReturnType is not fixed (depend on a column type)
			// 4) The 2th argument is parsed by NH and translated for a specific Dialect (can't be interpreted directly by RDBMS)
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql;
				IList l;
				Animal result;
				// Rendered in SELECT using a property 
				hql = "select cast(a.BodyWeight as Double) from Animal a";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.That(l[0], Is.TypeOf(typeof (double)));

				// Rendered in SELECT using a property in an operation with constants
				hql = "select cast(7+123-5*a.BodyWeight as Double) from Animal a";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(magicResult, (double)l[0], 0.00001);

				// Rendered in SELECT using a property and nested functions
				if (!(Dialect is Oracle8iDialect))
				{
					hql = "select cast(cast(a.BodyWeight as string) as Double) from Animal a";
					l = s.CreateQuery(hql).List();
					Assert.AreEqual(1, l.Count);
					Assert.That(l[0], Is.TypeOf(typeof(double)));
				}

				// TODO: Rendered in SELECT using string constant assigned with critic chars (separators)

				// Rendered in WHERE using a property 
				if (!(Dialect is Oracle8iDialect))
				{
					hql = "from Animal a where cast(a.BodyWeight as string) like '1.%'";
					result = (Animal) s.CreateQuery(hql).UniqueResult();
					Assert.AreEqual("abcdef", result.Description);
				}

				// Rendered in WHERE using a property in an operation with constants
				hql = "from Animal a where cast(7+123-2*a.BodyWeight as Double)>0";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				// Rendered in WHERE using a property and named param
				hql = "from Animal a where cast(:aParam+a.BodyWeight as Double)>0";
				result = (Animal)s.CreateQuery(hql)
					.SetDouble("aParam", 2D)
					.UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				// Rendered in WHERE using a property and nested functions
				if (!(Dialect is Oracle8iDialect))
				{
					hql = "from Animal a where cast(cast(cast(a.BodyWeight as string) as double) as int) = 1";
					result = (Animal) s.CreateQuery(hql).UniqueResult();
					Assert.AreEqual("abcdef", result.Description);
				}

				// Rendered in GROUP BY using a property 
				hql = "select cast(a.BodyWeight as Double) from Animal a group by cast(a.BodyWeight as Double)";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.That(l[0], Is.TypeOf(typeof(double)));

				// Rendered in GROUP BY using a property in an operation with constants
				hql = "select cast(7+123-5*a.BodyWeight as Double) from Animal a group by cast(7+123-5*a.BodyWeight as Double)";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(magicResult, (double)l[0], 0.00001);

				// Rendered in GROUP BY using a property and nested functions
				if (!(Dialect is Oracle8iDialect))
				{
					hql =
						"select cast(cast(a.BodyWeight as string) as Double) from Animal a group by cast(cast(a.BodyWeight as string) as Double)";
					l = s.CreateQuery(hql).List();
					Assert.AreEqual(1, l.Count);
					Assert.That(l[0], Is.TypeOf(typeof(double)));
				}

				if (Dialect.SupportsHavingOnGroupedByComputation)
				{
					// Rendered in HAVING using a property
					hql =
						"select cast(a.BodyWeight as Double) from Animal a group by cast(a.BodyWeight as Double) having cast(a.BodyWeight as Double)>0";
					l = s.CreateQuery(hql).List();
					Assert.AreEqual(1, l.Count);
					Assert.That(l[0], Is.TypeOf(typeof(double)));

					// Rendered in HAVING using a property in an operation with constants
					hql =
						"select cast(7+123.3-1*a.BodyWeight as int) from Animal a group by cast(7+123.3-1*a.BodyWeight as int) having cast(7+123.3-1*a.BodyWeight as int)>0";
					l = s.CreateQuery(hql).List();
					Assert.AreEqual(1, l.Count);
					Assert.AreEqual((int)(7 + 123.3 - 1 * 1.3d), l[0]);

					// Rendered in HAVING using a property and named param (NOT SUPPORTED)
					try
					{
						hql =
							"select cast(:aParam+a.BodyWeight as int) from Animal a group by cast(:aParam+a.BodyWeight as int) having cast(:aParam+a.BodyWeight as int)>0";
						l = s.CreateQuery(hql).SetInt32("aParam", 10).List();
						Assert.AreEqual(1, l.Count);
						Assert.AreEqual(11, l[0]);
					}
					catch (QueryException ex)
					{
						if (!(ex.InnerException is NotSupportedException))
							throw;
					}
					catch (ADOException ex)
					{
						if (ex.InnerException == null)
							throw;

						if (Dialect is Oracle8iDialect)
						{
							if (!ex.InnerException.Message.StartsWith("ORA-00979"))
								throw;
						}
						else if (Dialect is FirebirdDialect)
						{
							string msgToCheck =
								"not contained in either an aggregate function or the GROUP BY clause";
							// This test raises an exception in Firebird for an unknown reason.
							if (!ex.InnerException.Message.Contains(msgToCheck))
								throw;
						}
						else if (Dialect is MsSqlCeDialect)
						{
							var errorCodeProperty = ex.InnerException.GetType().GetProperty("NativeError");
							if (errorCodeProperty == null ||
								// 25515 is the error code for "In aggregate and grouping expressions, the SELECT clause can contain only aggregates and grouping expressions."
								// https://technet.microsoft.com/en-us/library/ms172350(v=sql.110).aspx
								errorCodeProperty.GetValue(ex.InnerException) as int? != 25515)
							{
								throw;
							}
						}
						else if (Dialect is HanaDialectBase)
						{
							string msgToCheck =
								"not a GROUP BY expression: 'ANIMAL0_.BODYWEIGHT' must be in group by clause";
							if (!ex.InnerException.Message.Contains(msgToCheck))
								throw;
						}
						else
						{
							string msgToCheck =
								"Column 'Animal.BodyWeight' is invalid in the HAVING clause because it is not contained in either an aggregate function or the GROUP BY clause.";
							// This test raises an exception in SQL Server because named 
							// parameters internally are always positional (@p0, @p1, etc.)
							// and named differently hence they mismatch between GROUP BY and HAVING clauses.
							if (!ex.InnerException.Message.Contains(msgToCheck))
								throw;
						}
					}

					// Rendered in HAVING using a property and nested functions
					if (!(Dialect is Oracle8iDialect))
					{
						string castExpr = "cast(cast(cast(a.BodyWeight as string) as double) as int)";
						hql = string.Format("select {0} from Animal a group by {0} having {0} = 1", castExpr);
						l = s.CreateQuery(hql).List();
						Assert.AreEqual(1, l.Count);
						Assert.AreEqual(1, l[0]);
					}
				}
			}
		}

		[Test]
		public void CastNH1446()
		{
			AssumeFunctionSupported("cast");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				// Rendered in SELECT using a property 
				string hql = "select cast(a.BodyWeight As Double) from Animal a";
				IList l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1.3f, (double)l[0], 0.00001);
			}
		}

		[Test]
		public void CastNH1979()
		{
			AssumeFunctionSupported("cast");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select cast(((a.BodyWeight + 50) / :divisor) as int) from Animal a";
				IList l = s.CreateQuery(hql).SetInt32("divisor", 2).List();
				Assert.AreEqual(1, l.Count);
			}
		}

		[Test]
		public void Current_TimeStamp()
		{
			AssumeFunctionSupported("current_timestamp");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select current_timestamp() from Animal";
				IList result = s.CreateQuery(hql).List();
			}
		}

		/// <summary>
		/// NH-1658
		/// </summary>
		[Test]
		public void Current_TimeStamp_Offset()
		{
			AssumeFunctionSupported("current_timestamp_offset");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select current_timestamp_offset() from Animal";
				IList result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Extract()
		{
			AssumeFunctionSupported("extract");
			AssumeFunctionSupported("current_timestamp");

			// test only the parser and render
			using (ISession s = OpenSession())
			{
				string hql = "select extract(second from current_timestamp()), extract(minute from current_timestamp()), extract(hour from current_timestamp()) from Animal";
				IList result = s.CreateQuery(hql).List();

				hql = "from Animal where extract(day from cast(current_timestamp() as Date))>0";
				result = s.CreateQuery(hql).List();
			}	
		}

		[Test]
		public void Concat()
		{
			AssumeFunctionSupported("concat");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select concat(a.Description,'zzz') from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("abcdefzzz", lresult[0]);

				// MS SQL doesn't support || operator
				if (!(Dialect is MsSql2000Dialect))
				{
					hql = "from Animal a where a.Description = concat('a', concat('b','c'), 'd'||'e')||'f'";
					Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
					Assert.AreEqual("abcdef", result.Description);
				}
			}
		}

		[Test]
		public void HourMinuteSecond()
		{
			AssumeFunctionSupported("second");
			AssumeFunctionSupported("minute");
			AssumeFunctionSupported("hour");
			AssumeFunctionSupported("current_timestamp");
			// test only the parser and render
			using (ISession s = OpenSession())
			{
				string hql = "select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Animal";
				IList result = s.CreateQuery(hql).List();
			}	
		}

		[Test]
		public void DayMonthYear()
		{
			AssumeFunctionSupported("day");
			AssumeFunctionSupported("month");
			AssumeFunctionSupported("year");
			// test only the parser and render
			using (ISession s = OpenSession())
			{
				string hql = "select day(h.Birthdate), month(h.Birthdate), year(h.Birthdate) from Human h";
				IList result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Str()
		{
			AssumeFunctionSupported("str");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select str(a.BodyWeight) from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(typeof(string), lresult[0].GetType());

				hql = "from Animal a where str(123) = '123'";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Iif()
		{
			AssumeFunctionSupported("iif");
			using (ISession s = OpenSession())
			{
				s.Save(new MaterialResource("Flash card 512MB", "A001/07", MaterialResource.MaterialState.Available));
				s.Save(new MaterialResource("Flash card 512MB", "A002/07", MaterialResource.MaterialState.Available));
				s.Save(new MaterialResource("Flash card 512MB", "A003/07", MaterialResource.MaterialState.Reserved));
				s.Save(new MaterialResource("Flash card 512MB", "A004/07", MaterialResource.MaterialState.Reserved));
				s.Save(new MaterialResource("Flash card 512MB", "A005/07", MaterialResource.MaterialState.Discarded));
				s.Flush();
			}

			// Statistic
			using (ISession s = OpenSession())
			{
				string hql = 
@"select mr.Description, 
sum(iif(mr.State= 0,1,0)), 
sum(iif(mr.State= 1,1,0)), 
sum(iif(mr.State= 2,1,0)) 
from MaterialResource mr
group by mr.Description";

				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual("Flash card 512MB", ((IList)lresult[0])[0]);
				Assert.AreEqual(2, ((IList)lresult[0])[1]);
				Assert.AreEqual(2, ((IList)lresult[0])[2]);
				Assert.AreEqual(1, ((IList)lresult[0])[3]);

				hql = "from MaterialResource mr where iif(mr.State=2,true,false)=true";
				MaterialResource result = (MaterialResource)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("A005/07", result.SerialNumber);
			}
			// clean up
			using (ISession s = OpenSession())
			{
				s.Delete("from MaterialResource");
				s.Flush();
			}
		}

		[Test]
		public void NH1725()
		{
			AssumeFunctionSupported("iif");
			// Only to test the parser
			using (ISession s = OpenSession())
			{
				var hql = "select new ForNh1725(mr.Description, iif(mr.State= 0,1,0)) from MaterialResource mr";
				s.CreateQuery(hql).List();
				hql = "select new ForNh1725(mr.Description, cast(iif(mr.State= 0,1,0) as int)) from MaterialResource mr";
				s.CreateQuery(hql).List();
			}
		}

		[Test, Ignore("Not supported yet!")]
		public void ParameterLikeArgument()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				string hql;
				IList l;
				Animal result;

				// Render in WHERE
				hql = "from Animal a where cast(:aParam as Double)>0";
				result = (Animal)s.CreateQuery(hql).SetDouble("aParam", 2D).UniqueResult();
				Assert.IsNotNull(result);

				// Render in WHERE with math operation
				hql = "from Animal a where cast(:aParam+a.BodyWeight as Double)>3";
				result = (Animal) s.CreateQuery(hql).SetDouble("aParam", 2D).UniqueResult();
				Assert.IsNotNull(result);

				// Render in all clauses
				hql =
					"select cast(:aParam+a.BodyWeight as int) from Animal a group by cast(:aParam+a.BodyWeight as int) having cast(:aParam+a.BodyWeight as Double)>0";
				l = s.CreateQuery(hql).SetInt32("aParam", 10).List();
				Assert.AreEqual(1, l.Count);
			}
		}

		[Test]
		public void BitwiseAnd()
		{
			AssumeFunctionSupported("band");
			CreateMaterialResources();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var query = s.CreateQuery("from MaterialResource m where (m.State & 1) > 0");
				var result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "& 1");

				query = s.CreateQuery("from MaterialResource m where (m.State & 2) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "& 2");

				query = s.CreateQuery("from MaterialResource m where (m.State & 3) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(2), "& 3");

				tx.Commit();
			}
		}

		[Test]
		public void BitwiseOr()
		{
			AssumeFunctionSupported("bor");
			CreateMaterialResources();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var query = s.CreateQuery("from MaterialResource m where (m.State | 1) > 0");
				var result = query.List();
				Assert.That(result, Has.Count.EqualTo(3), "| 1) > 0");

				query = s.CreateQuery("from MaterialResource m where (m.State | 1) > 1");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "| 1) > 1");

				query = s.CreateQuery("from MaterialResource m where (m.State | 0) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(2), "| 0) > 0");

				tx.Commit();
			}
		}

		[Test]
		public void BitwiseXor()
		{
			AssumeFunctionSupported("bxor");
			CreateMaterialResources();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var query = s.CreateQuery("from MaterialResource m where (m.State ^ 1) > 0");
				var result = query.List();
				Assert.That(result, Has.Count.EqualTo(2), "^ 1");

				query = s.CreateQuery("from MaterialResource m where (m.State ^ 2) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(2), "^ 2");

				query = s.CreateQuery("from MaterialResource m where (m.State ^ 3) > 0");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(3), "^ 3");

				tx.Commit();
			}
		}

		[Test]
		public void BitwiseNot()
		{
			AssumeFunctionSupported("bnot");
			AssumeFunctionSupported("band");
			CreateMaterialResources();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				// ! takes not precedence over & at least with some dialects (maybe all).
				var query = s.CreateQuery("from MaterialResource m where ((!m.State) & 3) = 3");
				var result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "((!m.State) & 3) = 3");

				query = s.CreateQuery("from MaterialResource m where ((!m.State) & 3) = 2");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "((!m.State) & 3) = 2");

				query = s.CreateQuery("from MaterialResource m where ((!m.State) & 3) = 1");
				result = query.List();
				Assert.That(result, Has.Count.EqualTo(1), "((!m.State) & 3) = 1");

				tx.Commit();
			}
		}

		// #1670
		[Test]
		public void BitwiseIsThreadsafe()
		{
			AssumeFunctionSupported("band");
			AssumeFunctionSupported("bor");
			AssumeFunctionSupported("bxor");
			AssumeFunctionSupported("bnot");
			var queries = new List<Tuple<string, int>>
			{
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State & 1) > 0", 1),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State & 2) > 0", 1),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State & 3) > 0", 2),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State | 1) > 0", 3),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State | 1) > 1", 1),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State | 0) > 0", 2),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State ^ 1) > 0", 2),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State ^ 2) > 0", 2),
				new Tuple<string, int> ("select count(*) from MaterialResource m where (m.State ^ 3) > 0", 3),
				new Tuple<string, int> ("select count(*) from MaterialResource m where ((!m.State) & 3) = 3", 1),
				new Tuple<string, int> ("select count(*) from MaterialResource m where ((!m.State) & 3) = 2", 1),
				new Tuple<string, int> ("select count(*) from MaterialResource m where ((!m.State) & 3) = 1", 1)
			};
			// Do not use a ManualResetEventSlim, it does not support async and exhausts the task thread pool in the
			// async counterparts of this test. SemaphoreSlim has the async support and release the thread when waiting.
			var semaphore = new SemaphoreSlim(0);
			var failures = new ConcurrentBag<Exception>();

			CreateMaterialResources();

			Parallel.For(
				0, queries.Count + 1,
				i =>
				{
					if (i >= queries.Count)
					{
						// Give some time to threads for reaching the wait, having all of them ready to do the
						// critical part of their job concurrently.
						Thread.Sleep(100);
						semaphore.Release(queries.Count);
						return;
					}

					try
					{
						var query = queries[i];
						using (var s = OpenSession())
						using (var tx = s.BeginTransaction())
						{
							semaphore.Wait();
							var q = s.CreateQuery(query.Item1);
							var result = q.UniqueResult<long>();
							Assert.That(result, Is.EqualTo(query.Item2), query.Item1);
							tx.Commit();
						}
					}
					catch (Exception e)
					{
						failures.Add(e);
					}
				});

			Assert.That(failures, Is.Empty, $"{failures.Count} task(s) failed.");
		}

		private void CreateMaterialResources()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new MaterialResource("m1", "18", MaterialResource.MaterialState.Available) { Cost = 51.76m });
				s.Save(new MaterialResource("m2", "19", MaterialResource.MaterialState.Reserved) { Cost = 15.24m });
				s.Save(new MaterialResource("m3", "20", MaterialResource.MaterialState.Discarded) { Cost = 21.54m });
				tx.Commit();
			}
		}
	}

	public class ForNh1725
	{
		public string Description { get; set; }
		public int Value { get; set; }

		public ForNh1725(string description, int value)
		{
			Description = description;
			Value = value;
		}
	}
}
