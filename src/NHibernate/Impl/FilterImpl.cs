using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl {
	
	public class FilterImpl : QueryImpl {
		private object collection;

		public FilterImpl(string queryString, object collection, ISessionImplementor session) : base(queryString, session) {
			this.collection = collection;
		}

		public override IEnumerable Enumerable() {
			Values[0] = null;
			Types[0] = null;
			return Session.EnumerableFilter(collection, QueryString, (object[]) Values.ToArray(typeof(object)), (IType[]) Types.ToArray(typeof(IType)), Selection, NamedParams );
		}

		public override IList List() {
			Values[0] = null;
			Types[0] = null;
			return Session.Filter(collection, QueryString, (object[]) Values.ToArray(typeof(object)), (IType[]) Types.ToArray(typeof(IType)), Selection, NamedParams );
		}
	}
}
