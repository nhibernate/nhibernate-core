using System.Collections;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="IQuery"/> interface for collection filters.
	/// </summary>
	public class CollectionFilterImpl : QueryImpl
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
