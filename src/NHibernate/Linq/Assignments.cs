using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq
{
	/// <summary>
	/// Class to hold assignments used in updates and inserts.
	/// </summary>
	/// <typeparam name="TSource">The type of the entity source of the insert or to update.</typeparam>
	/// <typeparam name="TTarget">The type of the entity to insert or to update.</typeparam>
	internal class Assignments<TSource, TTarget>
	{
		private readonly Dictionary<string, Expression> _assignments = new Dictionary<string, Expression>();

		internal IReadOnlyDictionary<string, Expression> List => _assignments;

		/// <summary>
		/// Set the specified property.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="expression">The expression that should be assigned to the property.</param>
		/// <returns>The current assignments list.</returns>
		public void Set<TProp>(Expression<Func<TTarget, TProp>> property, Expression<Func<TSource, TProp>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var member = GetMemberExpression(property);
			_assignments.Add(member.GetMemberPath(), expression);
		}

		/// <summary>
		/// Set the specified property.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns>The current assignments list.</returns>
		public void Set<TProp>(Expression<Func<TTarget, TProp>> property, TProp value)
		{
			var member = GetMemberExpression(property);
			_assignments.Add(member.GetMemberPath(), Expression.Constant(value, typeof(TProp)));
		}

		private static MemberExpression GetMemberExpression<TProp>(Expression<Func<TTarget, TProp>> property)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			var param = property.Parameters.Single();
			var member = property.Body as MemberExpression ??
				throw new ArgumentException($"The property expression must refer to a property of {param.Name}({param.Type.Name})", nameof(property));
			return member;
		}
	}
}