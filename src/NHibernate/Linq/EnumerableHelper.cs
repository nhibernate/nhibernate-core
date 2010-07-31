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
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			MethodInfo methodInfo = ((MethodCallExpression) method.Body).Method;
			return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
		}

		/// <summary>
		/// Extract the <see cref="MethodInfo"/> from a given expression.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The <see cref="MethodInfo"/> of the no-generic method or the generic-definition for a generic-method.</returns>
		/// <seealso cref="MethodInfo.GetGenericMethodDefinition"/>
		public static MethodInfo GetMethodDefinition(Expression<System.Action> method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			var methodInfo = ((MethodCallExpression)method.Body).Method;
			return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
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

		/// <summary>
		/// Check if a method is declared in a given <see cref="System.Type"/>.
		/// </summary>
		/// <param name="source">The method to check.</param>
		/// <param name="realDeclaringType">The where the method is really declared.</param>
		/// <returns>True if the method is an implementation of the method declared in <paramref name="realDeclaringType"/>; false otherwise. </returns>
		public static bool IsMethodOf(this MethodInfo source, System.Type realDeclaringType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (realDeclaringType == null)
			{
				throw new ArgumentNullException("realDeclaringType");
			}
			var methodDeclaringType = source.DeclaringType;
			if(realDeclaringType.Equals(methodDeclaringType))
			{
				return true;
			}
			if (methodDeclaringType.IsGenericType && !methodDeclaringType.IsGenericTypeDefinition && 
				realDeclaringType.Equals(methodDeclaringType.GetGenericTypeDefinition()))
			{
				return true;
			}
			if (realDeclaringType.IsInterface)
			{
				var declaringTypeInterfaces = methodDeclaringType.GetInterfaces();
				if(declaringTypeInterfaces.Contains(realDeclaringType))
				{
					var methodsMap = methodDeclaringType.GetInterfaceMap(realDeclaringType);
					if(methodsMap.TargetMethods.Contains(source))
					{
						return true;
					}
				}
				if (realDeclaringType.IsGenericTypeDefinition)
				{
					bool implements = declaringTypeInterfaces
						.Where(t => t.IsGenericType && t.GetGenericTypeDefinition().Equals(realDeclaringType))
						.Select(implementedGenericInterface => methodDeclaringType.GetInterfaceMap(implementedGenericInterface))
						.Any(methodsMap => methodsMap.TargetMethods.Contains(source));
					if (implements)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

    // TODO rename / remove - reflection helper above is better
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