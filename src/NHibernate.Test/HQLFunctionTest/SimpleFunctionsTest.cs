using System;
using System.Collections;
using NHibernate.SqlTypes;
using NUnit.Framework;
using NHibernate.Dialect.Function;
using NHibernate;

namespace NHibernate.Test.HQLFunctionTest
{
	[TestFixture]
	public class SimpleFunctionsTest: BaseFunctionFixture
	{
		[Test]
		public void NoArgFunction()
		{
			IList args = new ArrayList();
			NoArgSQLFunction nf = new NoArgSQLFunction("noArgs", NHibernateUtil.String);
			Assert.IsTrue(nf.HasParenthesesIfNoArguments);
			Assert.AreEqual("noArgs()", nf.Render(args, factoryImpl));

			nf = new NoArgSQLFunction("noArgs", NHibernateUtil.String,false);
			Assert.IsFalse(nf.HasParenthesesIfNoArguments);
			Assert.AreEqual("noArgs", nf.Render(args, factoryImpl));

			args.Add("aparam");
			try
			{
				string t = nf.Render(args, factoryImpl);
				Assert.Fail("No exception if has argument");
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void StandardFunction()
		{
			IList args = new ArrayList();

			StandardSQLFunction sf = new StandardSQLFunction("fname");
			Assert.AreEqual("fname()", sf.Render(args, factoryImpl));

			args.Add(1);
			args.Add(2);
			Assert.AreEqual("fname(1, 2)", sf.Render(args, factoryImpl));
		}

		[Test]
		public void CastFunc()
		{
			IList args = new ArrayList();

			CastFunction cf = new CastFunction();
			try
			{
				string t = cf.Render(args, factoryImpl);
				Assert.Fail("No exception if no argument");
			}
			catch (QueryException)
			{
				//correct
			}

			args.Add("'123'");
			args.Add("long");
			string expected = string.Format("cast({0} as {1})", args[0], factoryImpl.Dialect.GetCastTypeName(SqlTypeFactory.Int64));
			Assert.AreEqual(expected, cf.Render(args, factoryImpl));

			args.Clear();
			args.Add("'123'");
			args.Add("NO_TYPE");
			try
			{
				string t = cf.Render(args, factoryImpl);
				Assert.Fail("Ivalid type accepted");
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void VarArgsFunction()
		{
			IList args = new ArrayList();

			VarArgsSQLFunction vf = new VarArgsSQLFunction("(", " || ", ")");
			Assert.AreEqual("()", vf.Render(args, factoryImpl));

			args.Add("va1");
			Assert.AreEqual("(va1)", vf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			args.Add("va3");
			Assert.AreEqual("(va1 || va2 || va3)", vf.Render(args, factoryImpl));
		}

		[Test]
		public void Nvl()
		{
			IList args = new ArrayList();

			NvlFunction nf = new NvlFunction();
			args.Add("va1");
			Assert.AreEqual("va1", nf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			args.Add("va3");
			Assert.AreEqual("nvl(va1, nvl(va2, va3))", nf.Render(args, factoryImpl));
		}

		[Test]
		public void PositionSubstring()
		{
			IList args = new ArrayList();

			PositionSubstringFunction psf = new PositionSubstringFunction();
			args.Add("'a'");
			args.Add("va2");
			Assert.AreEqual("position('a' in va2)", psf.Render(args, factoryImpl));

			args.Clear();
			args.Add("'a'");
			args.Add("va2");
			args.Add("2");
			Assert.AreEqual("(position('a' in substring(va2, 2))+2-1)", psf.Render(args, factoryImpl));
		}

		[Test]
		public void ClassicSum()
		{
			//ANSI-SQL92 definition
			//<general set function> ::=
			//<set function type> <leftparen> [ <setquantifier> ] <value expression> <right paren>
			//<set function type> : := AVG | MAX | MIN | SUM | COUNT
			//<setquantifier> ::= DISTINCT | ALL
			IList args = new ArrayList();

			ClassicSumFunction csf = new ClassicSumFunction();
			args.Add("va1");
			Assert.AreEqual("sum(va1)", csf.Render(args, factoryImpl));

			args.Clear();
			args.Add("distinct");
			args.Add("va2");
			Assert.AreEqual("sum(distinct va2)", csf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			try
			{
				string t = csf.Render(args, factoryImpl);
				Assert.Fail("No exception 2 argument without <setquantifier>:" + t);
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void ClassicCount()
		{
			//ANSI-SQL92 definition
			//COUNT < leftparen> <asterisk> < right paren>
			IList args = new ArrayList();

			ClassicCountFunction ccf = new ClassicCountFunction();
			args.Add("va1");
			Assert.AreEqual("count(va1)", ccf.Render(args, factoryImpl));

			args.Clear();
			args.Add("*");
			Assert.AreEqual("count(*)", ccf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			try
			{
				string t = ccf.Render(args, factoryImpl);
				Assert.Fail("No exception 2 argument without <setquantifier>:" + t);
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void ClassicAvg()
		{
			//ANSI-SQL92 definition
			//<general set function> ::=
			//<set function type> <leftparen> [ <setquantifier> ] <value expression> <right paren>
			//<set function type> : := AVG | MAX | MIN | SUM | COUNT
			//<setquantifier> ::= DISTINCT | ALL
			IList args = new ArrayList();

			ClassicAvgFunction caf = new ClassicAvgFunction();
			args.Add("va1");
			Assert.AreEqual("avg(va1)", caf.Render(args, factoryImpl));

			args.Clear();
			args.Add("distinct");
			args.Add("va2");
			Assert.AreEqual("avg(distinct va2)", caf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			try
			{
				string t = caf.Render(args, factoryImpl);
				Assert.Fail("No exception 2 argument without <setquantifier>:" + t);
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void ClassicAggregate()
		{
			IList args = new ArrayList();

			ClassicAggregateFunction caf = new ClassicAggregateFunction("max",false);
			args.Add("va1");
			Assert.AreEqual("max(va1)", caf.Render(args, factoryImpl));

			args.Clear();
			args.Add("distinct");
			args.Add("va2");
			Assert.AreEqual("max(distinct va2)", caf.Render(args, factoryImpl));

			args.Clear();
			args.Add("va1");
			args.Add("va2");
			try
			{
				string t = caf.Render(args, factoryImpl);
				Assert.Fail("No exception 2 argument without <setquantifier>:" + t);
			}
			catch (QueryException)
			{
				//correct
			}

			args.Clear();
			args.Add("*");
			try
			{
				string t = caf.Render(args, factoryImpl);
				Assert.Fail("No exception '*' :" + t);
			}
			catch (QueryException)
			{
				//correct
			}
		}

		[Test]
		public void AnsiSubstring()
		{
			//ANSI-SQL92 definition
			// <character substring function> ::=
			// SUBSTRING <left paren> <character value expression> FROM < start position>
			// [ FOR <string length> ] <right paren>
			IList args = new ArrayList();

			AnsiSubstringFunction asf = new AnsiSubstringFunction();
			args.Add("var1");
			args.Add("3");
			Assert.AreEqual("substring(var1 from 3)", asf.Render(args, factoryImpl));

			args.Add("4");
			Assert.AreEqual("substring(var1 from 3 for 4)", asf.Render(args, factoryImpl));

			args.Clear();
			try
			{
				string t = asf.Render(args, factoryImpl);
				Assert.Fail("Not threw 'Not enough parameters' exception:" + t);
			}
			catch (QueryException)
			{
				//correct
			}
			args.Clear();
			args.Add("1");
			args.Add("2");
			args.Add("3");
			args.Add("4");
			try
			{
				string t = asf.Render(args, factoryImpl);
				Assert.Fail("Not threw 'Not enough parameters' exception:" + t);
			}
			catch (QueryException)
			{
				//correct
			}
		}
	}
}
