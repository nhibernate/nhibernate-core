// FIXME - Are there other things that can convert N into T?  What about the ?: operator?
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// The WhereJoinDetector creates the joins for the where clause, including
    /// optimizations for inner joins. The algorithms are explained in 
    /// the accompanying text file.
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
        private static readonly int[] ISNULL = new int[8];
        private static readonly int[] ISNOTNULL = new int[8];

        /// <summary>
        /// Setup of <see cref="AND"/>, <see cref="OR"/>, <see cref="NOT"/>.
        /// </summary>
        static WhereJoinDetector()
        {
            // Setup of simple values according to SQL 3-valued logic.
            NOT[T] = F;
            NOT[N] = N;
            NOT[F] = T;
            ISNULL[T] = F;
            ISNULL[N] = T;
            ISNULL[F] = F;
            ISNOTNULL[T] = T;
            ISNOTNULL[N] = F;
            ISNOTNULL[F] = T;

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
                // We only need to compute unary operations for compound values.
                if (splitP.Length > 1)
                {
                    int notResult = 0;
                    int isNullResult = 0;
                    int isNotNullResult = 0;
                    foreach (var p0 in splitP)
                    {
                        notResult |= NOT[p0];
                        isNullResult |= ISNULL[p0];
                        isNotNullResult |= ISNOTNULL[p0];
                    }
                    NOT[p] = notResult;
                    ISNULL[p] = isNullResult;
                    ISNOTNULL[p] = isNotNullResult;
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
        private Stack<Dictionary<string, int>> _memberExpressionMappings = new Stack<Dictionary<string, int>>();

        // The following two are used for member expressions traversal.
        private int _memberExpressionDepth = 0;

        internal
            WhereJoinDetector(NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
            : base(nameGenerator, isEntityDecider, joins, expressionMap)
        {
        }

        internal static void Find(Expression expression, NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
        {
            WhereJoinDetector f = new WhereJoinDetector(nameGenerator, isEntityDecider, joins, expressionMap);

            f._memberExpressionMappings.Push(new Dictionary<string, int>());

            f.VisitExpression(expression);

            foreach (var mapping in f._memberExpressionMappings.Pop())
            {
                // If outer join can never produce true, we can safely inner join.
                if ((mapping.Value & T) == 0)
                {
                    f.MakeInnerIfJoined(mapping.Key);
                }
            }
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);

            Expression baseResult = expression;
            if (expression.NodeType == ExpressionType.AndAlso && expression.Type == typeof(bool))
            {
                // Case (a) from the text at NH-2583.
                _memberExpressionMappings.Push(new Dictionary<string, int>());
                var newLeft = VisitExpression(expression.Left);
                var leftMapping = _memberExpressionMappings.Pop();

                _memberExpressionMappings.Push(new Dictionary<string, int>());
                var newRight = VisitExpression(expression.Right);
                var rightMapping = _memberExpressionMappings.Pop();

                BinaryMapping(leftMapping, rightMapping, AND);

                // The following is copy-pasted from Relinq's visitor, as I had to split the code above.
                var newConversion = (LambdaExpression)VisitExpression(expression.Conversion);
                if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                    baseResult = Expression.MakeBinary(expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            }
            else if (expression.NodeType == ExpressionType.OrElse && expression.Type == typeof(bool))
            {
                // Case (b)
                _memberExpressionMappings.Push(new Dictionary<string, int>());
                var newLeft = VisitExpression(expression.Left);
                var leftMapping = _memberExpressionMappings.Pop();

                _memberExpressionMappings.Push(new Dictionary<string, int>());
                var newRight = VisitExpression(expression.Right);
                var rightMapping = _memberExpressionMappings.Pop();

                BinaryMapping(leftMapping, rightMapping, OR);

                // Again, the following is copy-pasted from Relinq's visitor, as I had to split the code above.
                var newConversion = (LambdaExpression)VisitExpression(expression.Conversion);
                if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                    baseResult = Expression.MakeBinary(expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            }
            else if (expression.Type == typeof(bool)
                && expression.NodeType == ExpressionType.NotEqual
                && (IsNullConstantExpression(expression.Right) || IsNullConstantExpression(expression.Left)))
            {
                // Case (h)
                _memberExpressionMappings.Push(new Dictionary<string, int>());
                baseResult = base.VisitBinaryExpression(expression);
                UnaryMapping(_memberExpressionMappings.Pop(), ISNOTNULL);
            }
            else if (expression.Type == typeof(bool)
                && expression.NodeType == ExpressionType.Equal
                && (IsNullConstantExpression(expression.Right) || IsNullConstantExpression(expression.Left)))
            {
                // Case (i)
                _memberExpressionMappings.Push(new Dictionary<string, int>());
                baseResult = base.VisitBinaryExpression(expression);
                UnaryMapping(_memberExpressionMappings.Pop(), ISNULL);
            }
            else // +, * etc.
            {
                // Case (j)
                baseResult = base.VisitBinaryExpression(expression);
            }
            return baseResult;
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);

            Expression baseResult;
            if (expression.NodeType == ExpressionType.Not && expression.Type == typeof(bool))
            {
                // Case (c) from text at NH-2583.
                _memberExpressionMappings.Push(new Dictionary<string, int>());
                baseResult = VisitExpression(expression.Operand);
                UnaryMapping(_memberExpressionMappings.Pop(), NOT);
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

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            _memberExpressionMappings.Push(new Dictionary<string, int>());
            Expression result = base.VisitMethodCallExpression(expression);
            // We would usually get NULL if one of our inner member expresions was null. (Mapped to N)
            // However, it's possible a method call will convert the null value from the failed join into any one of True, False, or Null. (Mapped to TNF)
            // This could be optimized by actually checking what the method does.  For example StartsWith("s") would leave null as null and would still allow us to inner join.
            FixedMapping(_memberExpressionMappings.Pop(), TNF);
            return result;
        }

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
                AddPossibility(ExpressionKeyVisitor.Visit(expression, null), N);
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

        private void FixedMapping(Dictionary<string, int> sourceMapping, int value)
        {
            foreach (var me in sourceMapping.Keys)
            {
                AddPossibility(me, value);
            }
        }

        private void BinaryMapping(Dictionary<string, int> leftMapping, Dictionary<string, int> rightMapping, int[,] op)
        {
            // Compute mapping for all member expressions in leftMapping. If the member expression is missing
            // in rightMapping, use TNF as a "pessimistic approximation" instead (inside the ?: operator). See
            // the text for an explanation of this.
            foreach (var lhs in leftMapping)
            {
                AddPossibility(lhs.Key, op[lhs.Value, rightMapping.ContainsKey(lhs.Key) ? rightMapping[lhs.Key] : TNF]);
            }
            // Compute mapping for all member expressions *only* in rightMapping (we did the common ones above).
            // Again, use TNF as pessimistic approximation to result of left subcondition.
            foreach (var rhs in rightMapping)
            {
                if (!leftMapping.ContainsKey(rhs.Key))
                {
                    AddPossibility(rhs.Key, op[rhs.Value, TNF]);
                }
            }
        }

        private void UnaryMapping(Dictionary<string, int> sourceMapping, int[] op)
        {
            foreach (var item in sourceMapping)
            {
                AddPossibility(item.Key, op[item.Value]);
            }
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

        private void AddPossibility(string memberPath, int value)
        {
            Dictionary<string, int> mapping = _memberExpressionMappings.Peek();
            if (mapping.ContainsKey(memberPath))
                mapping[memberPath] |= value;
            else
                mapping[memberPath] = value;
        }
    }
}
