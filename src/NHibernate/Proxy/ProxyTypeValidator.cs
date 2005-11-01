using System;
using System.Reflection;

namespace NHibernate.Proxy
{
	public class ProxyTypeValidator
	{
		private System.Type type;

		public ProxyTypeValidator( System.Type type )
		{
			this.type = type;
		}

		public void Validate()
		{
			if( type.IsInterface )
			{
				// Any interface is valid as a proxy
				return;
			}
			CheckHasVisibleDefaultConstructor();
			CheckEveryPublicMemberIsVirtual();
		}

		public static void Error( System.Type type, string reason )
		{
			throw new InvalidProxyTypeException( type, reason );
		}

		public void CheckHasVisibleDefaultConstructor()
		{
			if( !HasVisibleDefaultConstructor( type ) )
			{
				Error( type, "type does not have a visible (public or protected) no-argument constructor" );
			}
		}

		public void CheckEveryPublicMemberIsVirtual()
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
						CheckMethodIsVirtual( type, accessor );
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
					CheckMethodIsVirtual( type, ( MethodInfo ) member );
				}
				else if( member is FieldInfo )
				{
					Error( type, "public field " + member.Name + " is not allowed" );
				}
			}
		}

		public static void CheckMethodIsVirtual( System.Type type, MethodInfo method )
		{
			if( !method.IsVirtual )
			{
				Error( type, "method " + method.Name + " should be virtual" );
			}
		}

		public static bool HasVisibleDefaultConstructor( System.Type type )
		{
			ConstructorInfo constructor = type.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, System.Type.EmptyTypes, null );

			return constructor != null
				&& !constructor.IsPrivate;
		}
	}
}
