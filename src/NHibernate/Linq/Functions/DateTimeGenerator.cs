using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    public class DateTimeGenerator : BaseHqlGeneratorForType
    {
        public DateTimeGenerator()
        {
            PropertyRegistry.Add(new DatePartGenerator());
        }

        public class DatePartGenerator : BaseHqlGeneratorForProperty
        {
            public DatePartGenerator()
            {
                SupportedProperties = new[]
                                          {
                                              ReflectionHelper.GetProperty((DateTime x) => x.Year),
                                              ReflectionHelper.GetProperty((DateTime x) => x.Month),
                                              ReflectionHelper.GetProperty((DateTime x) => x.Day),
                                              ReflectionHelper.GetProperty((DateTime x) => x.Hour),
                                              ReflectionHelper.GetProperty((DateTime x) => x.Minute),
                                              ReflectionHelper.GetProperty((DateTime x) => x.Second),
                                          };
            }

            public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.MethodCall(member.Name.ToLowerInvariant(),
                                              visitor.Visit(expression).AsExpression());
            }
        }
    }
}