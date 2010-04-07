using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessOfType : IResultOperatorProcessor<OfTypeResultOperator>
    {
        public void Process(OfTypeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            var source =
                queryModelVisitor.CurrentEvaluationType.As<StreamedSequenceInfo>().ItemExpression;

            var type = BuildDot(resultOperator.SearchedItemType.FullName.Split('.'), tree.TreeBuilder);

            tree.AddWhereClause(tree.TreeBuilder.Equality(
                tree.TreeBuilder.Dot(
                    HqlGeneratorExpressionTreeVisitor.Visit(source, queryModelVisitor.VisitorParameters).AsExpression(),
                    tree.TreeBuilder.Class()),
                tree.TreeBuilder.Ident(resultOperator.SearchedItemType.FullName)));
        }

        private static HqlExpression BuildDot(IEnumerable<string> split, HqlTreeBuilder builder)
        {
            if (split.Count() == 1)
            {
                return builder.Ident(split.First());
            }

            return builder.Dot(builder.Ident(split.First()), BuildDot(split.Skip(1), builder));
        }
    }
}