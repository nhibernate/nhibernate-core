using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// The WhereJoinDetector creates the joins for the where clause, including
    /// optimizations for inner joins. The algorithms are explained in a text
    /// attached to JIRA entry NH-2583.
    /// </summary>
    internal class WhereJoinDetector : AbstractJoinDetector
    {
        // Possible results of a condition when emptily outer joined.
        private const int T = 1;
        private const int N = 2;
        private const int TN = T | N;
        private const int F = 4;
        private const int TF = T | F;
        private const int NF = N | F;
        private const int TNF = T | N | F;

        // Composition rules for possible results when emptily outer joined
        // for &&, ||, !.
        private static readonly int[,] AND = new int[8, 8];
        private static readonly int[,] OR = new int[8, 8];
        private static readonly int[] NOT = new int[8];

        /// <summary>
        /// Setup of <see cref="AND"/>, <see cref="OR"/>, <see cref="NOT"/>.
        /// </summary>
        static WhereJoinDetector()
        {
            // Setup of simple values according to SQL 3-valued logic.
            NOT[T] = F;
            NOT[N] = N;
            NOT[F] = T;

            foreach (var p in new[] { T, N, F })
            {
                OR[p, p] = AND[p, p] = p;
                AND[p, F] = AND[F, p] = F;
                OR[p, T] = OR[T, p] = T;
            }
            AND[T, N] = AND[N, T] = N;
            OR[F, N] = OR[N, F] = N;

            // Setup of compound values. Idea: Split each
            // compound value to simple values, compute results
            // for simple values and or them together.
            var allValues = new[] { T, N, TN, F, TF, NF, TNF };

            // How compound values are split up into simple values.
            var split = new int[8][];
            split[T] = new[] { T };
            split[N] = new[] { N };
            split[TN] = new[] { T, N };
            split[F] = new[] { F };
            split[TF] = new[] { T, F };
            split[NF] = new[] { N, F };
            split[TNF] = new[] { T, N, F };

            foreach (var p in allValues)
            {
                int[] splitP = split[p];
                // We only need to compute NOT for compound values.
                if (splitP.Length > 1)
                {
                    int notResult = 0;
                    foreach (var p0 in splitP)
                    {
                        notResult |= NOT[p0];
                    }
                    NOT[p] = notResult;
                }
                foreach (var q in allValues)
                {
                    int[] splitQ = split[q];
                    // We must compute AND and OR if both values are compound,
                    // *but also* if one is compound and the other is simple
                    // (e.g. T and TNF).
                    if (splitP.Length > 1 || splitQ.Length > 1)
                    {
                        int andResult = 0;
                        int orResult = 0;
                        foreach (var p0 in splitP)
                        {
                            foreach (var q0 in splitQ)
                            {
                                andResult |= AND[p0, q0];
                                orResult |= OR[p0, q0];
                            }
                        }
                        AND[p, q] = andResult;
                        OR[p, q] = orResult;
                    }
                }
            }
        }

        // The following is used for all *condition* traversal (but not *expressions* that are not conditions).
        // This is the "mapping" described in the text at NH-2583.
        private Dictionary<string, int> _memberExpressionMapping = new Dictionary<string, int>();

        // The following two are used for member expressions traversal.
        private HashSet<string> _collectedPathMemberExpressionsInExpression = new HashSet<string>();
        private int _memberExpressionDepth = 0;

        internal
            WhereJoinDetector(NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
            : base(nameGenerator, isEntityDecider, joins, expressionMap)
        {
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);

            Expression baseResult = expression;
            if (expression.NodeType == ExpressionType.AndAlso && expression.Type == typeof(bool))
            {
                // Case (a) from the text at NH-2583.
                var newLeft = VisitExpression(expression.Left);

                var leftMapping = _memberExpressionMapping;

                var newRight = VisitExpression(expression.Right);

                var rightMapping = _memberExpressionMapping;

                _memberExpressionMapping = BinaryMapping(leftMapping, rightMapping, AND);

                // The following is copy-pasted from Relinq's visitor, as I had to split the code above.
                var newConversion = (LambdaExpression)VisitExpression(expression.Conversion);
                if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                    baseResult = Expression.MakeBinary(expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            }
            else if (expression.NodeType == ExpressionType.OrElse && expression.Type == typeof(bool))
            {
                // Case (b)
                var newLeft = VisitExpression(expression.Left);

                var leftMapping = _memberExpressionMapping;

                var newRight = VisitExpression(expression.Right);

                var rightMapping = _memberExpressionMapping;

                _memberExpressionMapping = BinaryMapping(leftMapping, rightMapping, OR);

                // Again, the following is copy-pasted from Relinq's visitor, as I had to split the code above.
                var newConversion = (LambdaExpression)VisitExpression(expression.Conversion);
                if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                    baseResult = Expression.MakeBinary(expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            }
            else if (expression.Type == typeof(bool)
                && (expression.NodeType == ExpressionType.Equal && !IsNullConstantExpression(expression.Right) && !IsNullConstantExpression(expression.Left)
                 || expression.NodeType == ExpressionType.NotEqual && !IsNullConstantExpression(expression.Right) && !IsNullConstantExpression(expression.Left)
                 || expression.NodeType == ExpressionType.LessThan
                 || expression.NodeType == ExpressionType.LessThanOrEqual
                 || expression.NodeType == ExpressionType.GreaterThan
                 || expression.NodeType == ExpressionType.GreaterThanOrEqual))
            {
                // Cases (e), (f).2, (g).2
                _collectedPathMemberExpressionsInExpression = new HashSet<string>();

                baseResult = base.VisitBinaryExpression(expression);

                _memberExpressionMapping = FixedMapping(_collectedPathMemberExpressionsInExpression, N);
            }
            else if (expression.Type == typeof(bool)
                && expression.NodeType == ExpressionType.NotEqual)
            {
                // Case (h)
                _collectedPathMemberExpressionsInExpression = new HashSet<string>();

                baseResult = base.VisitBinaryExpression(expression);

                _memberExpressionMapping = FixedMapping(_collectedPathMemberExpressionsInExpression, F);
            }
            else if (expression.Type == typeof(bool)
                && expression.NodeType == ExpressionType.Equal)
            {
                // Case (i)
                _collectedPathMemberExpressionsInExpression = new HashSet<string>();

                baseResult = base.VisitBinaryExpression(expression);

                _memberExpressionMapping = FixedMapping(_collectedPathMemberExpressionsInExpression, T);
            }
            else // +, * etc.
            {
                // Case (j)
                _collectedPathMemberExpressionsInExpression = new HashSet<string>();

                baseResult = base.VisitBinaryExpression(expression);

                _memberExpressionMapping = FixedMapping(_collectedPathMemberExpressionsInExpression, TNF);
            }
            return baseResult;
        }

        private static Dictionary<string, int> FixedMapping(IEnumerable<string> collectedPathMemberExpressionsInExpression, int value)
        {
            var memberExpressionMapping = new Dictionary<string, int>();
            foreach (var me in collectedPathMemberExpressionsInExpression)
            {
                memberExpressionMapping.Add(me, value);
            }
            return memberExpressionMapping;
        }

        private static Dictionary<string, int> BinaryMapping(Dictionary<string, int> leftMapping, Dictionary<string, int> rightMapping, int[,] op)
        {
            var result = new Dictionary<string, int>();
            // Compute mapping for all member expressions in leftMapping. If the member expression is missing
            // in rightMapping, use TNF as a "pessimistic approximation" instead (inside the ?: operator). See
            // the text for an explanation of this.
            foreach (var lhs in leftMapping)
            {
                result.Add(lhs.Key, op[lhs.Value, rightMapping.ContainsKey(lhs.Key) ? rightMapping[lhs.Key] : TNF]);
            }
            // Compute mapping for all member expressions *only* in rightMapping (we did the common ones above).
            // Again, use TNF as pessimistic approximation to result of left subcondition.
            foreach (var rhs in rightMapping)
            {
                if (!leftMapping.ContainsKey(rhs.Key))
                {
                    result[rhs.Key] = op[rhs.Value, TNF];
                }
            }
            return result;
        }

        private static bool IsNullConstantExpression(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                var constant = (ConstantExpression)expression;
                return constant.Value == null;
            }
            else
            {
                return false;
            }
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);

            Expression baseResult;
            if (expression.NodeType == ExpressionType.Not && expression.Type == typeof(bool))
            {
                // Case (c) from text at NH-2583.
                baseResult = VisitExpression(expression.Operand);

                var opMapping = _memberExpressionMapping;
                _memberExpressionMapping = new Dictionary<string, int>();

                foreach (var m in opMapping)
                {
                    _memberExpressionMapping.Add(m.Key, NOT[m.Value]);
                }
            }
            else
            {
                baseResult = base.VisitUnaryExpression(expression);
            }
            return baseResult;
        }

        //protected override Expression VisitConditionalExpression(ConditionalExpression expression)
        //{
        //    ArgumentUtility.CheckNotNull("expression", expression);

        //    VisitExpression(expression.Test);
        //    // If the ?: returns bool, it is (most probably ...) a condition which may require outer joins.
        //    // TODO: Check check whether HQL accepts ?: conditions; if not, should be rewritten it as (a && b || !a && c).
        //    if (expression.Type == typeof(bool))
        //    {
        //        ...
        //    }
        //    return expression;
        //}

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);

            Expression newExpression;
            try
            {
                _memberExpressionDepth++;
                newExpression = base.VisitExpression(expression.Expression);
            }
            finally
            {
                _memberExpressionDepth--;
            }
            bool isEntity = _isEntityDecider.IsEntity(expression.Type);
            
            if (isEntity)
            {
                // See (h) why we do not check for _memberExpressionDepth here!
                _collectedPathMemberExpressionsInExpression.Add(ExpressionKeyVisitor.Visit(expression, null));
            }

            if (_memberExpressionDepth > 0 && isEntity)
            {
                return AddJoin(expression);
            }
            else
            {
                if (newExpression != expression.Expression)
                    return Expression.MakeMemberAccess(newExpression, expression.Member);
                return expression;
            }
        }

        internal static void Find(Expression expression, NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
        {
            WhereJoinDetector f = new WhereJoinDetector(nameGenerator, isEntityDecider, joins, expressionMap);

            f.VisitExpression(expression);

            foreach (var mapping in f._memberExpressionMapping)
            {
                // If outer join can never produce true, we can safely inner join.
                if ((mapping.Value & T) == 0)
                {
                    f.MakeInnerIfJoined(mapping.Key);
                }
            }
        }

    }
}
