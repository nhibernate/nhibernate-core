using System;
using System.Collections;
using System.Reflection;
using NHibernate.Property;
using NHibernate.Type;

namespace NHibernate.Util
{
	/// <summary>
	/// Helper class for Reflection related code.
	/// </summary>
	public sealed class ReflectHelper
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof( ReflectHelper ) );

		private ReflectHelper()
		{
			// not creatable
		}

		private static System.Type[ ] NoClasses = new System.Type[0];

		/// <summary>
		/// Determine if the specified <see cref="System.Type"/> overrides the
		/// implementation of Equals from <see cref="Object"/>
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to reflect.</param>
		/// <returns><c>true</c> if any type in the heirarchy overrides Equals(object).</returns>
		public static bool OverridesEquals( System.Type clazz )
		{
			try
			{
				MethodInfo equals = clazz.GetMethod( "Equals", new System.Type[ ] {typeof( object )} );
				if( equals == null )
				{
					return false;
				}
				else
				{
					// make sure that the DeclaringType is not System.Object - if that is the
					// declaring type then there is no override.
					return !equals.DeclaringType.Equals( typeof( object ) );
				}
			}
			catch( AmbiguousMatchException )
			{
				// an ambigious match means that there is an override and it
				// can't determine which one to use.
				return true;
			}

		}

		/// <summary>
		/// Finds the <see cref="IGetter"/> for the property in the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccessorName"></param>
		/// <returns></returns>
		/// <remarks>
		/// This one takes a propertyAccessor name as we might know the correct strategy by now so we avoid Exceptions which are costly
		/// </remarks>
		public static IGetter GetGetter( System.Type theClass, string propertyName, string propertyAccessorName )
		{
			IPropertyAccessor accessor = null;

			try
			{
				// first try the named strategy since that will be the most likely strategy used.
				accessor = ( IPropertyAccessor ) PropertyAccessorFactory.PropertyAccessors[ propertyAccessorName ];
				return accessor.GetGetter( theClass, propertyName );
			}
			catch( PropertyNotFoundException )
			{
				// the basic named strategy did not work so try the rest of them
				foreach( DictionaryEntry de in PropertyAccessorFactory.PropertyAccessors )
				{
					try
					{
						accessor = ( IPropertyAccessor ) de.Value;
						return accessor.GetGetter( theClass, propertyName );
					}
					catch( PropertyNotFoundException )
					{
						// ignore this exception because we want to try and move through
						// the rest of the accessor strategies.
					}
				}
				throw;
			}
		}

		//TODO: most calls to this will be replaced by the Mapping.Property.GetGetter() but
		// there will still be a call from hql into this.
		/// <summary>
		/// Finds the <see cref="IGetter"/> for the property in the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property/Field in.</param>
		/// <param name="propertyName">The name of the Property/Field to find.</param>
		/// <returns>The <see cref="IGetter"/> for the property.</returns>
		/// <remarks>
		/// <para>
		/// This does not use the <c>access=""</c> attribute specified in the mapping.  It
		/// first checks to see if there is a Property in your class with the same name.  If
		/// no Property is found then it moves through each <see cref="IPropertyAccessor"/> strategy
		/// and tries to find an <see cref="IGetter"/> through them.
		/// </para>
		/// </remarks>
		/// <exception cref="PropertyNotFoundException">
		/// No Property or Field with the <c>propertyName</c> could be found.
		/// </exception>
		public static IGetter GetGetter( System.Type theClass, string propertyName )
		{
			// first try the basicPropertyAccessor since that will be the most likely strategy used.
			return GetGetter( theClass, propertyName, "property" );
		}

		//TODO: add a method in here ReflectedPropertyClass and replace most calls to GetGetter
		// with calls to it


		/// <summary>
		/// Get the NHibernate <see cref="IType" /> for the named property of the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="name">The name of the property/field to find in the class.</param>
		/// <param name="propertyAccess"></param>
		/// <returns>
		/// The NHibernate <see cref="IType"/> for the named property.
		/// </returns>
		public static IType ReflectedPropertyType( System.Type theClass, string name, string propertyAccess )
		{
			return TypeFactory.HueristicType( GetGetter( theClass, name, propertyAccess ).ReturnType.AssemblyQualifiedName );
		}

		/// <summary>
		/// Get the <see cref="System.Type" /> for the named property of a type.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the property in.</param>
		/// <param name="name">The name of the property/field to find in the class.</param>
		/// <returns>The <see cref="System.Type" /> for the named property.</returns>
		public static System.Type ReflectedPropertyClass( System.Type theClass, string name)
		{
			return GetGetter(theClass, name).ReturnType;
		}

		/// <summary>
		/// Returns a reference to the Type.
		/// </summary>
		/// <param name="name">The name of the class.  Can be a name with the assembly included or just the name of the class.</param>
		/// <returns>The Type for the Class.</returns>
		public static System.Type ClassForName( string name )
		{
			return System.Type.GetType( name, true );
		}

		/// <summary>
		/// Returns a <see cref="System.Type"/> from an already loaded Assembly or an
		/// Assembly that is loaded with a partial name.
		/// </summary>
		/// <param name="className">The full name of the class.</param>
		/// <param name="assemblyName">
		/// The name of the assembly.  This can be the full assembly name or just the partial name.
		/// </param>
		/// <returns>
		/// The <see cref="System.Type"/> for the class in the assembly or 
		/// <c>null</c> if a <see cref="System.Type"/> can't be found.
		/// </returns>
		/// <remarks>
		/// Attempts to get a reference to the type from an already loaded assembly.  If the 
		/// Type can be found then the Assembly is loaded using LoadWithPartialName.
		/// </remarks>
		public static System.Type TypeFromAssembly( string className, string assemblyName ) 
		{
			try 
			{
				// try to get the Types from an already loaded assembly
				System.Type type = System.Type.GetType( className + ", " + assemblyName );

				// if the type is null then the assembly is not loaded.
				if( type==null ) 
				{
					// use the partial name because we don't know the public key, version, culture-info of
					// the assembly on the local machine.
					Assembly assembly = Assembly.LoadWithPartialName( assemblyName );
					if( assembly!=null ) 
					{
						type = assembly.GetType( className );
					}
				}

				return type;
			}
			catch( Exception e )
			{
				if( log.IsErrorEnabled ) 
				{
					log.Error( className + ", " + assemblyName + " could not be loaded.", e );
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the value contained in the static field.
		/// </summary>
		/// <param name="typeName">The name of the <see cref="System.Type"/>.</param>
		/// <param name="fieldName">The name of the Field in the <see cref="System.Type"/>.</param>
		/// <returns>The value contained in that field or <c>null</c> if the Type or Field does not exist.</returns>
		public static object GetConstantValue( string typeName, string fieldName )
		{
			System.Type clazz = System.Type.GetType( typeName, false );

			if( clazz == null ) return null;

			try
			{
				return clazz.GetField( fieldName ).GetValue( null );
			}
			catch( Exception )
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static object GetConstantValue( string name )
		{
			System.Type clazz;

			try
			{
				clazz = ClassForName( StringHelper.Qualifier( name ) );
			}
			catch( Exception )
			{
				return null;
			}

			try
			{
				return clazz.GetField( StringHelper.Unqualify( name ) ).GetValue( null );
			}
			catch( Exception )
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the default no arg constructor for the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the constructor for.</param>
		/// <returns>
		/// The <see cref="ConstructorInfo"/> for the no argument constructor, or <c>null</c> if a 
		/// default constructor could not be found.
		/// </returns>
		/// <exception cref="InstantiationException">
		/// Thrown when there is a problem calling the method GetConstructor on <see cref="System.Type"/>.
		/// </exception>
		public static ConstructorInfo GetDefaultConstructor( System.Type type )
		{
			if( IsAbstractClass( type ) ) return null;

			try
			{
				ConstructorInfo contructor = type.GetConstructor( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.HasThis, NoClasses, null );
				return contructor;
			}
			catch( Exception )
			{
				throw new InstantiationException(
					"Object class " + type.FullName + " must declare a default (no-arg) constructor"
					);
			}
		}

		/// <summary>
		/// Determines if the <see cref="System.Type"/> is a non creatable class.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to check.</param>
		/// <returns><c>true</c> if the <see cref="System.Type"/> is an Abstract Class or an Interface.</returns>
		public static bool IsAbstractClass( System.Type type )
		{
			return ( type.IsAbstract || type.IsInterface );
		}

	}
}