using System;
using System.Reflection;
using NHibernate.Type;

namespace NHibernate.Util 
{
	/// <summary>
	/// Helper class for Reflection related code.
	/// </summary>
	public sealed class ReflectHelper
	{
		//TODO: this dependency is kind of bad - H2.0.3 comment
		private static readonly Property.BasicPropertyAccessor basicPropertyAccessor = new Property.BasicPropertyAccessor();
		
		private static System.Type[] NoClasses = new System.Type[0];
		private static System.Type[] Object = new System.Type[] { typeof(object) };
		
		/// <summary>
		/// Determine if the specified <see cref="System.Type"/> overrides the
		/// implementation of Equals from <see cref="Object"/>
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to reflect.</param>
		/// <returns><c>true</c> if any type in the heirarchy overrides Equals(object).</returns>
		public static bool OverridesEquals(System.Type clazz) 
		{
			try 
			{
				MethodInfo equals = clazz.GetMethod("Equals", new System.Type[] { typeof(object) });
				if( equals==null ) 
				{
					return false;
				}
				else 
				{
					// make sure that the DeclaringType is not System.Object - if that is the
					// declaring type then there is no override.
					return !equals.DeclaringType.Equals( typeof(object) );
				}
			} 
			catch( AmbiguousMatchException ) 
			{
				// an ambigious match means that there is an override and it
				// can't determine which one to use.
				return true;
			}
			catch (Exception ) 
			{
				return false;
			}
			
		}

		//TODO: most calls to this will be replaced by the Mapping.Property.GetGetter() but
		// there will still be a call from hql into this.
		public static Property.IGetter GetGetter(System.Type theClass, string propertyName) 
		{
			try 
			{
				return basicPropertyAccessor.GetGetter(theClass, propertyName);
			}
			catch (PropertyNotFoundException pnfe) 
			{
				//TODO: figure out how we are going to know the field accessor to use here...
				throw pnfe;
			}
		}

		//TODO: add a method in here ReflectedPropertyClass and replace most calls to GetGetter
		// with calls to it


		public static IType ReflectedPropertyType(System.Type theClass, string name) 
		{
			return TypeFactory.HueristicType( GetGetter(theClass, name).ReturnType.AssemblyQualifiedName ); 
		}

		/// <summary>
		/// Returns a reference to the Type.
		/// </summary>
		/// <param name="name">The name of the class.  Can be a name with the assembly included or just the name of the class.</param>
		/// <returns>The Type for the Class.</returns>
		public static System.Type ClassForName(string name) 
		{
			return System.Type.GetType(name, true);
		}

		/// <summary>
		/// Returns the value contained in the static field.
		/// </summary>
		/// <param name="typeName">The name of the <see cref="System.Type"/>.</param>
		/// <param name="fieldName">The name of the Field in the <see cref="System.Type"/>.</param>
		/// <returns>The value contained in that field or <c>null</c> if the Type or Field does not exist.</returns>
		public static object GetConstantValue(string typeName, string fieldName) 
		{
			System.Type clazz = System.Type.GetType( typeName, false ); 
			
			if( clazz==null ) return null;

			try 
			{
				return clazz.GetField( fieldName ).GetValue( null );
			} 
			catch (Exception) 
			{
				return null;
			}
		}

		public static ConstructorInfo GetDefaultConstructor(System.Type type) 
		{
			if (IsAbstractClass(type)) return null;
			
			try 
			{
				ConstructorInfo contructor = type.GetConstructor(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, null, CallingConventions.HasThis, NoClasses, null);
				return contructor;
			} 
			catch (Exception) 
			{
				throw new PropertyNotFoundException(
					"Object class " + type.FullName + " must declare a default (no-arg) constructor"
					);
			}
		}

		public static bool IsAbstractClass(System.Type type) 
		{
			return (type.IsAbstract || type.IsInterface);
		}

	}
}
