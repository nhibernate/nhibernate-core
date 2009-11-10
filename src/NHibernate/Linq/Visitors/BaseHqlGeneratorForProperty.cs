using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq.Visitors
{
    public abstract class BaseHqlGeneratorForProperty : IHqlGeneratorForProperty
    {
        public IEnumerable<MemberInfo> SupportedProperties { get; protected set; }
        public abstract void BuildHql(MemberInfo member, Expression expression, HqlGeneratorExpressionTreeVisitor hqlGeneratorExpressionTreeVisitor);
    }
}