using System;
using System.Collections;
using NHibernate.Hql.Ast.ANTLR;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Loader.Hql;
using NHibernate.Util;

namespace NHibernate.Test.BulkManipulation
{
	public class BaseFixture: TestCase
	{
		private readonly IDictionary<string, IFilter> emptyfilters = CollectionHelper.EmptyDictionary<string, IFilter>();

		#region Overrides of TestCase

		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
		}

		#endregion

		protected override void Configure(Cfg.Configuration configuration)
		{
			var assembly = GetType().Assembly;
			string mappingNamespace = GetType().Namespace;
			foreach (var resource in assembly.GetManifestResourceNames())
			{
				if (resource.StartsWith(mappingNamespace) && resource.EndsWith(".hbm.xml"))
				{
					configuration.AddResource(resource, assembly);
				}
			}
		}

		public string GetSql(string query)
		{
			var qt = new QueryTranslatorImpl(null, 
			                                 new HqlParseEngine(query, false, Sfi).Parse(), 
			                                 emptyfilters, 
			                                 Sfi, 
			                                 CreateQueryLoader);
			qt.Compile(null, false);
			return qt.SQLString;
		}
		
		private static IQueryLoader CreateQueryLoader(QueryTranslatorImpl queryTranslatorImpl, 
		                                              ISessionFactoryImplementor sessionFactoryImplementor,
		                                              SelectClause selectClause)
		{
			return new QueryLoader(queryTranslatorImpl,
			                       sessionFactoryImplementor,
			                       selectClause);
		}
	}
}
