using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Type;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessResultOperatorReturn
    {
        public HqlTreeNode TreeNode { get; set;}
        public Action<IQuery, IDictionary<string, Tuple<object, IType>>> AdditionalCriteria { get; set; }
        public LambdaExpression ListTransformer { get; set;}
        public LambdaExpression PostExecuteTransformer { get; set;}
        public HqlBooleanExpression WhereClause { get; set;}
        public HqlGroupBy GroupBy { get; set;}
        public HqlTreeNode AdditionalFrom { get; set; }
    }
}