using System.Collections.Generic;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	public class DynProxyTypeValidator : IProxyValidator
	{
		private readonly List<string> errors = new List<string>();

		/// <summary>
		/// Validates whether <paramref name="type"/> can be specified as the base class
		/// (or an interface) for a dynamically-generated proxy.
		/// </summary>
		/// <param name="type">The type to validate.</param>
		/// <returns>
		/// A collection of errors messages, if any, or <see langword="null" /> if none were found.
		/// </returns>
		public ICollection<string> ValidateType(System.Type type)
		{
			errors.Clear();
			if (type.IsInterface)
			{
				// Any interface is valid as a proxy
				return null;
			}
			CheckHasVisibleDefaultConstructor(type);
			CheckAccessibleMembersAreVirtual(type);
			CheckNotSealed(type);
			return errors.Count > 0 ? errors : null;
		}

		protected void EnlistError(System.Type type, string text)
		{
			errors.Add(string.Format("{0}: {1}", type, text));
		}

		protected virtual void CheckHasVisibleDefaultConstructor(System.Type type)
		{
			if (!HasVisibleDefaultConstructor(type))
			{
				EnlistError(type, "type should have a visible (public or protected) no-argument constructor");
			}
		}

		protected virtual void CheckAccessibleMembersAreVirtual(System.Type type)
		{
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (var member in members)
			{
				if (member is PropertyInfo)
				{
					var property = (PropertyInfo) member;
					MethodInfo[] accessors = property.GetAccessors(false);

					if (accessors != null)
					{
						foreach (var accessor in accessors)
						{
							CheckMethodIsVirtual(type, accessor);
						}
					}
				}
				else if (member is MethodInfo)
				{
					if (member.DeclaringType == typeof (object) && member.Name == "GetType")
					{
						// object.GetType is ignored
						continue;
					}
					CheckMethodIsVirtual(type, (MethodInfo) member);
				}
				else if (member is FieldInfo)
				{
					var memberField = (FieldInfo) member;
					if (memberField.IsPublic || memberField.IsAssembly || memberField.IsFamilyOrAssembly)
					{
						EnlistError(type, "field " + member.Name + " should not be public nor internal");
					}
				}
			}
		}

		protected virtual void CheckMethodIsVirtual(System.Type type, MethodInfo method)
		{
			if (!IsProxeable(method))
			{
				EnlistError(type, "method " + method.Name + " should be 'public/protected virtual' or 'protected internal virtual'");
			}
		}

		public virtual bool IsProxeable(MethodInfo method)
		{
			// In NET if IsVirtual is false or IsFinal is true, then the method cannot be overridden.
			return !(method.DeclaringType != typeof (object) && !IsDisposeMethod(method)
			       && (method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly)
			       && (!method.IsVirtual || method.IsFinal || (method.IsVirtual && method.IsAssembly)));
		}

		protected static bool IsDisposeMethod(MethodBase method)
		{
			return method.Name.Equals("Dispose") && method.MemberType == MemberTypes.Method && method.GetParameters().Length == 0;
		}

		protected virtual bool HasVisibleDefaultConstructor(System.Type type)
		{
			ConstructorInfo constructor =
				type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
				                    System.Type.EmptyTypes, null);

			return constructor != null && !constructor.IsPrivate;
		}

		protected void CheckNotSealed(System.Type type)
		{
			if (ReflectHelper.IsFinalClass(type))
			{
				EnlistError(type, "type should not be sealed");
			}
		}
	}
}