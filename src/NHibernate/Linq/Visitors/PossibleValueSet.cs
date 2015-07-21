using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// Represents a possible set of values for a computation.  For example, an expression may
    /// be null, it may be a non-null value, or we may even have a constant value that is known
    /// precisely.  This class contains operators that know how to combine these values with
    /// each other.  This class is intended to be used to provide static analysis of expressions
    /// before we hit the database.  As an example for future improvement, we could handle
    /// ranges of numeric values.  We can also improve this by handling operators such as the
    /// comparison operators and arithmetic operators.  They are currently handled by naive
    /// null checks.
    /// </summary>
    public class PossibleValueSet
    {
        private System.Type ExpressionType { get; set; }
        private bool ContainsNull { get; set; }
        private bool ContainsAllNonNullValues { get; set; }
        private List<object> DistinctValues
        {
            get
            {
                if (_DistinctValues == null)
                    _DistinctValues = new List<object>();
                return _DistinctValues;
            }
        }
        private List<object> _DistinctValues;

        private bool ContainsAnyNonNullValues
        {
            get { return ContainsAllNonNullValues || DistinctValues.Any(); }
        }

        private PossibleValueSet(System.Type expressionType)
        {
            ExpressionType = expressionType;
        }

        public bool Contains(object obj)
        {
            if (obj == null)
                return ContainsNull;

            if (obj.GetType() != ExpressionType)
                return false;

            if (ContainsAllNonNullValues)
                return true;

            return DistinctValues.Contains(obj);
        }

        #region Operations

        public PossibleValueSet Add(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Divide(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Modulo(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Multiply(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Power(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Subtract(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet And(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet Or(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet ExclusiveOr(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet LeftShift(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        public PossibleValueSet RightShift(PossibleValueSet pvs, System.Type resultType)
        {
            return MathOperation(pvs, resultType);
        }

        private PossibleValueSet MathOperation(PossibleValueSet pvs, System.Type resultType)
        {
            // If either side is only null, the result will be null.
            if (!ContainsAnyNonNullValues || !pvs.ContainsAnyNonNullValues)
                return CreateNull(resultType);

            // If neither side is null, the result cannot be null.
            if (!ContainsNull && !pvs.ContainsNull)
                return CreateAllNonNullValues(resultType);

            // Otherwise, the result can be anything.
            return CreateAllValues(resultType);
        }

        public PossibleValueSet AndAlso(PossibleValueSet pvs)
        {
            /*
             * T && T == T
             * T && F == F
             * T && N == N
             * F && T == F
             * F && F == F
             * F && N == F
             * N && T == N
             * N && F == F
             * N && N == N
             */

            var result = new PossibleValueSet(DetermineBoolType(pvs));
            
            if (Contains(true) && pvs.Contains(true) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(true) && pvs.Contains(false) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(true) && pvs.Contains(null)) result.ContainsNull = true;
            if (Contains(false) && pvs.Contains(true) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(false) && pvs.Contains(false) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(false) && pvs.Contains(null) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(null) && pvs.Contains(true)) result.ContainsNull = true;
            if (Contains(null) && pvs.Contains(false) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(null) && pvs.Contains(null)) result.ContainsNull = true;

            return result;
        }

        public PossibleValueSet OrElse(PossibleValueSet pvs)
        {
            /*
             * T || T == T
             * T || F == T
             * T || N == T
             * F || T == T
             * F || F == F
             * F || N == N
             * N || T == T
             * N || F == N
             * N || N == N
             */

            var result = new PossibleValueSet(DetermineBoolType(pvs));

            if (Contains(true) && pvs.Contains(true) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(true) && pvs.Contains(false) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(true) && pvs.Contains(null) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(false) && pvs.Contains(true) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(false) && pvs.Contains(false) && !result.Contains(false)) result.DistinctValues.Add(false);
            if (Contains(false) && pvs.Contains(null)) result.ContainsNull = true;
            if (Contains(null) && pvs.Contains(true) && !result.Contains(true)) result.DistinctValues.Add(true);
            if (Contains(null) && pvs.Contains(false)) result.ContainsNull = true;
            if (Contains(null) && pvs.Contains(null)) result.ContainsNull = true;

            return result;
        }

        public PossibleValueSet Equal(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        public PossibleValueSet NotEqual(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        public PossibleValueSet GreaterThanOrEqual(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        public PossibleValueSet GreaterThan(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        public PossibleValueSet LessThan(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        public PossibleValueSet LessThanOrEqual(PossibleValueSet pvs)
        {
            return ComparisonOperation(pvs);
        }

        private PossibleValueSet ComparisonOperation(PossibleValueSet pvs)
        {
            return MathOperation(pvs, typeof (bool));
        }

        public PossibleValueSet Coalesce(PossibleValueSet pvs)
        {
            if (!ContainsNull)
                return this;

            PossibleValueSet result = new PossibleValueSet(ExpressionType);
            result.DistinctValues.AddRange(DistinctValues);
            result.ContainsAllNonNullValues = ContainsAllNonNullValues;

            result.ContainsNull = pvs.ContainsNull;
            result.ContainsAllNonNullValues |= pvs.ContainsAllNonNullValues;
            result.DistinctValues.AddRange(pvs.DistinctValues.Except(result.DistinctValues));

            return result;
        }

        // Unary Operators

        public PossibleValueSet Not()
        {
            DetermineBoolType();

            var result = new PossibleValueSet(ExpressionType);
            result.ContainsNull = ContainsNull;
            result.DistinctValues.AddRange(DistinctValues.Cast<bool>().Select(v => !v).Cast<object>());
            return result;
        }

        public PossibleValueSet BitwiseNot(System.Type resultType)
        {
            return UnaryMathOperation(resultType);
        }

        public PossibleValueSet ArrayLength(System.Type resultType)
        {
            return CreateAllNonNullValues(typeof (int));
        }

        public PossibleValueSet Convert(System.Type resultType)
        {
            return UnaryMathOperation(resultType);
        }

        public PossibleValueSet Negate(System.Type resultType)
        {
            return UnaryMathOperation(resultType);
        }

        public PossibleValueSet UnaryPlus(System.Type resultType)
        {
            return this;
        }

        private PossibleValueSet UnaryMathOperation(System.Type resultType)
        {
            return
                !ContainsAnyNonNullValues ? CreateNull(resultType) :
                !ContainsNull ? CreateAllNonNullValues(resultType) :
                CreateAllValues(resultType);
        }

        // Special Operators

        public PossibleValueSet IsNull()
        {
            PossibleValueSet result = new PossibleValueSet(typeof(bool));
            if (ContainsNull)
                result.DistinctValues.Add(true);
            if (ContainsAllNonNullValues || DistinctValues.Any())
                result.DistinctValues.Add(false);
            return result;
        }
        
        public PossibleValueSet IsNotNull()
        {
            PossibleValueSet result = new PossibleValueSet(typeof(bool));
            if (ContainsNull)
                result.DistinctValues.Add(false);
            if (ContainsAllNonNullValues || DistinctValues.Any())
                result.DistinctValues.Add(true);
            return result;
        }

        public PossibleValueSet MemberAccess(System.Type resultType)
        {
            // If instance is null, member will be null too.
            if (!ContainsAnyNonNullValues)
                return CreateNull(resultType);

            // Otherwise, value could be anything due to value of member.
            return CreateAllValues(resultType);
        }

        #endregion


        /// <summary>
        /// Verify that ExpressionType of both this and the other set is bool or nullable bool,
        /// and return the negotiated type (nullable bool if either side is nullable).
        /// </summary>
        private System.Type DetermineBoolType(PossibleValueSet otherSet)
        {
            DetermineBoolType();
            otherSet.DetermineBoolType();

            var nullableBoolType = typeof(bool?);
            if (ExpressionType == nullableBoolType || otherSet.ExpressionType == nullableBoolType)
                return nullableBoolType;

            return typeof(bool);
        }


        /// <summary>
        /// Verify that ExpressionType is bool or nullable bool.
        /// </summary>
        private void DetermineBoolType()
        {
            var boolType = typeof(bool);
            var nullableBoolType = typeof(bool?);

            if (ExpressionType != boolType && ExpressionType != nullableBoolType)
                throw new AssertionFailure(
                    "Cannot perform desired possible value set operation on expressions of type: " +
                    ExpressionType);
        }

        public static PossibleValueSet CreateNull(System.Type expressionType)
        {
            return new PossibleValueSet(expressionType) {ContainsNull = true};
        }

        public static PossibleValueSet CreateAllNonNullValues(System.Type expressionType)
        {
            PossibleValueSet result = new PossibleValueSet(expressionType);
            if (expressionType == typeof(bool))
            {
                result.DistinctValues.Add(true);
                result.DistinctValues.Add(false);
            }
            else
            {
                result.ContainsAllNonNullValues = true;
            }
            return result;
        }

        public static PossibleValueSet CreateAllValues(System.Type expressionType)
        {
            PossibleValueSet result = CreateAllNonNullValues(expressionType);
            result.ContainsNull = true;
            return result;
        }

        public static PossibleValueSet Create(System.Type expressionType, params object[] values)
        {
            PossibleValueSet result = new PossibleValueSet(expressionType);
            foreach (var v in values)
            {
                if (v == null)
                    result.ContainsNull = true;
                else if (v.GetType() == expressionType && !result.DistinctValues.Contains(v))
                    result.DistinctValues.Add(v);
                else
                    throw new AssertionFailure("Don't know how to add value to possible value set of type " + expressionType + ": " + v);
            }
            return result;
        }
    }
}
