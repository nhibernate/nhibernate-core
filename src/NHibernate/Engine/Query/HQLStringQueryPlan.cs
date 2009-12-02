using System;
using System.Collections.Generic;
using NHibernate.Hql;

namespace NHibernate.Engine.Query
{
    [Serializable]
    public class HQLStringQueryPlan : HQLQueryPlan
    {
        public HQLStringQueryPlan(string hql, bool shallow, 
                                  IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : this(hql, (string) null, shallow, enabledFilters, factory)
        {
        }

        protected internal HQLStringQueryPlan(string hql, string collectionRole, bool shallow,
                                              IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : base(hql, CreateTranslators(hql, collectionRole, shallow, enabledFilters, factory))
        {
        }

        private static IQueryTranslator[] CreateTranslators(string hql, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
        {
            return factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(hql, collectionRole, shallow, enabledFilters, factory);
        }
    }
}