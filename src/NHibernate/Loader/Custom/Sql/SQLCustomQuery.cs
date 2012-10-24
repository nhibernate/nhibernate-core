using System.Collections.Generic;
using System.Linq;

using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Param;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Custom.Sql
{
	/// <summary> Implements Hibernate's built-in support for native SQL queries. </summary>
	///<remarks>This support is built on top of the notion of "custom queries"...</remarks>
	public class SQLCustomQuery : ICustomQuery
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof (SQLCustomQuery));

		private readonly List<IReturn> customQueryReturns;
		private readonly ISet<string> querySpaces = new HashSet<string>();
		private readonly SqlString sql;
		private readonly List<IParameterSpecification> parametersSpecifications;

		public SQLCustomQuery(INativeSQLQueryReturn[] queryReturns, string sqlQuery, ICollection<string> additionalQuerySpaces,
							  ISessionFactoryImplementor factory)
		{
			log.Debug("starting processing of sql query [{0}]", sqlQuery);
			var processor = new SQLQueryContext(queryReturns, factory);

			var parser = new SQLQueryParser(factory, sqlQuery, processor);
			this.sql = parser.Process();
			this.customQueryReturns = GenerateCustomReturns(queryReturns, parser.QueryHasAliases, processor).ToList();
			this.parametersSpecifications = parser.CollectedParametersSpecifications.ToList();

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

		#region private methods

		private static IEnumerable<IReturn> GenerateCustomReturns(IEnumerable<INativeSQLQueryReturn> queryReturns, bool queryHadAliases, SQLQueryContext context)
		{
			IDictionary<string, NonScalarReturn> customReturnsByAlias = new Dictionary<string, NonScalarReturn>();

			foreach (var nativeRtn in queryReturns)
			{
				var nativeScalarRtn = nativeRtn as NativeSQLQueryScalarReturn;
				if (nativeScalarRtn != null)
				{
					yield return new ScalarReturn(nativeScalarRtn.Type, nativeScalarRtn.ColumnAlias);
					continue;
				}

				var nativeJoinRtn = nativeRtn as NativeSQLQueryJoinReturn;
				if (nativeJoinRtn != null)
				{
					var owner = customReturnsByAlias[nativeJoinRtn.OwnerAlias];
					var fetchReturn = new NonScalarReturn(context, queryHadAliases, nativeJoinRtn.Alias, nativeJoinRtn.LockMode, owner);
					yield return customReturnsByAlias[fetchReturn.Alias] = fetchReturn;
					continue;
				}

				var nativeNonScalarRtn = nativeRtn as NativeSQLQueryNonScalarReturn;
				if (nativeNonScalarRtn != null)
				{
					var nonFetchReturn = new NonScalarReturn(context, queryHadAliases, nativeNonScalarRtn.Alias, nativeNonScalarRtn.LockMode);
					yield return customReturnsByAlias[nonFetchReturn.Alias] = nonFetchReturn;
				}
			}
		}

		#endregion
	}
}
