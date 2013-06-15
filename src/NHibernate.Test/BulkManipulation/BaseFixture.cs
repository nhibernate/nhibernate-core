using System.Collections;
using NHibernate.Hql.Ast.ANTLR;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Test.BulkManipulation
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
			var qt = new QueryTranslatorImpl(null, new HqlParseEngine(query, false, sessions).Parse(), emptyfilters, sessions);
			qt.Compile(null, false);
			return qt.SQLString;
		}
	}
}