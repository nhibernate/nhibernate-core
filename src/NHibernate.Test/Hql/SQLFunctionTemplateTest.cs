using System;
using System.Collections;
using System.Globalization;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	[TestFixture]
	public class SQLFunctionTemplateTest : BaseFunctionFixture
	{
		[Test]
		public void Simple()
		{
			SQLFunctionTemplate ft = new SQLFunctionTemplate(NHibernateUtil.String, "ltrim( ?1 )");
			Assert.IsTrue(ft.HasArguments);
			IList args = new ArrayList();
			args.Add("'abcd   <'");
			Assert.AreEqual("ltrim( 'abcd   <' )", ft.Render(args, factoryImpl).ToString());

			ft = new SQLFunctionTemplate(NHibernateUtil.String, "ltrim( Az?ab )");
			Assert.IsFalse(ft.HasArguments);
			Assert.AreEqual("ltrim( Az?ab )", ft.Render(args, factoryImpl).ToString());

			ft = new SQLFunctionTemplate(NHibernateUtil.String, "function( ?1 )? 5:6");
			Assert.IsTrue(ft.HasArguments);
			Assert.AreEqual("function( 'abcd   <' )? 5:6", ft.Render(args, factoryImpl).ToString());

			ft = new SQLFunctionTemplate(NHibernateUtil.String, "????????1?");
			Assert.IsTrue(ft.HasArguments);
			Assert.AreEqual("???????'abcd   <'?", ft.Render(args, factoryImpl).ToString());
		}

		[Test]
		public void RepetedParams()
		{
			SQLFunctionTemplate ft;
			IList args = new ArrayList();

			ft =
				new SQLFunctionTemplate(NHibernateUtil.String,
				                        "replace( replace( rtrim( replace( replace( ?1, ' ', '${space}$' ), ?2, ' ' ) ), ' ', ?2 ), '${space}$', ' ' )");
			args.Add("'param1  '");
			args.Add("'param2 ab '");
			Assert.AreEqual(
				"replace( replace( rtrim( replace( replace( 'param1  ', ' ', '${space}$' ), 'param2 ab ', ' ' ) ), ' ', 'param2 ab ' ), '${space}$', ' ' )",
				ft.Render(args, factoryImpl).ToString());

			args.Clear();
			ft = new SQLFunctionTemplate(NHibernateUtil.String, "?1 ?3 ?2 ?3 ?1");
			args.Add(1);
			args.Add(2);
			args.Add(3);
			Assert.AreEqual("1 3 2 3 1", ft.Render(args, factoryImpl).ToString());
		}

		//[Test] not required
		public void NoStringArguments()
		{
			SQLFunctionTemplate ft;
			IList args = new ArrayList();

			ft = new SQLFunctionTemplate(NHibernateUtil.String, "?1 ?2 ?3");
			args.Add(DateTime.Today);
			args.Add(125.6D);
			args.Add(0910.123456m);
			string expected = string.Format("{0} {1} {2}",
			                                DateTime.Today.ToString(DateTimeFormatInfo.InvariantInfo),
			                                (125.6D).ToString(NumberFormatInfo.InvariantInfo),
			                                (0910.123456m).ToString(NumberFormatInfo.InvariantInfo));
			Assert.AreEqual(expected, ft.Render(args, factoryImpl).ToString());
		}

		[Test]
		public void ArgsDiffParams()
		{
			SQLFunctionTemplate ft;
			IList args = new ArrayList();

			// No Args; 2 params
			ft = new SQLFunctionTemplate(NHibernateUtil.String, "func(?1,?2)");
			Assert.AreEqual("func(,)", ft.Render(args, factoryImpl).ToString());

			// Args<params
			args.Clear();
			ft = new SQLFunctionTemplate(NHibernateUtil.String, "func(?1,?2)");
			args.Add(1);
			Assert.AreEqual("func(1,)", ft.Render(args, factoryImpl).ToString());

			// Args>params
			args.Clear();
			ft = new SQLFunctionTemplate(NHibernateUtil.String, "func(?1,?3)");
			args.Add(1);
			args.Add(2);
			args.Add(3);
			Assert.AreEqual("func(1,3)", ft.Render(args, factoryImpl).ToString());

			// Args in different position
			args.Clear();
			ft = new SQLFunctionTemplate(NHibernateUtil.String, "func(?2, ?1, ?3)");
			args.Add(1);
			args.Add(2);
			args.Add(3);
			Assert.AreEqual("func(2, 1, 3)", ft.Render(args, factoryImpl).ToString());
		}
	}
}
