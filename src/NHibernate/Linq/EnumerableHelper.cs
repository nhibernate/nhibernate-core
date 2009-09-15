using System.Linq;
using System.Reflection;

namespace NHibernate.Linq
{
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