using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq
{
	public static class ReflectionHelper
	{
		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <typeparam name="TSource">The declaring-type of the method.</typeparam>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the no-generic method or the generic-definition for a generic-method.</returns>
		/// <seealso cref="MethodInfo.GetGenericMethodDefinition"/>
		public static MethodInfo GetMethodDefinition<TSource>(Expression<Action<TSource>> method)
		{
			MethodInfo methodInfo = GetMethod(method);
			return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <typeparam name="TSource">The declaring-type of the method.</typeparam>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the method.</returns>
		public static MethodInfo GetMethod<TSource>(Expression<Action<TSource>> method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return ((MethodCallExpression)method.Body).Method;
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the no-generic method or the generic-definition for a generic-method.</returns>
		/// <seealso cref="MethodInfo.GetGenericMethodDefinition"/>
		public static MethodInfo GetMethodDefinition(Expression<System.Action> method)
		{
			MethodInfo methodInfo = GetMethod(method);
			return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the method.</returns>
		public static MethodInfo GetMethod(Expression<System.Action> method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return ((MethodCallExpression)method.Body).Method;
		}

		/// <summary>
		/// Gets the field or property to be accessed.
		/// </summary>
		/// <typeparam name="TSource">The declaring-type of the property.</typeparam>
		/// <typeparam name="TResult">The type of the property.</typeparam>
		/// <param name="property">The expression representing the property getter.</param>
		/// <returns>The <see cref="MemberInfo"/> of the property.</returns>
		public static MemberInfo GetProperty<TSource, TResult>(Expression<Func<TSource, TResult>> property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			return ((MemberExpression)property.Body).Member;
		}

		internal static System.Type GetPropertyOrFieldType(this MemberInfo memberInfo)
		{
			var propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}

			var fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}

			return null;
		}
	}

	internal static class ReflectionCache
	{
		// When adding a method to this cache, please follow the naming convention of those subclasses and fields:
		//  - Add your method to a subclass named according to the type holding the method, and suffixed with "Methods".
		//  - Name the field according to the method name.
		//  - If the method has overloads, suffix it with "With" followed by its parameter names. Do not list parameters
		//    common to all overloads.
		//  - If the method is a generic method definition, add "Definition" as final suffix.
		//  - If the method is generic, suffix it with "On" followed by its generic parameter type names.
		// Avoid caching here narrow cases, such as those using specific types and unlikely to be used by many classes.
		// Cache them instead in classes using them.
		internal static class EnumerableMethods
		{
			internal static readonly MethodInfo AggregateDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object>(null, null));
			internal static readonly MethodInfo AggregateWithSeedDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object>(null, null, null));
			internal static readonly MethodInfo AggregateWithSeedAndResultSelectorDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object, object>(null, null, null, null));

			internal static readonly MethodInfo CastDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Cast<object>(null));
			internal static readonly MethodInfo CastOnObjectArray =
				ReflectionHelper.GetMethod(() => Enumerable.Cast<object[]>(null));

			internal static readonly MethodInfo GroupByWithElementSelectorDefinition = ReflectionHelper.GetMethodDefinition(
				() => Enumerable.GroupBy<object, object, object>(null, null, (Func<object, object>)null));

			internal static readonly MethodInfo SelectDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Select<object, object>(null, (Func<object, object>)null));

			internal static readonly MethodInfo ToArrayDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.ToArray<object>(null));

			internal static readonly MethodInfo ToListDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.ToList<object>(null));
		}
	}
	
	[Obsolete("Please use ReflectionHelper instead")]
	public static class EnumerableHelper
	{
		public static MethodInfo GetMethod(string name, System.Type[] parameterTypes)
		{
			return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(m => m.Name == name &&
							ParameterTypesMatch(m.GetParameters(), parameterTypes))
				.Single();
		}

		public static MethodInfo GetMethod(string name, System.Type[] parameterTypes, System.Type[] genericTypeParameters)
		{
			return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(m => m.Name == name &&
							m.ContainsGenericParameters &&
							m.GetGenericArguments().Count() == genericTypeParameters.Length &&
							ParameterTypesMatch(m.GetParameters(), parameterTypes))
				.Single()
				.MakeGenericMethod(genericTypeParameters);
		}

		private static bool ParameterTypesMatch(ParameterInfo[] parameters, System.Type[] types)
		{
			if (parameters.Length != types.Length)
			{
				return false;
			}

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType == types[i])
				{
					continue;
				}

				if (parameters[i].ParameterType.ContainsGenericParameters && types[i].ContainsGenericParameters &&
					parameters[i].ParameterType.GetGenericArguments().Length == types[i].GetGenericArguments().Length)
				{
					continue;
				}

				return false;
			}

			return true;
		}
	}
}