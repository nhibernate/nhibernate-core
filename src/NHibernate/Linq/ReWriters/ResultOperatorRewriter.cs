namespace NHibernate.Linq.ReWriters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NHibernate.Linq.Visitors;

    using Remotion.Data.Linq;
    using Remotion.Data.Linq.Clauses;
    using Remotion.Data.Linq.Clauses.Expressions;
    using Remotion.Data.Linq.Clauses.ResultOperators;
    using Remotion.Data.Linq.Clauses.StreamedData;
    using Remotion.Data.Linq.EagerFetching;

    /// <summary>
    /// Removes various result operators from a query so that they can be processed at the same
    /// tree level as the query itself.
    /// </summary>
    public class ResultOperatorRewriter : QueryModelVisitorBase
    {
        private readonly List<ResultOperatorBase> resultOperators = new List<ResultOperatorBase>();
        private IStreamedDataInfo evaluationType;

        private ResultOperatorRewriter()
        {
        }

        public static ResultOperatorRewriterResult Rewrite(QueryModel queryModel)
        {
            ResultOperatorRewriter rewriter = new ResultOperatorRewriter();

            rewriter.VisitQueryModel(queryModel);

            return new ResultOperatorRewriterResult(rewriter.resultOperators, rewriter.evaluationType);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            base.VisitMainFromClause(fromClause, queryModel);

            ResultOperatorExpressionRewriter rewriter = new ResultOperatorExpressionRewriter();
            fromClause.TransformExpressions(rewriter.Rewrite);
            if (fromClause.FromExpression.NodeType == ExpressionType.Constant)
            {
                System.Type expressionType = queryModel.MainFromClause.FromExpression.Type;
                if (expressionType.IsGenericType && expressionType.GetGenericTypeDefinition() == typeof(NhQueryable<>))
                {
                    queryModel.MainFromClause.ItemType = expressionType.GetGenericArguments()[0];
                }
            }

            this.resultOperators.AddRange(rewriter.ResultOperators);
            this.evaluationType = rewriter.EvaluationType;
        }

        /// <summary>
        /// Rewrites expressions so that they sit in the outermost portion of the query.
        /// </summary>
        private class ResultOperatorExpressionRewriter : NhExpressionTreeVisitor
        {
            private static readonly System.Type[] rewrittenTypes = new[]
                {
                    typeof(FetchRequestBase),
                    typeof(OfTypeResultOperator),
                    typeof(CacheableResultOperator),
                };

            private readonly List<ResultOperatorBase> resultOperators = new List<ResultOperatorBase>();
            private IStreamedDataInfo evaluationType;

            /// <summary>
            /// Gets an <see cref="IEnumerable{T}" /> of <see cref="ResultOperatorBase" /> that were rewritten.
            /// </summary>
            public IEnumerable<ResultOperatorBase> ResultOperators
            {
                get { return this.resultOperators; }
            }

            /// <summary>
            /// Gets the <see cref="IStreamedDataInfo" /> representing the type of data that the operator works upon.
            /// </summary>
            public IStreamedDataInfo EvaluationType
            {
                get { return this.evaluationType; }
            }

            public Expression Rewrite(Expression expression)
            {
                Expression rewrittenExpression = this.VisitExpression(expression);

                return rewrittenExpression;
            }

            protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
            {
                this.resultOperators.AddRange(
                    expression.QueryModel.ResultOperators
                        .Where(r => rewrittenTypes.Any(t => t.IsAssignableFrom(r.GetType()))));

                this.resultOperators.ForEach(f => expression.QueryModel.ResultOperators.Remove(f));
                this.evaluationType = expression.QueryModel.SelectClause.GetOutputDataInfo();

                if (expression.QueryModel.ResultOperators.Count == 0)
                {
                    return expression.QueryModel.MainFromClause.FromExpression;
                }

                return base.VisitSubQueryExpression(expression);
            }
        }
    }
}
