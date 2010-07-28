using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;

namespace NHibernate.Linq.Visitors
{
    public class VisitorParameters
    {
        public ISessionFactoryImplementor SessionFactory { get; private set; }
        public IDictionary<ConstantExpression, NamedParameter> ConstantToParameterMap { get; private set; }
        public List<NamedParameterDescriptor> RequiredHqlParameters { get; private set; }

				public VisitorParameters(ISessionFactoryImplementor sessionFactory, IDictionary<ConstantExpression, NamedParameter> constantToParameterMap, List<NamedParameterDescriptor> requiredHqlParameters)
        {
            SessionFactory = sessionFactory;
            ConstantToParameterMap = constantToParameterMap;
            RequiredHqlParameters = requiredHqlParameters;
        }
    }
}