using System.Collections.Generic;
using System.Linq;

using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader.Custom.Sql
{
	/// <summary> Implements Hibernate's built-in support for native SQL queries. </summary>
	///<remarks>This support is built on top of the notion of "custom queries"...</remarks>
	public class SQLCustomQuery : ICustomQuery
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (SQLCustomQuery));

		private readonly List<IReturn> customQueryReturns = new List<IReturn>();
		private readonly ISet<string> querySpaces = new HashSet<string>();
		private readonly SqlString sql;
		private readonly List<IParameterSpecification> parametersSpecifications;

		public SQLCustomQuery(INativeSQLQueryReturn[] queryReturns, string sqlQuery, ICollection<string> additionalQuerySpaces,
		                      ISessionFactoryImplementor factory)
		{
			log.Debug("starting processing of sql query [" + sqlQuery + "]");
			SQLQueryReturnProcessor processor = new SQLQueryReturnProcessor(queryReturns, factory);
			SQLQueryReturnProcessor.ResultAliasContext aliasContext = processor.Process();

			SQLQueryParser parser = new SQLQueryParser(factory, sqlQuery, new ParserContext(aliasContext));
			sql = parser.Process();
			ArrayHelper.AddAll(customQueryReturns, processor.GenerateCustomReturns(parser.QueryHasAliases));
			parametersSpecifications = parser.CollectedParametersSpecifications.ToList();

			if (additionalQuerySpaces != null)
			{
				querySpaces.UnionWith(additionalQuerySpaces);
			}
		}

		public IEnumerable<IParameterSpecification> CollectedParametersSpecifications
		{
			get { return parametersSpecifications; }
		}

		#region ICustomQuery Members

		public SqlString SQL
		{
			get { return sql; }
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public IList<IReturn> CustomQueryReturns
		{
			get { return customQueryReturns; }
		}

		#endregion

		private class ParserContext : SQLQueryParser.IParserContext
		{
			private readonly SQLQueryReturnProcessor.ResultAliasContext aliasContext;

			public ParserContext(SQLQueryReturnProcessor.ResultAliasContext aliasContext)
			{
				this.aliasContext = aliasContext;
			}

			#region IParserContext Members

			public bool IsEntityAlias(string alias)
			{
				return GetEntityPersisterByAlias(alias) != null;
			}

			public ISqlLoadable GetEntityPersisterByAlias(string alias)
			{
				return aliasContext.GetEntityPersister(alias);
			}

			public string GetEntitySuffixByAlias(string alias)
			{
				return aliasContext.GetEntitySuffix(alias);
			}

			public bool IsCollectionAlias(string alias)
			{
				return GetCollectionPersisterByAlias(alias) != null;
			}

			public ISqlLoadableCollection GetCollectionPersisterByAlias(string alias)
			{
				return aliasContext.GetCollectionPersister(alias);
			}

			public string GetCollectionSuffixByAlias(string alias)
			{
				return aliasContext.GetCollectionSuffix(alias);
			}

			public IDictionary<string,string[]> GetPropertyResultsMapByAlias(string alias)
			{
				return aliasContext.GetPropertyResultsMap(alias);
			}

			#endregion
		}
	}
}