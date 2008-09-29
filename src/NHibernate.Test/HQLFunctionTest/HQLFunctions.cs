using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.HQLFunctionTest
{
	/// <summary>
	/// This test run each HQL function separatelly so is easy to know wich function need
	/// an override in the specific dialect implementation.
	/// </summary>
	[TestFixture]
	//, Ignore("HQLFunctions not yet completely implemented (HQL parser don't parse HQLFunction)")]
	public class HQLFunctions : TestCase
	{
		static Hashtable notSupportedStandardFunction = new Hashtable();
		static HQLFunctions()
		{
			notSupportedStandardFunction.Add("locate",
				new System.Type[] { typeof(MsSql2000Dialect), typeof(MsSql2005Dialect), typeof(FirebirdDialect), typeof(PostgreSQLDialect) });
			notSupportedStandardFunction.Add("bit_length",
				new System.Type[] { typeof(MsSql2000Dialect), typeof(MsSql2005Dialect) });
			notSupportedStandardFunction.Add("extract",
				new System.Type[] { typeof(MsSql2000Dialect), typeof(MsSql2005Dialect) });
		}

		private void IgnoreIfNotSupported(string functionName)
		{
			if (notSupportedStandardFunction.ContainsKey(functionName))
			{
				IList dialects = (IList)notSupportedStandardFunction[functionName];
				if(dialects.Contains(Dialect.GetType()))
					Assert.Ignore(Dialect + " doesn't support "+functionName+" function.");
			}
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "HQLFunctionTest.Animal.hbm.xml", "HQLFunctionTest.MaterialResource.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Human");
				s.Delete("from Animal");
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
				object result = s.CreateQuery("select count(distinct a.id) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(long), result.GetType());
				Assert.AreEqual(2, result);

				result = s.CreateQuery("select count(*) from Animal").UniqueResult();
				Assert.AreEqual(typeof(long), result.GetType());
				Assert.AreEqual(2, result);

				// Count in where
				result = s.CreateQuery("select count(a.id) from Animal a having count(a.id)>1").UniqueResult();
				Assert.AreEqual(typeof(long), result.GetType());
				Assert.AreEqual(2, result);
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
				result = s.CreateQuery("select avg(a.BodyWeight) from Animal a having avg(a.BodyWeight)>0").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(15D, result);
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

				result = s.CreateQuery("select max(a.BodyWeight) from Animal a having max(a.BodyWeight)>0").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(20F, result);
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

				result = s.CreateQuery("select min(a.BodyWeight) from Animal a having min(a.BodyWeight)>0").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(10F, result);
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

				result = s.CreateQuery("select sum(a.BodyWeight) from Animal a having sum(a.BodyWeight)>0").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(30D, result);
			}
		}

		[Test, ExpectedException(typeof(QueryException))]
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
				s.CreateQuery("select distinct new SummaryItem(a.Description, sum(BodyWeight)) from Animal a").List<SummaryItem>();
			}
		}

		[Test]
		public void SubString()
		{
			IgnoreIfNotSupported("substring");
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql;

				bool twoArgSubstringSupported = Dialect.Functions["substring"] is AnsiSubstringFunction;

				if (twoArgSubstringSupported)
				{
					hql = "select substring(a.Description, 3) from Animal a";
					IList lresult = s.CreateQuery(hql).List();
					Assert.AreEqual(1, lresult.Count);
					Assert.AreEqual("cdef", lresult[0]);
				}

				hql = "from Animal a where substring(a.Description, 2, 3) = 'bcd'";
				Animal result = (Animal) s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where substring(a.Description, 2, 3) = ?";
				result = (Animal)s.CreateQuery(hql)
					.SetParameter(0, "bcd")
					.UniqueResult();
				Assert.AreEqual("abcdef", result.Description);


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

				if (twoArgSubstringSupported)
				{
					hql = "from Animal a where substring(a.Description, 4) = 'def'";
					result = (Animal) s.CreateQuery(hql).UniqueResult();
					Assert.AreEqual("abcdef", result.Description);
				}
			}
		}

		[Test]
		public void Locate()
		{
			IgnoreIfNotSupported("locate");
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
			IgnoreIfNotSupported("trim");
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
			IgnoreIfNotSupported("length");

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
			IgnoreIfNotSupported("bit_length");

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
			IgnoreIfNotSupported("coalesce");
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
			IgnoreIfNotSupported("nullif");
			// test only the parser and render
			using (ISession s = OpenSession())
			{
				string hql = "select nullif(h.NickName, '1e1') from Human h";
				IList result = s.CreateQuery(hql).List();

				hql = "from Human h where nullif(h.NickName, '1e1') not is null";
				result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Abs()
		{
			IgnoreIfNotSupported("abs");
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

				hql = "select abs(a.BodyWeight*-1) from Animal a group by abs(a.BodyWeight*-1) having abs(a.BodyWeight*-1)>0";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(1, lresult.Count);
			}
		}

		[Test]
		public void Mod()
		{
			IgnoreIfNotSupported("mod");
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
			IgnoreIfNotSupported("sqrt");
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
			IgnoreIfNotSupported("upper");
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

				//test only parser
				hql = "select upper(an.Description) from Animal an group by upper(an.Description) having upper(an.Description)='ABCDEF'";
				lresult = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Lower()
		{
			IgnoreIfNotSupported("lower");
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

				//test only parser
				hql = "select lower(an.Description) from Animal an group by lower(an.Description) having lower(an.Description)='abcdef'";
				lresult = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Cast()
		{
			IgnoreIfNotSupported("cast");
			// The cast is used to test various cases of a function render
			// Cast was selected because represent a special case for:
			// 1) Has more then 1 argument
			// 2) The argument separator is "as" (for the other function is ',' or ' ')
			// 3) The ReturnType is not fixed (depend on a column type)
			// 4) The 2th argument is parsed by NH and traslated for a specific Dialect (can't be interpreted directly by RDBMS)
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
				Assert.AreEqual(1.3f, l[0]);

				// Rendered in SELECT using a property in an operation with costant 
				hql = "select cast(7+123-5*a.BodyWeight as Double) from Animal a";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(7f + 123f - 5f * 1.3f, l[0]);

				// Rendered in SELECT using a property and nested functions
				hql = "select cast(cast(a.BodyWeight as string) as Double) from Animal a";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1.3F, l[0]);

				// TODO: Rendered in SELECT using string costant assigned with critic chars (separators)

				// Rendered in WHERE using a property 
				hql = "from Animal a where cast(a.BodyWeight as string) like '1.%'";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				// Rendered in WHERE using a property in an operation with costants
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
				hql = "from Animal a where cast(cast(cast(a.BodyWeight as string) as double) as int) = 1";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				// Rendered in GROUP BY using a property 
				hql = "select cast(a.BodyWeight as Double) from Animal a group by cast(a.BodyWeight as Double)";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1.3f, l[0]);

				// Rendered in GROUP BY using a property in an operation with costant 
				hql = "select cast(7+123-5*a.BodyWeight as Double) from Animal a group by cast(7+123-5*a.BodyWeight as Double)";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(7f + 123f - 5f * 1.3f, l[0]);

				// Rendered in GROUP BY using a property and nested functions
				hql = "select cast(cast(a.BodyWeight as string) as Double) from Animal a group by cast(cast(a.BodyWeight as string) as Double)";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1.3F, l[0]);

				// Rendered in HAVING using a property 
				hql = "select cast(a.BodyWeight as Double) from Animal a group by cast(a.BodyWeight as Double) having cast(a.BodyWeight as Double)>0";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1.3f, l[0]);

				// Rendered in HAVING using a property in an operation with costants
				hql = "select cast(7+123.3-1*a.BodyWeight as int) from Animal a group by cast(7+123.3-1*a.BodyWeight as int) having cast(7+123.3-1*a.BodyWeight as int)>0";
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(129, l[0]);

				// Rendered in HAVING using a property and named param (NOT SUPPORTED)
				try
				{
					hql = "select cast(:aParam+a.BodyWeight as int) from Animal a group by cast(:aParam+a.BodyWeight as int) having cast(:aParam+a.BodyWeight as int)>0";
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
					// This test raises an exception in SQL Server because named 
					// parameters internally are always positional (@p0, @p1, etc.)
					// and named differently hence they mismatch between GROUP BY and HAVING clauses.
					if (!ex.InnerException.Message.Equals(
						"Column 'Animal.BodyWeight' is invalid in the HAVING clause " +
						"because it is not contained in either an aggregate function or the GROUP BY clause."))
						throw;
				}

				// Rendered in HAVING using a property and nested functions
				string castExpr = "cast(cast(cast(a.BodyWeight as string) as double) as int)";
				hql = string.Format("select {0} from Animal a group by {0} having {0} = 1", castExpr);
				l = s.CreateQuery(hql).List();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual(1, l[0]);
			}
		}

		[Test]
		public void CastNH1446()
		{
			IgnoreIfNotSupported("cast");
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
				Assert.AreEqual(1.3f, l[0]);
			}
		}


		[Test]
		public void Current_TimeStamp()
		{
			IgnoreIfNotSupported("current_timestamp");
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

		[Test]
		public void Extract()
		{
			IgnoreIfNotSupported("extract");

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
			IgnoreIfNotSupported("concat");
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
			IgnoreIfNotSupported("second");
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
			IgnoreIfNotSupported("day");
			IgnoreIfNotSupported("month");
			IgnoreIfNotSupported("year");
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
			IgnoreIfNotSupported("str");
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
			if (!Dialect.Functions.ContainsKey("iif"))
				Assert.Ignore(Dialect + "doesn't support iif function.");
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
	}
}
