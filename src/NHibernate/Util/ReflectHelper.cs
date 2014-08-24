using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.Engine;

namespace NHibernate.Util
{
	/// <summary>
	/// Helper class for Reflection related code.
	/// </summary>
	public static class ReflectHelper
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ReflectHelper));

		public const BindingFlags AnyVisibilityInstance = BindingFlags.Instance | BindingFlags.Public |
														   BindingFlags.NonPublic;

		private static readonly System.Type[] NoClasses = System.Type.EmptyTypes;

		private static readonly MethodInfo Exception_InternalPreserveStackTrace =
			typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		/// <summary>
		/// Determine if the specified <see cref="System.Type"/> overrides the
		/// implementation of Equals from <see cref="Object"/>
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to reflect.</param>
		/// <returns><see langword="true" /> if any type in the hierarchy overrides Equals(object).</returns>
		public static bool OverridesEquals(System.Type clazz)
		{
			return OverrideMethod(clazz, "Equals", new[] { typeof(object) });
		}

		private static bool OverrideMethod(System.Type clazz, string methodName, System.Type[] parametersTypes)
		{
			try
			{
				MethodInfo method = !clazz.IsInterface
										? clazz.GetMethod(methodName, parametersTypes)
										: GetMethodFromInterface(clazz, methodName, parametersTypes);
				if (method == null)
				{
					return false;
				}
				else
				{
					// make sure that the DeclaringType is not System.Object - if that is the
					// declaring type then there is no override.
					return !(method.DeclaringType == typeof(object));
				}
			}
			catch (AmbiguousMatchException)
			{
				// an ambigious match means that there is an override and it
				// can't determine which one to use.
				return true;
			}
		}

		private static MethodInfo GetMethodFromInterface(System.Type type, string methodName, System.Type[] parametersTypes)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
			if (type == null)
			{
				return null;
			}
			MethodInfo method = type.GetMethod(methodName, flags, null, parametersTypes, null);
			if (method == null)
			{
				System.Type[] interfaces = type.GetInterfaces();
				foreach (var @interface in interfaces)
				{
					method = GetMethodFromInterface(@interface, methodName, parametersTypes);
					if (method != null)
					{
						return method;
					}
				}
			}
			return method;
		}

		/// <summary>
		/// Determine if the specified <see cref="System.Type"/> overrides the
		/// implementation of GetHashCode from <see cref="Object"/>
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to reflect.</param>
		/// <returns><see langword="true" /> if any type in the hierarchy overrides GetHashCode().</returns>
		public static bool OverridesGetHashCode(System.Type clazz)
		{
			return OverrideMethod(clazz, "GetHashCode", System.Type.EmptyTypes);
		}

		/// <summary>
		/// Finds the <see cref="IGetter"/> for the property in the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the property in.</param>
		/// <param name="propertyName">The name of the Property to find.</param>
		/// <param name="propertyAccessorName">The name of the property access strategy.</param>
		/// <returns>The <see cref="IGetter"/> to get the value of the Property.</returns>
		/// <remarks>
		/// This one takes a propertyAccessor name as we might know the correct strategy by now so we avoid Exceptions which are costly
		/// </remarks>
		public static IGetter GetGetter(System.Type theClass, string propertyName, string propertyAccessorName)
		{
			return PropertyAccessorFactory
				.GetPropertyAccessor(propertyAccessorName)
				.GetGetter(theClass, propertyName);
		}

		/// <summary>
		/// Get the NHibernate <see cref="IType" /> for the named property of the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="name">The name of the property/field to find in the class.</param>
		/// <param name="access">The name of the property accessor for the property.</param>
		/// <returns>
		/// The NHibernate <see cref="IType"/> for the named property.
		/// </returns>
		public static IType ReflectedPropertyType(System.Type theClass, string name, string access)
		{
			System.Type propertyClass = ReflectedPropertyClass(theClass, name, access);

			var heuristicClass = propertyClass.UnwrapIfNullable();

			return TypeFactory.HeuristicType(heuristicClass.AssemblyQualifiedName);
		}

		/// <summary>
		/// Get the <see cref="System.Type" /> for the named property of a type.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the property in.</param>
		/// <param name="name">The name of the property/field to find in the class.</param>
		/// <param name="access">The name of the property accessor for the property.</param>
		/// <returns>The <see cref="System.Type" /> for the named property.</returns>
		public static System.Type ReflectedPropertyClass(System.Type theClass, string name, string access)
		{
			return GetGetter(theClass, name, access).ReturnType;
		}

		/// <summary>
		/// Get the <see cref="System.Type" /> for the named property of a type.
		/// </summary>
		/// <param name="className">The FullName to find the property in.</param>
		/// <param name="name">The name of the property/field to find in the class.</param>
		/// <param name="accessorName">The name of the property accessor for the property.</param>
		/// <returns>The <see cref="System.Type" /> for the named property.</returns>
		public static System.Type ReflectedPropertyClass(string className, string name, string accessorName)
		{
			try
			{
				System.Type clazz = ClassForName(className);
				return GetGetter(clazz, name, accessorName).ReturnType;
			}
			catch (Exception cnfe)
			{
				throw new MappingException(string.Format("class {0} not found while looking for property: {1}", className, name), cnfe);
			}
		}

		/// <summary>
		/// Returns a reference to the Type.
		/// </summary>
		/// <param name="name">The name of the class or a fully qualified name.</param>
		/// <returns>The Type for the Class.</returns>
		public static System.Type ClassForName(string name)
		{
			AssemblyQualifiedTypeName parsedName = TypeNameParser.Parse(name);
			System.Type result = TypeFromAssembly(parsedName, true);
			return result;
		}

		/// <summary>
		/// Load a System.Type given is't name.
		/// </summary>
		/// <param name="classFullName">The class FullName or AssemblyQualifiedName</param>
		/// <returns>The System.Type</returns>
		/// <remarks>
		/// If the <paramref name="classFullName"/> don't represent an <see cref="System.Type.AssemblyQualifiedName"/>
		/// the method try to find the System.Type scanning all Assemblies of the <see cref="AppDomain.CurrentDomain"/>.
		/// </remarks>
		/// <exception cref="TypeLoadException">If no System.Type was found for <paramref name="classFullName"/>.</exception>
				public static System.Type ClassForFullName(string classFullName)
		{
			var result = ClassForFullNameOrNull(classFullName);
			if (result == null)
			{
				string message = "Could not load type " + classFullName + ". Possible cause: the assembly was not loaded or not specified.";
				throw new TypeLoadException(message);
			}

			return result;
		}

		/// <summary>
				/// Load a System.Type given is't name.
				/// </summary>
				/// <param name="classFullName">The class FullName or AssemblyQualifiedName</param>
				/// <returns>The System.Type or null</returns>
				/// <remarks>
				/// If the <paramref name="classFullName"/> don't represent an <see cref="System.Type.AssemblyQualifiedName"/>
				/// the method try to find the System.Type scanning all Assemblies of the <see cref="AppDomain.CurrentDomain"/>.
				/// </remarks>
				public static System.Type ClassForFullNameOrNull(string classFullName)
				{
					System.Type result = null;
					AssemblyQualifiedTypeName parsedName = TypeNameParser.Parse(classFullName);
					if (!string.IsNullOrEmpty(parsedName.Assembly))
					{
						result = TypeFromAssembly(parsedName, false);
					}
					else
					{
						if (!string.IsNullOrEmpty(classFullName))
						{
							Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
							foreach (Assembly a in ass)
							{
								result = a.GetType(classFullName, false, false);
								if (result != null)
									break; //<<<<<================
							}
						}
					}

					return result;
				}

		public static System.Type TypeFromAssembly(string type, string assembly, bool throwIfError)
		{
			return TypeFromAssembly(new AssemblyQualifiedTypeName(type, assembly), throwIfError);
		}

		/// <summary>
		/// Returns a <see cref="System.Type"/> from an already loaded Assembly or an
		/// Assembly that is loaded with a partial name.
		/// </summary>
		/// <param name="name">An <see cref="AssemblyQualifiedTypeName" />.</param>
		/// <param name="throwOnError"><see langword="true" /> if an exception should be thrown
		/// in case of an error, <see langword="false" /> otherwise.</param>
		/// <returns>
		/// A <see cref="System.Type"/> object that represents the specified type,
		/// or <see langword="null" /> if the type cannot be loaded.
		/// </returns>
		/// <remarks>
		/// Attempts to get a reference to the type from an already loaded assembly.  If the 
		/// type cannot be found then the assembly is loaded using
		/// <see cref="Assembly.Load(string)" />.
		/// </remarks>
		public static System.Type TypeFromAssembly(AssemblyQualifiedTypeName name, bool throwOnError)
		{
			try
			{
				// Try to get the type from an already loaded assembly
				System.Type type = System.Type.GetType(name.ToString());

				if (type != null)
				{
					return type;
				}

				if (name.Assembly == null)
				{
					// No assembly was specified for the type, so just fail
					string message = "Could not load type " + name + ". Possible cause: no assembly name specified.";
					log.Warn(message);
					if (throwOnError) throw new TypeLoadException(message);
					return null;
				}

				Assembly assembly = Assembly.Load(name.Assembly);

				if (assembly == null)
				{
					log.Warn("Could not load type " + name + ". Possible cause: incorrect assembly name specified.");
					return null;
				}

				type = assembly.GetType(name.Type, throwOnError);

				if (type == null)
				{
					log.Warn("Could not load type " + name + ".");
					return null;
				}

				return type;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Could not load type " + name + ".", e);
				}
				if (throwOnError) throw;
				return null;
			}
		}

		public static bool TryLoadAssembly(string assemblyName)
		{
			if (string.IsNullOrEmpty(assemblyName))
				return false;

			bool result = true;
			try
			{
				Assembly.Load(assemblyName);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the static field <paramref name="fieldName"/> of <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> .</param>
		/// <param name="fieldName">The name of the field in the <paramref name="type"/>.</param>
		/// <returns>The value contained in the field, or <see langword="null" /> if the type or the field does not exist.</returns>
		public static object GetConstantValue(System.Type type, string fieldName)
		{
			try
			{
				FieldInfo field = type.GetField(fieldName);
				if (field == null)
				{
					return null;
				}
				return field.GetValue(null);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the default no arg constructor for the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the constructor for.</param>
		/// <returns>
		/// The <see cref="ConstructorInfo"/> for the no argument constructor, or <see langword="null" /> if the
		/// <c>type</c> is an abstract class.
		/// </returns>
		/// <exception cref="InstantiationException">
		/// Thrown when there is a problem calling the method GetConstructor on <see cref="System.Type"/>.
		/// </exception>
		public static ConstructorInfo GetDefaultConstructor(System.Type type)
		{
			if (IsAbstractClass(type))
				return null;

			try
			{
				ConstructorInfo constructor =
					type.GetConstructor(AnyVisibilityInstance, null, CallingConventions.HasThis, NoClasses, null);
				return constructor;
			}
			catch (Exception e)
			{
				throw new InstantiationException("A default (no-arg) constructor could not be found for: ", e, type);
			}
		}

		/// <summary>
		/// Finds the constructor that takes the parameters.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the constructor in.</param>
		/// <param name="types">The <see cref="IType"/> objects to use to find the appropriate constructor.</param>
		/// <returns>
		/// An <see cref="ConstructorInfo"/> that can be used to create the type with
		/// the specified parameters.
		/// </returns>
		/// <exception cref="InstantiationException">
		/// Thrown when no constructor with the correct signature can be found.
		/// </exception>
		public static ConstructorInfo GetConstructor(System.Type type, IType[] types)
		{
			ConstructorInfo[] candidates = type.GetConstructors(AnyVisibilityInstance);

			foreach (ConstructorInfo constructor in candidates)
			{
				ParameterInfo[] parameters = constructor.GetParameters();

				if (parameters.Length == types.Length)
				{
					bool found = true;

					for (int j = 0; j < parameters.Length; j++)
					{
						bool ok = parameters[j].ParameterType.IsAssignableFrom(
							types[j].ReturnedClass);

						if (!ok)
						{
							found = false;
							break;
						}
					}

					if (found)
					{
						return constructor;
					}
				}
			}

			throw new InstantiationException(FormatConstructorNotFoundMessage(types), null, type);
		}

		private static string FormatConstructorNotFoundMessage(IEnumerable<IType> types)
		{
			var result = new StringBuilder("no constructor compatible with (");
			bool first = true;
			foreach (IType type in types)
			{
				if (!first)
				{
					result.Append(", ");
				}
				first = false;
				result.Append(type.ReturnedClass);
			}
			result.Append(") found in class: ");
			return result.ToString();
		}

		/// <summary>
		/// Determines if the <see cref="System.Type"/> is a non creatable class.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to check.</param>
		/// <returns><see langword="true" /> if the <see cref="System.Type"/> is an Abstract Class or an Interface.</returns>
		public static bool IsAbstractClass(System.Type type)
		{
			return (type.IsAbstract || type.IsInterface);
		}

		public static bool IsFinalClass(System.Type type)
		{
			return type.IsSealed;
		}

		/// <summary>
		/// Unwraps the supplied <see cref="System.Reflection.TargetInvocationException"/> 
		/// and returns the inner exception preserving the stack trace.
		/// </summary>
		/// <param name="ex">
		/// The <see cref="System.Reflection.TargetInvocationException"/> to unwrap.
		/// </param>
		/// <returns>The unwrapped exception.</returns>
		public static Exception UnwrapTargetInvocationException(TargetInvocationException ex)
		{
			Exception_InternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] { });
			return ex.InnerException;
		}

		/// <summary>
		/// Try to find a method in a given type.
		/// </summary>
		/// <param name="type">The given type.</param>
		/// <param name="method">The method info.</param>
		/// <returns>The found method or null.</returns>
		/// <remarks>
		/// The <paramref name="method"/>, in general, become from another <see cref="Type"/>.
		/// </remarks>
		public static MethodInfo TryGetMethod(System.Type type, MethodInfo method)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (method == null)
			{
				return null;
			}

			System.Type[] tps = GetMethodSignature(method);
			return SafeGetMethod(type, method, tps);
		}

		private static System.Type[] GetMethodSignature(MethodInfo method)
		{
			var pi = method.GetParameters();
			var tps = new System.Type[pi.Length];
			for (int i = 0; i < pi.Length; i++)
			{
				tps[i] = pi[i].ParameterType;
			}
			return tps;
		}

		private static MethodInfo SafeGetMethod(System.Type type, MethodInfo method, System.Type[] tps)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			List<System.Type> typesToSearch = new List<System.Type>();
			MethodInfo foundMethod = null;
			
			try
			{            
				typesToSearch.Add(type);
			
				if (type.IsInterface)
				{
					// Methods on parent interfaces are not actually inherited
					// by child interfaces, so we have to use GetInterfaces to
					// identify any parent interfaces that may contain the
					// method implementation
					System.Type[] inheritedInterfaces = type.GetInterfaces();
					typesToSearch.AddRange(inheritedInterfaces);
				}

				foreach (System.Type typeToSearch in typesToSearch)
				{
					MethodInfo result = typeToSearch.GetMethod(method.Name, bindingFlags, null, tps, null);
					if (result != null)
					{
						foundMethod = result;
						break;
					}
				}
			}
			catch (Exception)
			{
			   throw;
			}
			
			return foundMethod;
		}

		internal static object GetConstantValue(string qualifiedName)
		{
			return GetConstantValue(qualifiedName, null);
		}

		internal static object GetConstantValue(string qualifiedName, ISessionFactoryImplementor sfi)
		{
			string className = StringHelper.Qualifier(qualifiedName);

			if (!string.IsNullOrEmpty(className))
			{
				System.Type t = System.Type.GetType(className);

				if (t == null && sfi != null)
				{
					t = System.Type.GetType(sfi.GetImportedClassName(className));
				}

				if (t != null)
				{
					return GetConstantValue(t, StringHelper.Unqualify(qualifiedName));
				}
			}

			return null;
		}

		public static MethodInfo GetGenericMethodFrom<T>(string methodName, System.Type[] genericArgs, System.Type[] signature)
		{
			MethodInfo result = null;
			MethodInfo[] methods = typeof(T).GetMethods();
			foreach (var method in methods)
			{
				if (method.Name.Equals(methodName) && method.IsGenericMethod
						&& signature.Length == method.GetParameters().Length
					&& method.GetGenericArguments().Length == genericArgs.Length)
				{
					bool foundCandidate = true;
					result = method.MakeGenericMethod(genericArgs);

					ParameterInfo[] ms = result.GetParameters();
					for (int i = 0; i < signature.Length; i++)
					{
						if (ms[i].ParameterType != signature[i])
						{
							foundCandidate = false;
						}
					}

					if (foundCandidate)
					{
						return result;
					}
				}
			}
			return result;
		}

		public static IDictionary<string, string> ToTypeParameters(this object source)
		{
			if (source == null)
			{
				return new Dictionary<string, string>(1);
			}
			var props = source.GetType().GetProperties();
			if (props.Length == 0)
			{
				return new Dictionary<string, string>(1);
			}
			var result = new Dictionary<string, string>(props.Length);
			foreach (var prop in props)
			{
				var value = prop.GetValue(source, null);
				if (!ReferenceEquals(null, value))
				{
					result[prop.Name] = value.ToString();
				}
			}
			return result;
		}

		public static bool IsPropertyGet(MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("get_");
		}

		public static bool IsPropertySet(MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("set_");
		}

		public static string GetPropertyName(MethodInfo method)
		{
			return method.Name.Substring(4);
		}

		public static System.Type GetCollectionElementType(this IEnumerable collectionInstance)
		{
			if (collectionInstance == null)
			{
				throw new ArgumentNullException("collectionInstance");
			}
			var collectionType = collectionInstance.GetType();
				return GetCollectionElementType(collectionType);
		}

			public static System.Type GetCollectionElementType(System.Type collectionType)
			{
				if (collectionType == null)
				{
					throw new ArgumentNullException("collectionType");
				}
				if (collectionType.IsArray)
				{
					return collectionType.GetElementType();
				}
				if (collectionType.IsGenericType)
				{
					List<System.Type> interfaces = collectionType.GetInterfaces().Where(t => t.IsGenericType).ToList();
					if (collectionType.IsInterface)
					{
						interfaces.Add(collectionType);
					}
					var enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
					if (enumerableInterface != null)
					{
						return enumerableInterface.GetGenericArguments()[0];
					}
				}
				return null;
			}

			/// <summary>
			/// Try to find a property, that can be managed by NHibernate, from a given type.
			/// </summary>
			/// <param name="source">The given <see cref="System.Type"/>. </param>
			/// <param name="propertyName">The name of the property to find.</param>
			/// <returns>true if the property exists; otherwise false.</returns>
			/// <remarks>
			/// When the user defines a field.xxxxx access strategy should be because both the property and the field exists.
			/// NHibernate can work even when the property does not exist but in this case the user should use the appropiate accessor.
			/// </remarks>
			public static bool HasProperty(this System.Type source, string propertyName)
			{
				if (source == typeof (object) || source == null)
				{
					return false;
				}
				if (string.IsNullOrEmpty(propertyName))
				{
					return false;
				}

				PropertyInfo property = source.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				if (property != null)
				{
					return true;
				}
				return HasProperty(source.BaseType, propertyName) || source.GetInterfaces().Any(@interface => HasProperty(@interface, propertyName));
			}

					/// <summary>
		/// Check if a method is declared in a given <see cref="System.Type"/>.
		/// </summary>
		/// <param name="source">The method to check.</param>
		/// <param name="realDeclaringType">The where the method is really declared.</param>
		/// <returns>True if the method is an implementation of the method declared in <paramref name="realDeclaringType"/>; false otherwise. </returns>
		public static bool IsMethodOf(this MethodInfo source, System.Type realDeclaringType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (realDeclaringType == null)
			{
				throw new ArgumentNullException("realDeclaringType");
			}
			var methodDeclaringType = source.DeclaringType;
			if(realDeclaringType == methodDeclaringType)
			{
				return true;
			}
			if (methodDeclaringType.IsGenericType && !methodDeclaringType.IsGenericTypeDefinition && 
				realDeclaringType == methodDeclaringType.GetGenericTypeDefinition())
			{
				return true;
			}
			if (realDeclaringType.IsInterface)
			{
				var declaringTypeInterfaces = methodDeclaringType.GetInterfaces();
				if(declaringTypeInterfaces.Contains(realDeclaringType))
				{
					var methodsMap = methodDeclaringType.GetInterfaceMap(realDeclaringType);
					if(methodsMap.TargetMethods.Contains(source))
					{
						return true;
					}
				}
				if (realDeclaringType.IsGenericTypeDefinition)
				{
					bool implements = declaringTypeInterfaces
						.Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == realDeclaringType)
						.Select(implementedGenericInterface => methodDeclaringType.GetInterfaceMap(implementedGenericInterface))
						.Any(methodsMap => methodsMap.TargetMethods.Contains(source));
					if (implements)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
