using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Linq
{
	[Obsolete("Please use NHibernate.Util.ReflectHelper instead")]
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
			return ReflectHelper.GetMethodDefinition(method);
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <typeparam name="TSource">The declaring-type of the method.</typeparam>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the method.</returns>
		public static MethodInfo GetMethod<TSource>(Expression<Action<TSource>> method)
		{
			return ReflectHelper.GetMethod(method);
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the no-generic method or the generic-definition for a generic-method.</returns>
		/// <seealso cref="MethodInfo.GetGenericMethodDefinition"/>
		public static MethodInfo GetMethodDefinition(Expression<System.Action> method)
		{
			return ReflectHelper.GetMethodDefinition(method);
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the method.</returns>
		public static MethodInfo GetMethod(Expression<System.Action> method)
		{
			return ReflectHelper.GetMethod(method);
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
			return ReflectHelper.GetProperty(property);
		}
	}

	[Obsolete("Please use NHibernate.Util.ReflectHelper instead")]
	public static class EnumerableHelper
	{
		public static MethodInfo GetMethod(string name, System.Type[] parameterTypes)
		{
			return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(m => m.Name == name &&
					ReflectHelper.ParameterTypesMatch(m.GetParameters(), parameterTypes))
				.Single();
		}

		public static MethodInfo GetMethod(string name, System.Type[] parameterTypes, System.Type[] genericTypeParameters)
		{
			return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(m => m.Name == name &&
							m.ContainsGenericParameters &&
							m.GetGenericArguments().Count() == genericTypeParameters.Length &&
							ReflectHelper.ParameterTypesMatch(m.GetParameters(), parameterTypes))
				.Single()
				.MakeGenericMethod(genericTypeParameters);
		}
	}
}