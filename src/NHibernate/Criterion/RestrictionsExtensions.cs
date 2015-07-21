using System;
using System.Collections;
using System.Linq.Expressions;
using NHibernate.Impl;

namespace NHibernate.Criterion
{
	public static class RestrictionExtensions
	{
		/// <summary>
		/// Apply a "like" restriction in a QueryOver expression
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsLike(this string projection, string comparison)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply a "like" restriction in a QueryOver expression
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsLike(this string projection, string comparison, MatchMode matchMode)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply a "like" restriction in a QueryOver expression
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsLike(this string projection, string comparison, MatchMode matchMode, char? escapeChar)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply a "like" restriction in a QueryOver expression
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsInsensitiveLike(this string projection, string comparison)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply a "like" restriction in a QueryOver expression
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsInsensitiveLike(this string projection, string comparison, MatchMode matchMode)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply an "in" constraint to the named property 
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsIn(this object projection, object[] values)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply an "in" constraint to the named property 
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static bool IsIn(this object projection, ICollection values)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static RestrictionBetweenBuilder IsBetween(this object projection, object lo)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		public class RestrictionBetweenBuilder
		{
			public bool And(object hi)
			{
				throw new Exception("Not to be used directly - use inside QueryOver expression");
			}
		}

		public static ICriterion ProcessIsLike(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			object value = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			return projection.CreateCriterion(Restrictions.Like, Restrictions.Like, value);
		}

		public static ICriterion ProcessIsLikeMatchMode(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			string value = (string)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			MatchMode matchMode = (MatchMode)ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
			return projection.Create<ICriterion>(s => Restrictions.Like(s, value, matchMode), p => Restrictions.Like(p, value, matchMode));
		}

		public static ICriterion ProcessIsLikeMatchModeEscapeChar(MethodCallExpression methodCallExpression)
		{
			string property = ExpressionProcessor.FindMemberExpression(methodCallExpression.Arguments[0]);
			string value = (string)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			MatchMode matchMode = (MatchMode)ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
			char? escapeChar = (char?)ExpressionProcessor.FindValue(methodCallExpression.Arguments[3]);
			return Restrictions.Like(property, value, matchMode, escapeChar);
		}

		public static ICriterion ProcessIsInsensitiveLike(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			object value = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			return projection.CreateCriterion(Restrictions.InsensitiveLike, Restrictions.InsensitiveLike, value);
		}

		public static ICriterion ProcessIsInsensitiveLikeMatchMode(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			string value = (string)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			MatchMode matchMode = (MatchMode)ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
			return projection.Create<ICriterion>(s => Restrictions.InsensitiveLike(s, value, matchMode), p => Restrictions.InsensitiveLike(p, value, matchMode));
		}

		public static ICriterion ProcessIsInArray(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			object[] values = (object[])ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			return projection.Create<ICriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values));
		}

		public static ICriterion ProcessIsInCollection(MethodCallExpression methodCallExpression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
			var values = (ICollection)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
			return projection.Create<ICriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values));
		}

		public static ICriterion ProcessIsBetween(MethodCallExpression methodCallExpression)
		{
			MethodCallExpression betweenFunction = (MethodCallExpression)methodCallExpression.Object;
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(betweenFunction.Arguments[0]);
			object lo = ExpressionProcessor.FindValue(betweenFunction.Arguments[1]);
			object hi = ExpressionProcessor.FindValue(methodCallExpression.Arguments[0]);
			return projection.Create<ICriterion>(s => Restrictions.Between(s, lo, hi), p => Restrictions.Between(p, lo, hi));
		}
	}
}
