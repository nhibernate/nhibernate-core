using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	public static class CriteriaExtensions
	{
		public static Int32 Delete(this IQueryOver queryOver)
		{
			return Delete(queryOver.UnderlyingCriteria);
		}

		public static Int32 Delete(this ICriteria criteria)
		{
			//NH-3735
			criteria.ClearOrders();
			criteria = criteria.SetProjection(Projections.Id());
			var criteriaImpl = criteria as CriteriaImpl;
			var sessionImpl = criteriaImpl.Session;
			var session = sessionImpl as ISession;
			var factory = sessionImpl.Factory;
			var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);
			var loader = new CriteriaLoader(factory.GetEntityPersister(implementors[0]) as IOuterJoinLoadable, factory, criteriaImpl, implementors[0], sessionImpl.EnabledFilters);
			var parameters = loader.Translator.GetQueryParameters().NamedParameters.Select(x => x.Value.Value).ToArray();

			
			var aliasRegex = new Regex(" from (\\w+) (\\w+) ", RegexOptions.IgnoreCase);
			var parameterTokensRegex = new Regex("\\?");
			//we need to turn positional parameters into named parameters because of SetParameterList
			var count = 0;
			var replacedSql = parameterTokensRegex.Replace(loader.SqlString.ToString(), m => ":p" + count++);
			var sql = new StringBuilder(replacedSql);
			//find from
			var fromIndex = sql.ToString().IndexOf(" from ", StringComparison.InvariantCultureIgnoreCase);
			//find alias
			var alias = aliasRegex.Match(sql.ToString()).Groups[2].Value;

			//make a string in the form DELETE alias FROM table alias WHERE condition
			sql.Remove(0, fromIndex);
			sql.Insert(0, string.Concat("delete ", alias, " "));

			using (var childSession = session.GetSession(session.ActiveEntityMode))
			{
				try
				{
					var query = childSession.CreateSQLQuery(sql.ToString());

					for (var i = 0; i < parameters.Length; ++i)
					{
						var parameter = parameters[i];

						if (!(parameter is IEnumerable) || (parameter is string) || (parameter is byte[]))
						{
							query.SetParameter(String.Format("p{0}", i), parameter);
						}
						else
						{
							query.SetParameterList(String.Format("p{0}", i), parameter as IEnumerable);
						}
					}

					return query.ExecuteUpdate();
				}
				catch (Exception ex)
				{
					throw ADOExceptionHelper.Convert(sessionImpl.Factory.SQLExceptionConverter, ex, "Error deleting records.", new SqlString(sql.ToString()), parameters, loader.Translator.GetQueryParameters().NamedParameters.ToDictionary(x => x.Key, x => x.Value));
				}
			}
		}
	}
}
