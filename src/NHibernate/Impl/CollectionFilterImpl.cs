using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="IQuery"/> interface for collection filters.
	/// </summary>
	public partial class CollectionFilterImpl : QueryImpl
	{
		private readonly object collection;

		public CollectionFilterImpl(string queryString, object collection, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryString, session, parameterMetadata)
		{
			this.collection = collection;
		}

		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			return Session.EnumerableFilter(collection, ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			return Session.EnumerableFilter<T>(collection, ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		protected internal override IEnumerable<ITranslator> GetTranslators(ISessionImplementor session, QueryParameters queryParameters)
		{
			// NOTE: updates queryParameters.NamedParameters as (desired) side effect
			var queryExpression = ExpandParameters(queryParameters.NamedParameters);

			return GetTranslators(session, queryParameters, queryExpression, collection);
		}

		internal static IEnumerable<ITranslator> GetTranslators(ISessionImplementor session, QueryParameters queryParameters, IQueryExpression queryExpression, object collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "null collection passed to filter");

			//NOTE: SessionImpl.GetFilterQueryPlan might do flushing to support not saved collections and handles collection role changes (?)
			//It's not supported for Future queries

			var entry = session.PersistenceContext.GetCollectionEntryOrNull(collection);
			var persiter = entry?.LoadedPersister;
			if (persiter == null)
			{
				throw new QueryException("Not persistent collections are not supported");
			}

			var plan = session.Factory.QueryPlanCache.GetFilterQueryPlan(queryExpression, persiter.Role, false, session.EnabledFilters);
			if (queryParameters != null)
			{
				queryParameters.PositionalParameterValues[0] = entry.LoadedKey;
				queryParameters.PositionalParameterTypes[0] = entry.LoadedPersister.KeyType;
			}
			return plan.Translators.Select(t => new HqlTranslatorWrapper(t));
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			return Session.ListFilter(collection, ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			return Session.ListFilter<T>(collection, ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override void List(IList results)
		{
			ArrayHelper.AddAll(results, List());
		}

		public override IType[] TypeArray()
		{
			IList<IType> typeList = Types;
			int size = typeList.Count;
			IType[] result = new IType[size + 1];
			for (int i = 0; i < size; i++)
			{
				result[i + 1] = typeList[i];
			}
			return result;
		}

		public override object[] ValueArray()
		{
			IList valueList = Values;
			int size = valueList.Count;
			object[] result = new object[size + 1];
			for (int i = 0; i < size; i++)
			{
				result[i + 1] = valueList[i];
			}
			return result;
		}
	}
}
