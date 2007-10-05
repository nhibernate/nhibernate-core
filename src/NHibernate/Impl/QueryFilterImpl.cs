using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="IQuery"/> interface for collection filters.
	/// </summary>
	public class QueryFilterImpl : QueryImpl
	{
		private object collection;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		public QueryFilterImpl(string queryString, object collection, ISessionImplementor session)
			: base(queryString, FlushMode.Unspecified, session)
		{
			this.collection = collection;
		}

		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.EnumerableFilter(collection, BindParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.EnumerableFilter<T>(collection, BindParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.ListFilter(collection, BindParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.ListFilter<T>(collection, BindParameterLists(namedParams), GetQueryParameters(namedParams));
		}

		public override IType[] TypeArray()
		{
			IList typeList = Types;
			int size = typeList.Count;
			IType[] result = new IType[size + 1];
			for (int i = 0; i < size; i++)
			{
				result[i + 1] = (IType) typeList[i];
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
