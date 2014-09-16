using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public static class TypeExtensions
	{
		private const BindingFlags PropertiesOfClassHierarchy =	BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		private const BindingFlags PropertiesOrFieldOfClass = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		public static IEnumerable<System.Type> GetBaseTypes(this System.Type type)
		{
			foreach (var @interface in type.GetInterfaces())
				yield return @interface;
			
			var analizing = type;
			while (analizing != null && analizing != typeof (object))
			{
				analizing = analizing.BaseType;
				yield return analizing;
			}
		}

		public static IEnumerable<System.Type> GetHierarchyFromBase(this System.Type type)
		{
			var typeHierarchy = new Stack<System.Type>();
			var analyzingType = type;
			while (analyzingType != null && analyzingType != typeof (object))
			{
				typeHierarchy.Push(analyzingType);
				analyzingType = analyzingType.BaseType;
			}
			return typeHierarchy;
		}

		public static System.Type GetPropertyOrFieldType(this MemberInfo propertyOrField)
		{
			if (propertyOrField.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo) propertyOrField).PropertyType;
			}

			if (propertyOrField.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo) propertyOrField).FieldType;
			}
			
			throw new ArgumentOutOfRangeException("propertyOrField",
												  "Expected PropertyInfo or FieldInfo; found :" + propertyOrField.MemberType);
		}

		public static MemberInfo DecodeMemberAccessExpression<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			return DecodeMemberAccessExpression<TEntity, object>(expression);
		}

		public static MemberInfo DecodeMemberAccessExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof (TProperty)))
				{
					return ((MemberExpression) ((UnaryExpression) expression.Body).Operand).Member;
				}
				throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
					expression.Body.NodeType));
			}
			return ((MemberExpression) expression.Body).Member;
		}

		/// <summary>
		/// Decode a member access expression of a specific ReflectedType
		/// </summary>
		/// <typeparam name="TEntity">Type to reflect</typeparam>
		/// <param name="expression">The expression of the property getter</param>
		/// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
		public static MemberInfo DecodeMemberAccessExpressionOf<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			return DecodeMemberAccessExpressionOf<TEntity, object>(expression);
		}

		/// <summary>
		/// Decode a member access expression of a specific ReflectedType
		/// </summary>
		/// <typeparam name="TEntity">Type to reflect</typeparam>
		/// <typeparam name="TProperty">Type of property</typeparam>
		/// <param name="expression">The expression of the property getter</param>
		/// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
		public static MemberInfo DecodeMemberAccessExpressionOf<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var memberOfDeclaringType = DecodeMemberAccessExpression(expression);
			if (typeof (TEntity).IsInterface)
			{
				// Type.GetProperty(string name,Type returnType) does not work properly with interfaces
				return memberOfDeclaringType;
			}

			return typeof (TEntity).GetProperty(memberOfDeclaringType.Name, PropertiesOfClassHierarchy,
			                                    null, memberOfDeclaringType.GetPropertyOrFieldType(), new System.Type[0], null);
		}

		public static MemberInfo GetMemberFromDeclaringType(this MemberInfo source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source.DeclaringType.Equals(source.ReflectedType))
			{
				return source;
			}

			if (source is PropertyInfo)
			{
				return source.DeclaringType.GetProperty(source.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			}
			if (source is FieldInfo)
			{
				return source.DeclaringType.GetField(source.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			}

			return null;
		}

		public static IEnumerable<MemberInfo> GetMemberFromDeclaringClasses(this MemberInfo source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source is PropertyInfo)
			{
				var reflectedType = source.ReflectedType;
				var memberType = source.GetPropertyOrFieldType();
				return reflectedType.GetPropertiesOfHierarchy().Cast<PropertyInfo>().Where(x => source.Name.Equals(x.Name) && memberType.Equals(x.PropertyType)).Cast<MemberInfo>();
			}

			if (source is FieldInfo)
			{
				return new[] { source.GetMemberFromDeclaringType() };
			}

			return Enumerable.Empty<MemberInfo>();
		}

		public static IEnumerable<MemberInfo> GetPropertyFromInterfaces(this MemberInfo source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			var propertyInfo = source as PropertyInfo;
			if (propertyInfo == null)
			{
				yield break;
			}
			if (source.ReflectedType.IsInterface)
			{
				yield break;
			}
			System.Type[] interfaces = source.ReflectedType.GetInterfaces();
			if (interfaces.Length == 0)
			{
				yield break;
			}
			MethodInfo propertyGetter = propertyInfo.GetGetMethod();
			foreach (System.Type @interface in interfaces)
			{
				InterfaceMapping memberMap = source.ReflectedType.GetInterfaceMap(@interface);
				PropertyInfo[] interfaceProperties = @interface.GetProperties();
				for (int i = 0; i < memberMap.TargetMethods.Length; i++)
				{
					if (memberMap.TargetMethods[i] == propertyGetter)
					{
						yield return interfaceProperties.Single(pi => pi.GetGetMethod() == memberMap.InterfaceMethods[i]);
					}
				}
			}
		}

		public static System.Type DetermineCollectionElementType(this System.Type genericCollection)
		{
			List<System.Type> interfaces = genericCollection.GetInterfaces().Where(t => t.IsGenericType).ToList();

			if (genericCollection.IsGenericType)
			{
				interfaces.Add(genericCollection);
			}

			System.Type enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerableInterface != null)
			{
				return enumerableInterface.GetGenericArguments()[0];
			}

			return null;
		}


		public static System.Type DetermineRequiredCollectionElementType(this MemberInfo collectionProperty)
		{
			System.Type propertyType = collectionProperty.GetPropertyOrFieldType();
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();

			if (collectionElementType == null)
			{
				var message = string.Format(
					"Unable to determine collection element type for the property/field '{0}' of {1}. The collection must be generic.",
					collectionProperty.Name,
					collectionProperty.DeclaringType != null ? collectionProperty.DeclaringType.FullName : "<global>");
				throw new MappingException(message);
			}

			return collectionElementType;
		}


		public static System.Type DetermineCollectionElementOrDictionaryValueType(this System.Type genericCollection)
		{
			if (genericCollection.IsGenericType)
			{
				List<System.Type> interfaces = genericCollection.GetInterfaces().Where(t => t.IsGenericType).ToList();
				if (genericCollection.IsInterface)
				{
					interfaces.Add(genericCollection);
				}
				System.Type enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
				if (enumerableInterface != null)
				{
					System.Type dictionaryInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IDictionary<,>));
					if (dictionaryInterface == null)
					{
						return enumerableInterface.GetGenericArguments()[0];
					}
					return dictionaryInterface.GetGenericArguments()[1];
				}
			}
			return null;
		}

		public static System.Type DetermineDictionaryKeyType(this System.Type genericDictionary)
		{
			if (genericDictionary.IsGenericType)
			{
				System.Type dictionaryInterface = GetDictionaryInterface(genericDictionary);
				if (dictionaryInterface != null)
				{
					return dictionaryInterface.GetGenericArguments()[0];
				}
			}
			return null;
		}

		private static System.Type GetDictionaryInterface(System.Type genericDictionary)
		{
			List<System.Type> interfaces = genericDictionary.GetInterfaces().Where(t => t.IsGenericType).ToList();
			if (genericDictionary.IsInterface)
			{
				interfaces.Add(genericDictionary);
			}
			return interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IDictionary<,>));
		}

		public static System.Type DetermineDictionaryValueType(this System.Type genericDictionary)
		{
			if (genericDictionary.IsGenericType)
			{
				System.Type dictionaryInterface = GetDictionaryInterface(genericDictionary);
				if (dictionaryInterface != null)
				{
					return dictionaryInterface.GetGenericArguments()[1];
				}
			}
			return null;
		}

		public static bool IsGenericCollection(this System.Type source)
		{
			return source.IsGenericType && typeof (IEnumerable).IsAssignableFrom(source);
		}

		public static MemberInfo GetFirstPropertyOfType(this System.Type propertyContainerType, System.Type propertyType)
		{
			return GetFirstPropertyOfType(propertyContainerType, propertyType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static MemberInfo GetFirstPropertyOfType(this System.Type propertyContainerType, System.Type propertyType, Func<PropertyInfo, bool> acceptPropertyClauses)
		{
			return GetFirstPropertyOfType(propertyContainerType, propertyType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, acceptPropertyClauses);
		}

		public static MemberInfo GetFirstPropertyOfType(this System.Type propertyContainerType, System.Type propertyType, BindingFlags bindingFlags)
		{
			return GetFirstPropertyOfType(propertyContainerType, propertyType, bindingFlags, x => true);
		}

		public static MemberInfo GetFirstPropertyOfType(this System.Type propertyContainerType, System.Type propertyType, BindingFlags bindingFlags, Func<PropertyInfo, bool> acceptPropertyClauses)
		{
			if (acceptPropertyClauses == null)
			{
				throw new ArgumentNullException("acceptPropertyClauses");
			}
			if (propertyContainerType == null || propertyType == null)
			{
				return null;
			}
			PropertyInfo[] propertyInfos = propertyContainerType.GetProperties(bindingFlags);
			return propertyInfos.FirstOrDefault(p => acceptPropertyClauses(p) && propertyType.Equals(p.PropertyType));
		}

		public static IEnumerable<MemberInfo> GetInterfaceProperties(this System.Type type)
		{
			if (!type.IsInterface)
			{
				yield break;
			}

			var analyzedInterface = new List<System.Type>();
			var interfacesQueue = new Queue<System.Type>();
			analyzedInterface.Add(type);
			interfacesQueue.Enqueue(type);
			while (interfacesQueue.Count > 0)
			{
				System.Type subType = interfacesQueue.Dequeue();
				foreach (System.Type subInterface in
					subType.GetInterfaces().Where(subInterface => !analyzedInterface.Contains(subInterface)))
				{
					analyzedInterface.Add(subInterface);
					interfacesQueue.Enqueue(subInterface);
				}

				foreach (PropertyInfo propertyInfo in subType.GetProperties())
				{
					yield return propertyInfo;
				}
			}
		}

		public static bool IsEnumOrNullableEnum(this System.Type type)
		{
			return type != null && type.UnwrapIfNullable().IsEnum;
		}

		public static bool IsFlagEnumOrNullableFlagEnum(this System.Type type)
		{
			if (type == null)
			{
				return false;
			}
			var typeofEnum = type.UnwrapIfNullable();
			return typeofEnum.IsEnum && typeofEnum.GetCustomAttributes(typeof (FlagsAttribute), false).Length > 0;
		}

		public static IEnumerable<System.Type> GetGenericInterfaceTypeDefinitions(this System.Type type)
		{
			if (type.IsGenericType && type.IsInterface)
			{
				yield return type.GetGenericTypeDefinition();
			}
			foreach (System.Type t in type.GetInterfaces().Where(t => t.IsGenericType))
			{
				yield return t.GetGenericTypeDefinition();
			}
		}

		public static System.Type GetFirstImplementorOf(this System.Type source, System.Type abstractType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (abstractType == null)
			{
				throw new ArgumentNullException("abstractType");
			}

			if (source.IsInterface)
			{
				return null;
			}

			if (source.Equals(abstractType))
			{
				return source;
			}

			return source.GetHierarchyFromBase().FirstOrDefault(t => !t.Equals(abstractType) && abstractType.IsAssignableFrom(t));
		}

		public static bool HasPublicPropertyOf(this System.Type source, System.Type typeOfProperty)
		{
			return GetFirstPropertyOfType(source, typeOfProperty, PropertiesOfClassHierarchy) != null;
		}

		public static bool HasPublicPropertyOf(this System.Type source, System.Type typeOfProperty, Func<PropertyInfo, bool> acceptPropertyClauses)
		{
			return GetFirstPropertyOfType(source, typeOfProperty, PropertiesOfClassHierarchy, acceptPropertyClauses) != null;
		}

		/// <summary>
		/// Given a property or a field try to get the member from a given possible inherited type.
		/// </summary>
		/// <param name="member">The member to find.</param>
		/// <param name="reflectedType">The type where find the member.</param>
		/// <returns>The member from the reflected-type or the original <paramref name="member"/> where the <paramref name="member"/> is not accessible from <paramref name="reflectedType"/>.</returns>
		public static MemberInfo GetMemberFromReflectedType(this MemberInfo member, System.Type reflectedType)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (reflectedType == null)
			{
				throw new ArgumentNullException("reflectedType");
			}
			var field = member as FieldInfo;
			if (field != null && field.IsPrivate)
			{
				return member;
			}
			var property = member as PropertyInfo;
			if (property != null)
			{
				var propertyGetter = property.GetGetMethod(true);
				if (propertyGetter.IsPrivate)
				{
					return member;
				}
				if (property.DeclaringType.IsInterface)
				{
					System.Type[] interfaces = reflectedType.GetInterfaces();
					var @interface = property.DeclaringType;
					if (!interfaces.Contains(@interface))
					{
						return member;
					}
					var reflectedCandidateProps = reflectedType.GetProperties(PropertiesOfClassHierarchy);
					InterfaceMapping memberMap = reflectedType.GetInterfaceMap(@interface);
					for (int i = 0; i < memberMap.TargetMethods.Length; i++)
					{
						if (memberMap.InterfaceMethods[i] == propertyGetter)
						{
							return reflectedCandidateProps.Single(pi => pi.GetGetMethod(true) == memberMap.TargetMethods[i]);
						}
					}
					return member;
				}
			}
			var reflectedTypeProperties = reflectedType.GetProperties(PropertiesOfClassHierarchy);
			var members = reflectedTypeProperties.Cast<MemberInfo>().Concat(reflectedType.GetFields(PropertiesOfClassHierarchy));
			var result = members.FirstOrDefault(m=> m.Name.Equals(member.Name) && m.GetPropertyOrFieldType().Equals(member.GetPropertyOrFieldType()));
			return result ?? member;
		}

		/// <summary>
		/// Try to find a property or field from a given type.
		/// </summary>
		/// <param name="source">The type</param>
		/// <param name="memberName">The property or field name.</param>
		/// <returns>
		/// A <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/> where the member is found; null otherwise.
		/// </returns>
		/// <remarks>
		/// Where found the member is returned always from the declaring type.
		/// </remarks>
		public static MemberInfo GetPropertyOrFieldMatchingName(this System.Type source, string memberName)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (memberName == null)
			{
				return null;
			}
			var nameToFind = memberName.Trim();
			var members = source.GetPropertiesOfHierarchy().Concat(source.GetFieldsOfHierarchy()).Concat(source.GetPropertiesOfInterfacesImplemented());
			return members.FirstOrDefault(x => x.Name == nameToFind);
		}

		private static IEnumerable<MemberInfo> GetPropertiesOfInterfacesImplemented(this System.Type source)
		{
			if (source.IsInterface)
			{
				foreach (var interfaceProperty in source.GetInterfaceProperties())
				{
					yield return interfaceProperty;
				}
				yield break;
			}
			foreach (var @interface in source.GetInterfaces())
			{
				foreach (var interfaceProperty in @interface.GetInterfaceProperties())
				{
					yield return interfaceProperty;
				}
			}
		}

		internal static IEnumerable<MemberInfo> GetPropertiesOfHierarchy(this System.Type type)
		{
			if(type.IsInterface)
			{
				yield break;
			}
			System.Type analizing = type;
			while (analizing != null && analizing != typeof(object))
			{
				foreach (PropertyInfo propertyInfo in analizing.GetProperties(PropertiesOrFieldOfClass))
				{
					yield return propertyInfo;
				}
				analizing = analizing.BaseType;
			}
		}

		private static IEnumerable<MemberInfo> GetFieldsOfHierarchy(this System.Type type)
		{
			if (type.IsInterface)
			{
				yield break;
			}
			System.Type analizing = type;
			while (analizing != null && analizing != typeof(object))
			{
				foreach (FieldInfo fieldInfo in GetUserDeclaredFields(analizing))
				{
					yield return fieldInfo;
				}
				analizing = analizing.BaseType;
			}
		}

		private static IEnumerable<FieldInfo> GetUserDeclaredFields(System.Type type)
		{
			// can't find another way to exclude fields generated by the compiler (for both auto-properties and anonymous-types)
			return type.GetFields(PropertiesOrFieldOfClass).Where(x => !x.Name.StartsWith("<"));
		}
	}
}