using System.Reflection;
using log4net;

namespace NHibernate.Property
{
	/// <summary>
	/// Accesses mapped property values via a get/set pair, which may be nonpublic.
	/// The default (and recommended strategy).
	/// </summary>
	public class BasicPropertyAccessor : IPropertyAccessor
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( BasicPropertyAccessor ) );

		#region IPropertyAccessor Members

		/// <summary>
		/// Create a <see cref="BasicGetter"/> for the mapped property.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public IGetter GetGetter( System.Type theClass, string propertyName )
		{
			BasicGetter result = GetGetterOrNull( theClass, propertyName );
			if( result == null )
			{
				throw new PropertyNotFoundException( "Could not find a getter for property " + propertyName + " in class " + theClass.FullName );
			}
			return result;
		}

		/// <summary>
		/// Create a <see cref="BasicSetter"/> for the mapped property.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicSetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public ISetter GetSetter( System.Type type, string propertyName )
		{
			BasicSetter result = GetSetterOrNull( type, propertyName );
			if( result == null )
			{
				throw new PropertyNotFoundException( "Could not find a setter for property " + propertyName + " in class " + type.FullName );
			}
			return result;
		}

		#endregion

		/// <summary>
		/// Helper method to find the Property <c>get</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicGetter"/> for the Property <c>get</c> or <c>null</c>
		/// if the Property could not be found.
		/// </returns>
		internal static BasicGetter GetGetterOrNull( System.Type type, string propertyName )
		{
			
			if( type==typeof( object ) || type==null )
			{
				// the full inheritance chain has been walked and we could
				// not find the Property get
				return null;
			}

			PropertyInfo property = type.GetProperty( propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

			if( property != null )
			{
				return new BasicGetter( type, property, propertyName );
			}
			else
			{
				// recursively call this method for the base Type
				BasicGetter getter = GetGetterOrNull( type.BaseType, propertyName );
				
				// didn't find anything in the base class - check to see if there is 
				// an explicit interface implementation.
				if( getter == null )
				{
					System.Type[ ] interfaces = type.GetInterfaces();
					for( int i = 0; getter == null && i < interfaces.Length; i++ )
					{
						getter = GetGetterOrNull( interfaces[ i ], propertyName );
					}
				}
				return getter;
			}

		}
		
		/// <summary>
		/// Helper method to find the Property <c>set</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="BasicSetter"/> for the Property <c>set</c> or <c>null</c>
		/// if the Property could not be found.
		/// </returns>
		internal static BasicSetter GetSetterOrNull( System.Type type, string propertyName )
		{
			if( type == typeof( object ) || type == null )
			{
				// the full inheritance chain has been walked and we could
				// not find the Property get
				return null;
			}

			PropertyInfo property = type.GetProperty( propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

			if( property != null )
			{
				return new BasicSetter( type, property, propertyName );
			}
			else
			{
				// recursively call this method for the base Type
				BasicSetter setter = GetSetterOrNull( type.BaseType, propertyName );
				
				// didn't find anything in the base class - check to see if there is 
				// an explicit interface implementation.
				if( setter == null )
				{
					System.Type[ ] interfaces = type.GetInterfaces();
					for( int i = 0; setter == null && i < interfaces.Length; i++ )
					{
						setter = GetSetterOrNull( interfaces[ i ], propertyName );
					}
				}
				return setter;
			}
		}
	}
}