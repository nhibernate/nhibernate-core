using System.Collections;
using System.Collections.Generic;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Util;

namespace NHibernate.Test.Hql.Ast
{
	public class BaseFixture: TestCase
	{
		private readonly IDictionary<string, IFilter> emptyfilters = new CollectionHelper.EmptyMapClass<string, IFilter>();
		
		#region Overrides of TestCase

		protected override IList Mappings
		{
			get { return new string[0]; }
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
			return GetSql(query, null);
		}

		public string GetSql(string query, IDictionary<string, string> replacements)
		{
			var qt = new QueryTranslatorImpl(null, new HqlParseEngine(query, false, Sfi).Parse(), emptyfilters, Sfi);
			qt.Compile(replacements, false);
			return qt.SQLString;
		}
	}
}