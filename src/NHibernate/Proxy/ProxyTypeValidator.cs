using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	public class ProxyTypeValidator
	{
		private ProxyTypeValidator()
		{
		}

		/// <summary>
		/// Validates whether <paramref name="type"/> can be specified as the base class
		/// (or an interface) for a dynamically-generated proxy.
		/// </summary>
		/// <returns>
		/// A collection of errors, if any, or <see langword="null" /> if none were found.
		/// </returns>
		/// <param name="type">The type to validate.</param>
		public static ICollection ValidateType(System.Type type)
		{
			StringCollection errors = new StringCollection();

			if (type.IsInterface)
			{
				// Any interface is valid as a proxy
				return null;
			}
			CheckHasVisibleDefaultConstructor(type, errors);
			CheckAccessibleMembersAreVirtual(type, errors);
			CheckNotSealed(type, errors);
			if (errors.Count > 0)
			{
				return errors;
			}
			return null;
		}

		private static void Error(IList errors, System.Type type, string text)
		{
			errors.Add(string.Format("{0}: {1}", type, text));
		}

		private static void CheckHasVisibleDefaultConstructor(System.Type type, IList errors)
		{
			if (!HasVisibleDefaultConstructor(type))
			{
				Error(errors, type, "type should have a visible (public or protected) no-argument constructor");
			}
		}

		private static void CheckAccessibleMembersAreVirtual(System.Type type, IList errors)
		{
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MemberInfo member in members)
			{
				if (member is PropertyInfo)
				{
					PropertyInfo property = (PropertyInfo) member;
					MethodInfo[] accessors = property.GetAccessors(false);

					foreach (MethodInfo accessor in accessors)
					{
						CheckMethodIsVirtual(type, accessor, errors);
					}
				}
				else if (member is MethodInfo)
				{
					if (member.DeclaringType == typeof(object)
					    && member.Name == "GetType")
					{
						// object.GetType is ignored
						continue;
					}
					CheckMethodIsVirtual(type, (MethodInfo) member, errors);
				}
				else if (member is FieldInfo)
				{
					FieldInfo memberField = (FieldInfo) member;
					if (memberField.IsPublic || memberField.IsAssembly || memberField.IsFamilyOrAssembly)
					{
						Error(errors, type, "field " + member.Name + " should not be public nor internal");
					}
				}
			}
		}

		private static void CheckMethodIsVirtual(System.Type type, MethodInfo method, IList errors)
		{
			if (method.DeclaringType != typeof(object) && !IsDisposeMethod(method) &&
				method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly)
			{
				if (!method.IsVirtual || method.IsFinal)
				{
					Error(errors, type, "method " + method.Name + " should be virtual");
				}
			}
		}

		private static bool IsDisposeMethod(MethodBase method)
		{
			return method.Name.Equals("Dispose") && method.MemberType == MemberTypes.Method && method.GetParameters().Length == 0;
		}

		private static bool HasVisibleDefaultConstructor(System.Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, System.Type.EmptyTypes, null);

			return constructor != null
			       && !constructor.IsPrivate;
		}

		private static void CheckNotSealed(System.Type type, IList errors)
		{
			if (ReflectHelper.IsFinalClass(type))
			{
				Error(errors, type, "type should not be sealed");
			}
		}
	}
}
