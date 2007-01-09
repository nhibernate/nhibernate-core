using System;
using System.Collections;
using System.Reflection;
using System.Text;
using NHibernate;
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
		/// <returns>A <see cref="InvalidProxyTypeException" /> instance containing
		/// problems detected, or <c>null</c> if no problems were found.
		/// </returns>
		/// <param name="type">The type to validate.</param>
		public static InvalidProxyTypeException ValidateType( System.Type type )
		{
			ArrayList errors = new ArrayList();

			if( type.IsInterface )
			{
				// Any interface is valid as a proxy
				return null;
			}
			CheckHasVisibleDefaultConstructor( type, errors );
			CheckEveryPublicMemberIsVirtual( type, errors );
			CheckNotSealed( type, errors );
			return ConvertToException(type, errors);
		}

		private static InvalidProxyTypeException ConvertToException(System.Type type, ArrayList errors)
		{
			if (errors.Count > 0)
			{
				return new InvalidProxyTypeException(type, errors);
			}
			else
			{
				return null;
			}
		}

		private static void Error( ArrayList errors, string text )
		{
			errors.Add(text);
		}

		private static void CheckHasVisibleDefaultConstructor( System.Type type, ArrayList errors )
		{
			if( !HasVisibleDefaultConstructor( type ) )
			{
				Error( errors, "type does not have a visible (public or protected) no-argument constructor" );
			}
		}

		private static void CheckEveryPublicMemberIsVirtual( System.Type type, ArrayList errors )
		{
			MemberInfo[] members = type.GetMembers( BindingFlags.Instance | BindingFlags.Public );

			foreach( MemberInfo member in members )
			{
				if( member is PropertyInfo )
				{
					PropertyInfo property = ( PropertyInfo ) member;
					MethodInfo[] accessors = property.GetAccessors( false );
					
					foreach( MethodInfo accessor in accessors )
					{
						CheckMethodIsVirtual( type, accessor, errors );
					}
				}
				else if( member is MethodInfo )
				{
					if( member.DeclaringType == typeof( object )
						&& member.Name == "GetType" )
					{
						// object.GetType is ignored
						continue;
					}
					CheckMethodIsVirtual( type, ( MethodInfo ) member, errors );
				}
				else if( member is FieldInfo )
				{
					Error( errors, "public field " + member.Name + " is not allowed" );
				}
			}
		}

		private static void CheckMethodIsVirtual( System.Type type, MethodInfo method, ArrayList errors )
		{
			if( !method.IsVirtual )
			{
				Error( errors, "method " + method.Name + " should be virtual" );
			}
		}

		private static bool HasVisibleDefaultConstructor( System.Type type )
		{
			ConstructorInfo constructor = type.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, System.Type.EmptyTypes, null );

			return constructor != null
				&& !constructor.IsPrivate;
		}

		private static void CheckNotSealed( System.Type type, ArrayList errors )
		{
			if( ReflectHelper.IsFinalClass( type ) )
			{
				Error( errors, "type is sealed" );
			}
		}
	}
}
