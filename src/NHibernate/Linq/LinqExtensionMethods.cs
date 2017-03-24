using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
		public static Int32 Delete<T>(this ISession session, Expression<Func<T, Boolean>> condition)
		{
			//these could be cached as static readonly fields
			var instanceBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var staticBindingFlags = BindingFlags.Public | BindingFlags.Static;
			var selectMethod = typeof(Queryable).GetMethods(staticBindingFlags).First(x => x.Name == "Select");
			var whereMethod = typeof(Queryable).GetMethods(staticBindingFlags).First(x => x.Name == "Where");
			var translatorFactory = new ASTQueryTranslatorFactory();
			var aliasRegex = new Regex(" from (\\w+) (\\w+) ");
			var parameterTokensRegex = new Regex("\\?");

			var entityType = typeof(T);
			var queryable = session.Query<T>();
			var sessionImpl = session.GetSessionImplementation();
			var persister = sessionImpl.GetEntityPersister(entityType.FullName, null);
			var idName = persister.IdentifierPropertyName;
			var idType = persister.IdentifierType.ReturnedClass;
			var idProperty = entityType.GetProperty(idName, instanceBindingFlags);
			var idMember = idProperty as MemberInfo;

			if (idProperty == null)
			{
				var fieldEntityType = entityType;

				//if the property is null, it means the the id is implemented as a field
				while ((fieldEntityType != typeof(Object)) && (idMember == null))
				{
					//try to find the field recursively
					idMember = fieldEntityType.GetField(idName, instanceBindingFlags);

					fieldEntityType = fieldEntityType.BaseType;
				}
			}

			if (idMember == null)
			{
				throw new InvalidOperationException(string.Format("Could not find identity property {0} in entity {1}.", idName, entityType.FullName));
			}

			var delegateType = typeof(Func<,>).MakeGenericType(entityType, idType);
			var parm = Expression.Parameter(entityType, "x");
			var lambda = Expression.Lambda(delegateType, Expression.MakeMemberAccess(parm, idMember), new ParameterExpression[] { parm });
			var where = Expression.Call(null, whereMethod.MakeGenericMethod(entityType), queryable.Expression, condition);
			var call = Expression.Call(null, selectMethod.MakeGenericMethod(entityType, idType), where, lambda);

			var nhLinqExpression = new NhLinqExpression(call, sessionImpl.Factory);
			var translator = translatorFactory.CreateQueryTranslators(nhLinqExpression, null, false, sessionImpl.EnabledFilters, sessionImpl.Factory).Single();
			var parameters = nhLinqExpression.ParameterValuesByName.Select(x => x.Value.Item1).ToArray();
			//we need to turn positional parameters into named parameters because of SetParameterList
			var count = 0;
			var replacedSql = parameterTokensRegex.Replace(translator.SQLString, m => ":p" + count++);
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
					throw ADOExceptionHelper.Convert(sessionImpl.Factory.SQLExceptionConverter, ex, "Error deleting records.", new SqlString(sql.ToString()), parameters, nhLinqExpression.ParameterValuesByName.ToDictionary(x => x.Key, x => new TypedValue(x.Value.Item2, x.Value.Item1, session.ActiveEntityMode)));
				}
			}
		}

		public static IQueryable<T> Query<T>(this ISession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this ISession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => Cacheable<object>(null)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression);

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => CacheMode<object>(null, NHibernate.CacheMode.Normal)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(cacheMode));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => CacheRegion<object>(null, null)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(region));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> Timeout<T>(this IQueryable<T> query, int timeout)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => Timeout<object>(null, 0)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(timeout));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IEnumerable<T> ToFuture<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			return (IEnumerable<T>) future;
		}

		public static IFutureValue<T> ToFutureValue<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			if (future is IEnumerable<T>)
			{
				return new FutureValue<T>(() => ((IEnumerable<T>) future));
			}

			return (IFutureValue<T>) future;
		}

		public static T MappedAs<T>(this T parameter, IType type)
		{
			throw new InvalidOperationException("The method should be used inside Linq to indicate a type of a parameter");
		}

		public static IFutureValue<TResult> ToFutureValue<T, TResult>(this IQueryable<T> query, Expression<Func<IQueryable<T>, TResult>> selector)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) query.Provider;

			var expression = ReplacingExpressionTreeVisitor.Replace(selector.Parameters.Single(),
																	query.Expression,
																	selector.Body);

			return (IFutureValue<TResult>) provider.ExecuteFuture(expression);
		}
	}
}
