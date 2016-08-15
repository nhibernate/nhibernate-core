#if FEATURE_NETCORE_REFLECTION_API

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate
{
	internal static class TypeReflectionExtensions
	{
		public static MethodInfo GetMethod(this System.Type type, string name, BindingFlags bindingAttr, object binder,
			System.Type[] types, ParameterModifier[] modifiers)
		{
			return type.GetMethods(bindingAttr)
				.FirstOrDefault(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
		}

		public static PropertyInfo GetProperty(this System.Type type, string name, BindingFlags bindingAttr, System.Type returnType, object binder,
			System.Type[] types, ParameterModifier[] modifiers)
		{
			return type.GetProperties(bindingAttr)
				.FirstOrDefault(m => m.Name == name 
					&& (returnType == null || m.PropertyType == returnType)
					&& (type == null || m.GetIndexParameters().Select(p => p.ParameterType).SequenceEqual(types)));
		}

		public static ConstructorInfo GetConstructor(this System.Type type, BindingFlags bindingAttr, object binder,
			System.Type[] types, ParameterModifier[] modifiers)
		{
			return type.GetConstructors(bindingAttr)
				.FirstOrDefault(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
		}

		public static ConstructorInfo GetConstructor(this System.Type type, BindingFlags bindingAttr, object binder,
			CallingConventions callingConventions, System.Type[] types, ParameterModifier[] modifiers)
		{
			return type.GetConstructors(bindingAttr)
				.FirstOrDefault(m => m.CallingConvention.HasFlag(callingConventions) && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
		}
	}
}

#endif
