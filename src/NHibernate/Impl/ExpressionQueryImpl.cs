using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;

namespace NHibernate.Impl
{
    class ExpressionQueryImpl : AbstractQueryImpl
    {
        private readonly Dictionary<string, LockMode> _lockModes = new Dictionary<string, LockMode>(2);

        public IQueryExpression QueryExpression { get; private set; }

        public ExpressionQueryImpl(IQueryExpression queryExpression, ISessionImplementor session, ParameterMetadata parameterMetadata) 
            : base(queryExpression.Key, FlushMode.Unspecified, session, parameterMetadata)
        {
            QueryExpression = queryExpression;
        }
        
        public override IQuery SetLockMode(string alias, LockMode lockMode)
        {
            _lockModes[alias] = lockMode;
            return this;
        }

        protected internal override IDictionary<string, LockMode> LockModes
        {
            get { return _lockModes; }
        }

        public override int ExecuteUpdate()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable Enumerable()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<T> Enumerable<T>()
        {
            throw new NotImplementedException();
        }

        public override IList List()
        {
            VerifyParameters();
            IDictionary<string, TypedValue> namedParams = NamedParams;
            Before();
            try
            {
                return Session.List(QueryExpression, GetQueryParameters(namedParams));
            }
            finally
            {
                After();
            }
        }

        public override void List(IList results)
        {
            throw new NotImplementedException();
        }

        public override IList<T> List<T>()
        {
            throw new NotImplementedException();
        }
    }
}