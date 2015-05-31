using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Linq;

namespace NHibernate.Impl
{
    public class CollectionFilterQueryImpl : AbstractQueryImpl2
    {
        public NhLinqExpression LinqExpression { get; private set; }

        public CollectionFilterQueryImpl(NhLinqExpression linqExpression, string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata)
            : base(queryString, flushMode, session, parameterMetadata)
        {
            LinqExpression = linqExpression;
        }

        protected override IQueryExpression ExpandParameters(IDictionary<string, TypedValue> namedParamsCopy)
        {
            throw new NotImplementedException();
        }
    }
}
