using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl {
	
	internal class FilterImpl : QueryImpl {
		private object collection;

		public FilterImpl(string queryString, object collection, ISessionImplementor session) : base(queryString, session) {
			this.collection = collection;
		}

		public override IEnumerable Enumerable() {
			IDictionary namedParams = new Hashtable( NamedParams );
			string query = BindParameterLists( namedParams );
			return Session.EnumerableFilter( collection, query, ValueArray(), TypeArray(), Selection, namedParams);
		}

		public override IList List() {
			IDictionary namedParams = new Hashtable( NamedParams );
			string query = BindParameterLists( namedParams );
			return Session.Filter( collection, query, ValueArray(), TypeArray(), Selection, namedParams);
		}

		private IType[] TypeArray() {
			IList typeList = Types;
			int size = typeList.Count;
			IType[] result = new IType[size+1];
			for (int i=0; i<size; i++)
				result[i+1] = (IType) typeList[i];
			return result;
		}

		private object[] ValueArray() {
			IList valueList = Values;
			int size = valueList.Count;
			object[] result = new object[size+1];
			for (int i=0; i<size; i++)
				result[i+1] = valueList[i];
			return result;
		}
	}
}
