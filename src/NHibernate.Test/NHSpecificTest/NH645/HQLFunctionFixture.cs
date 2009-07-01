using System;
using System.Collections;
using Antlr.Runtime.Tree;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Classic;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH645
{
	[TestFixture]
	public class HqlFunctionWithClassicParser : HQLFunctionFixtureBase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.QueryTranslator, typeof(ClassicQueryTranslatorFactory).AssemblyQualifiedName);
		}
	}

	[TestFixture, Ignore("Not fixed yet in the AST-HQL parser")]
	public class HqlFunctionWithAstHqlParser : HQLFunctionFixtureBase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.QueryTranslator, typeof(ASTQueryTranslatorFactory).AssemblyQualifiedName);
		}
	}

	public abstract class HQLFunctionFixtureBase : TestCase
	{
		private bool appliesToThisDialect = true;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "HQL.Animal.hbm.xml", "HQL.MaterialResource.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return appliesToThisDialect;
		}

		protected override void Configure(Configuration configuration)
		{
			if (Dialect is MsSql2005Dialect)
				configuration.SetProperty(Environment.Dialect, typeof(CustomDialect).AssemblyQualifiedName);
			else
				appliesToThisDialect = false;
		}

		public void Run(string hql)
		{
			using (ISession s = OpenSession())
				try
				{
					s.CreateQuery(hql).List();
				}
				catch (Exception ex)
				{
					if (IsClassicParser)
					{
						if (ex is QueryException)
							Assert.Fail("The parser think that 'freetext' is a boolean function");
					}
					else //Hql-Parser
					{
						if (ex is RewriteEmptyStreamException || ex is InvalidCastException)
							Assert.Fail("The parser think that 'freetext' is a boolean function");
					}
				}
		}

		/// <summary>
		/// Just test the parser can compile, and SqlException is expected.
		/// </summary>
		[Test]
		public void SimpleWhere()
		{
			Run("from Animal a where freetext(a.Description, 'hey apple car')");
		}

		[Test]
		public void SimpleWhereWithAnotherClause()
		{
			Run("from Animal a where freetext(a.Description, 'hey apple car') AND 1 = 1");
		}

		[Test]
		public void SimpleWhereWithAnotherClause2()
		{
			Run("from Animal a where freetext(a.Description, 'hey apple car') AND a.Description <> 'foo'");
		}
	}

	public class CustomDialect : MsSql2005Dialect
	{
		public CustomDialect()
		{
			RegisterFunction("freetext", new SQLFunctionTemplate(null, "freetext($1,$2)"));
		}
	}
}