using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Accesses property values via a get/set pair, which may be nonpublic.
	/// The default (and recommended strategy).
	/// </summary>
	public class BasicPropertyAccessor : IPropertyAccessor
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BasicPropertyAccessor));
		
		#region IPropertyAccessor Members

		public ISetter GetSetter(System.Type type, string propertyName) 
		{
			BasicSetter result = GetSetterOrNull(type, propertyName);
			if (result==null) throw new PropertyNotFoundException( "Could not find a setter for property " + propertyName + " in class " + type.FullName );
			return result;
		}

		public IGetter GetGetter(System.Type theClass, string propertyName) 
		{
			BasicGetter result = GetGetterOrNull(theClass, propertyName);
			if (result == null) throw new PropertyNotFoundException( "Could not find a setter for property " + propertyName + " in class " + theClass.FullName );
			return result;
		}

		#endregion

		internal static BasicSetter GetSetterOrNull(System.Type type, string propertyName) 
		{
			if (type == typeof(object) || type == null) return null;

			//PropertyInfo property = type.GetProperty(propertyName);
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);

			if (property != null) 
			{
				return new BasicSetter(type, property, propertyName);
			} 
			else 
			{
				BasicSetter setter = GetSetterOrNull( type.BaseType, propertyName );
				if (setter == null) 
				{
					System.Type[] interfaces = type.GetInterfaces();
					for ( int i=0; setter==null && i<interfaces.Length; i++) 
					{
						setter = GetSetterOrNull(interfaces[i], propertyName);
					}
				}
				return setter;
			}
		}
	
		internal static BasicGetter GetGetterOrNull(System.Type type, string propertyName) 
		{
			if (type==typeof(object) || type==null) return null;

			//PropertyInfo property = type.GetProperty(propertyName);
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);

			if (property != null) 
			{
				return new BasicGetter(type, property, propertyName);
			} 
			else 
			{
				BasicGetter getter = GetGetterOrNull( type.BaseType, propertyName );
				if (getter == null) 
				{
					System.Type[] interfaces = type.GetInterfaces();
					for (int i=0; getter==null && i<interfaces.Length; i++) 
					{
						getter = GetGetterOrNull( interfaces[i], propertyName );
					}
				}
				return getter;
			}

		}
		}
}
