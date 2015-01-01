using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq
{
	public abstract class Assignments
	{
		protected static readonly ConstructorInfo DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new[] { typeof(int) });
		protected static readonly MethodInfo DictionaryAddMethodInfo = typeof(Dictionary<string, object>).GetMethod("Add");
	}

	/// <summary>
	/// Class to hold assigments used in updates and inserts
	/// </summary>
	/// <typeparam name="TInput">The type of the input.</typeparam>
	/// <typeparam name="TOutput">The type of the output.</typeparam>
	public class Assignments<TInput, TOutput> : Assignments
	{
		private readonly List<Assignment> _sets = new List<Assignment>();

		/// <summary>
		/// Sets the specified property.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="expression">The expression that should be assigned to the property.</param>
		/// <returns></returns>
		public Assignments<TInput, TOutput> Set<TProp>(Expression<Func<TOutput, TProp>> property, Expression<Func<TInput, TProp>> expression)
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
		/// <returns></returns>
		public Assignments<TInput, TOutput> Set<TProp>(Expression<Func<TOutput, TProp>> property, TProp value)
		{
			var member = GetMemberExpression(property);
			_sets.Add(new Assignment(member.GetMemberPath(), Expression.Constant(value, typeof(TProp))));
			return this;
		}

		private static MemberExpression GetMemberExpression<TProp>(Expression<Func<TOutput, TProp>> property)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			var param = property.Parameters.Single();
			var member = property.Body as MemberExpression ??
				throw new ArgumentException($"The property expression must refer to a property of {param.Name}({param.Type.Name})", nameof(property));
			return member;
		}

		/// <summary>
		/// Converts the assignments into a to lambda expression, which creates a Dictionary&lt;string,object%gt;.
		/// </summary>
		/// <returns></returns>
		public LambdaExpression ConvertToDictionaryExpression()
		{
			var param = Expression.Parameter(typeof(TInput));
			var inits = new List<ElementInit>();
			foreach (var set in _sets)
			{
				var setter = set.Expression;
				var setterLambda = setter as LambdaExpression;
				if (setterLambda != null)
				{
					setter = setterLambda.Body.Replace(setterLambda.Parameters.First(), param);
				}
				inits.Add(Expression.ElementInit(DictionaryAddMethodInfo, Expression.Constant(set.PropertyPath),
															Expression.Convert(
																setter,
																typeof(object))));

			}


			//The ListInit is intentionally "infected" with the lambda parameter (param), in the form of an IIF. 
			//The only relevance is to make sure that the ListInit is not evaluated by the PartialEvaluatingExpressionTreeVisitor,
 			//which could turn it into a Constant
			var listInit = Expression.ListInit(
				Expression.New(
					DictionaryConstructorInfo,
					Expression.Condition(
						Expression.Equal(param, Expression.Constant(null, typeof(TInput))),
						Expression.Constant(_sets.Count),
						Expression.Constant(_sets.Count))),
				inits);



			return Expression.Lambda(listInit, param);
		}

		public static Assignments<TInput, TOutput> FromExpression(Expression<Func<TInput, TOutput>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var instance = new Assignments<TInput, TOutput>();
			var memberInitExpression = expression.Body as MemberInitExpression;
			
			if (memberInitExpression == null)
			{
				throw new ArgumentException("The expression must be member initialization, e.g. x => new Dog{Name = x.Name,Age = x.Age + 5}");
			}
			
			AddSetsFromBindings(memberInitExpression.Bindings, instance, "", expression.Parameters);
			
			return instance;
		}

		private static void AddSetsFromBindings(IEnumerable<MemberBinding> bindings, Assignments<TInput, TOutput> instance, string path, ReadOnlyCollection<ParameterExpression> parameters)
		{
			foreach (var binding in bindings)
			{
				if (binding.BindingType == MemberBindingType.Assignment) // {Property="Value"}
				{
					AddSetsFromAssignment((MemberAssignment)binding, instance, path + "." + binding.Member.Name, parameters);
				}
				else if (binding.BindingType == MemberBindingType.MemberBinding) // {Property={SubProperty="Value}}
				{
					AddSetsFromBindings(((MemberMemberBinding) binding).Bindings, instance, path + "." + binding.Member.Name, parameters);
				}
			}
		}

		private static void AddSetsFromAssignment(MemberAssignment assignment, Assignments<TInput, TOutput> instance, string path, ReadOnlyCollection<ParameterExpression> parameters)
		{
			var memberInit = assignment.Expression as MemberInitExpression; // {Property=new Instance{SubProperty="Value"}}
			if (memberInit!=null)
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