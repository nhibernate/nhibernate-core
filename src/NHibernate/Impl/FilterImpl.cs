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

		public override ICollection GetCollection() {
			Values[0] = null;
			Types[0] = null;
			return Session.FilterCollection(collection, QueryString, (object[]) Values.ToArray(typeof(object)), (IType[]) Types.ToArray(typeof(IType)), Selection, NamedParams );
		}

		public override IList GetList() {
			Values[0] = null;
			Types[0] = null;
			return Session.FilterList(collection, QueryString, (object[]) Values.ToArray(typeof(object)), (IType[]) Types.ToArray(typeof(IType)), Selection, NamedParams );
		}
	}
}
