using System;
using System.Reflection;
using NHibernate.Type;

namespace NHibernate.Util 
{
	
	public sealed class ReflectHelper
	{
		//TODO: this dependency is kind of bad - H2.0.3 comment
		private static readonly Property.BasicPropertyAccessor basicPropertyAccessor = new Property.BasicPropertyAccessor();
		
		private static System.Type[] NoClasses = new System.Type[0];
		private static System.Type[] Object = new System.Type[] { typeof(object) };
		private static MethodInfo ObjectEquals;

		static ReflectHelper() 
		{
			MethodInfo eq;
			try 
			{
				eq = typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public );
			} 
			catch (Exception e) 
			{
				throw new AssertionFailure("Could not find Object.Equals()", e);
			}
			ObjectEquals = eq;
		}

		public static bool OverridesEquals(System.Type clazz) 
		{
			MethodInfo equals;
			try 
			{
				equals = clazz.GetMethod("Equals");
			} 
			catch (Exception) 
			{
				return false;
			}
			return !ObjectEquals.Equals(equals);
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

		public static object GetConstantValue(string name) 
		{
			System.Type clazz;
			try 
			{
				clazz = ClassForName( StringHelper.Qualifier(name) );
			} 
			catch(Exception) 
			{
				return null;
			}
			try 
			{
				return clazz.GetProperty( StringHelper.Unqualify(name) ).GetValue(null, new object[0]);
				//??
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
