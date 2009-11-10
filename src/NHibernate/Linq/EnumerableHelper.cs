using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq
{
    public static class ReflectionHelper
    {
        public delegate void Action();

        public static MethodInfo GetMethod<TSource>(Expression<Action<TSource>> method)
        {
            var methodInfo = ((MethodCallExpression) method.Body).Method;
            return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
        }

        public static MethodInfo GetMethod(Expression<Action> method)
        {
            var methodInfo = ((MethodCallExpression)method.Body).Method;
            return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
        }

        public static MemberInfo GetProperty<TSource, TResult>(Expression<Func<TSource, TResult>> property)
        {
            return ((MemberExpression) property.Body).Member;
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