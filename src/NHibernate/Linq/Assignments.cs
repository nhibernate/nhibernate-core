using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq
{
	public abstract class Assignments
	{
		protected static readonly ConstructorInfo DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new[] { typeof(int) });
		protected static readonly MethodInfo DictionaryAddMethodInfo = ReflectHelper.GetMethod<Dictionary<string, object>>(d => d.Add(null, null));
	}

	/// <summary>
	/// Class to hold assignments used in updates and inserts.
	/// </summary>
	/// <typeparam name="TSource">The type of the entity source of the insert or to update.</typeparam>
	/// <typeparam name="TTarget">The type of the entity to insert or to update.</typeparam>
	public class Assignments<TSource, TTarget> : Assignments
	{
		private readonly List<Assignment> _sets = new List<Assignment>();

		/// <summary>
		/// Sets the specified property.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="expression">The expression that should be assigned to the property.</param>
		/// <returns>The current assignments list.</returns>
		public Assignments<TSource, TTarget> Set<TProp>(Expression<Func<TTarget, TProp>> property, Expression<Func<TSource, TProp>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var member = GetMemberExpression(property);
			_sets.Add(new Assignment(member.GetMemberPath(), expression));
			return this;
		}

		/// <summary>
		/// Sets the specified property.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns>The current assignments list.</returns>
		public Assignments<TSource, TTarget> Set<TProp>(Expression<Func<TTarget, TProp>> property, TProp value)
		{
			var member = GetMemberExpression(property);
			_sets.Add(new Assignment(member.GetMemberPath(), Expression.Constant(value, typeof(TProp))));
			return this;
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

		/// <summary>
		/// Converts the assignments into a lambda expression, which creates a Dictionary&lt;string,object%gt;.
		/// </summary>
		/// <returns>A lambda expression representing the assignments.</returns>
		public LambdaExpression ConvertToDictionaryExpression()
		{
			var param = Expression.Parameter(typeof(TSource));
			var inits = new List<ElementInit>();
			foreach (var set in _sets)
			{
				var setter = set.Expression;
				if (setter is LambdaExpression setterLambda)
				{
					setter = setterLambda.Body.Replace(setterLambda.Parameters.First(), param);
				}
				inits.Add(Expression.ElementInit(DictionaryAddMethodInfo, Expression.Constant(set.PropertyPath),
					Expression.Convert(setter, typeof(object))));
			}

			//The ListInit is intentionally "infected" with the lambda parameter (param), in the form of an IIF. 
			//The only relevance is to make sure that the ListInit is not evaluated by the PartialEvaluatingExpressionTreeVisitor,
			//which could turn it into a Constant
			var listInit = Expression.ListInit(
				Expression.New(
					DictionaryConstructorInfo,
					Expression.Condition(
						Expression.Equal(param, Expression.Constant(null, typeof(TSource))),
						Expression.Constant(_sets.Count),
						Expression.Constant(_sets.Count))),
				inits);

			return Expression.Lambda(listInit, param);
		}

		/// <summary>
		/// Converts a members initialization expression to assignments. Unset members are ignored and left untouched.
		/// </summary>
		/// <param name="expression">The expression to convert.</param>
		/// <returns>The corresponding assignments.</returns>
		public static Assignments<TSource, TTarget> FromExpression(Expression<Func<TSource, TTarget>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var instance = new Assignments<TSource, TTarget>();
			var memberInitExpression = expression.Body as MemberInitExpression ??
				throw new ArgumentException("The expression must be member initialization, e.g. x => new Dog { Name = x.Name, Age = x.Age + 5 }");

			AddSetsFromBindings(memberInitExpression.Bindings, instance, "", expression.Parameters);

			return instance;
		}

		private static void AddSetsFromBindings(IEnumerable<MemberBinding> bindings, Assignments<TSource, TTarget> instance, string path, ReadOnlyCollection<ParameterExpression> parameters)
		{
			foreach (var binding in bindings)
			{
				if (binding.BindingType == MemberBindingType.Assignment) // {Property="Value"}
				{
					AddSetsFromAssignment((MemberAssignment)binding, instance, path + "." + binding.Member.Name, parameters);
				}
				else if (binding.BindingType == MemberBindingType.MemberBinding) // {Property={SubProperty="Value}}
				{
					AddSetsFromBindings(((MemberMemberBinding)binding).Bindings, instance, path + "." + binding.Member.Name, parameters);
				}
			}
		}

		private static void AddSetsFromAssignment(MemberAssignment assignment, Assignments<TSource, TTarget> instance, string path, ReadOnlyCollection<ParameterExpression> parameters)
		{
			// {Property=new Instance{SubProperty="Value"}}
			if (assignment.Expression is MemberInitExpression memberInit)
			{
				AddSetsFromBindings(memberInit.Bindings, instance, path, parameters);
			}
			else
			{
				instance._sets.Add(new Assignment(path.Substring(1), Expression.Lambda(assignment.Expression, parameters)));
			}
		}
	}
}