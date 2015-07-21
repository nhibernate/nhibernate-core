using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class AccessorPropertyMapper : IAccessorPropertyMapper
	{
		private const BindingFlags FieldBindingFlag =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private readonly bool canChangeAccessor = true;

		private readonly System.Type declaringType;

		private readonly IDictionary<string, IFieldNamingStrategy> fieldNamningStrategies = PropertyToField.DefaultStrategies;

		private readonly string propertyName;
		private readonly Action<string> setAccessor;

		public AccessorPropertyMapper(System.Type declaringType, string propertyName, Action<string> accesorValueSetter)
		{
			PropertyName = propertyName;
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			MemberInfo member = null;
			if (propertyName != null)
			{
				member = declaringType.GetMember(propertyName, FieldBindingFlag).FirstOrDefault();
			}
			if (member == null)
			{
				accesorValueSetter("none");
				canChangeAccessor = false;
			}
			else if ((member as FieldInfo) != null)
			{
				accesorValueSetter("field");
				canChangeAccessor = false;
			}
			this.declaringType = declaringType;
			this.propertyName = propertyName;
			setAccessor = accesorValueSetter;
		}

		public string PropertyName { get; set; }

		#region IAccessorPropertyMapper Members

		public void Access(Accessor accessor)
		{
			if (!canChangeAccessor)
			{
				return;
			}
			switch (accessor)
			{
				case Accessor.Property:
					setAccessor("property");
					break;
				case Accessor.Field:
					string partialFieldNamingStrategyName = GetNamingFieldStrategy();
					if (partialFieldNamingStrategyName != null)
					{
						setAccessor("field." + partialFieldNamingStrategyName);
					}
					else
					{
						setAccessor("field");
					}
					break;
				case Accessor.NoSetter:
					string partialNoSetterNamingStrategyName = GetNamingFieldStrategy();
					if (partialNoSetterNamingStrategyName != null)
					{
						setAccessor("nosetter." + partialNoSetterNamingStrategyName);
					}
					else
					{
						throw new ArgumentOutOfRangeException("accessor",
						                                      "The property name " + propertyName
						                                      + " does not match with any supported field naming strategies.");
					}
					break;
				case Accessor.ReadOnly:
					setAccessor("readonly");
					break;
				case Accessor.None:
					setAccessor("none");
					break;
				default:
					throw new ArgumentOutOfRangeException("accessor");
			}
		}

		public void Access(System.Type accessorType)
		{
			if (!canChangeAccessor)
			{
				return;
			}
			if (accessorType == null)
			{
				throw new ArgumentNullException("accessorType");
			}
			if (typeof (IPropertyAccessor).IsAssignableFrom(accessorType))
			{
				setAccessor(accessorType.AssemblyQualifiedName);
			}
			else
			{
				throw new ArgumentOutOfRangeException("accessorType", "The accessor should implements IPropertyAccessor.");
			}
		}

		#endregion

		private string GetNamingFieldStrategy()
		{
			KeyValuePair<string, IFieldNamingStrategy> pair =
				fieldNamningStrategies.FirstOrDefault(p => GetField(declaringType, p.Value.GetFieldName(propertyName)) != null);
			return pair.Key;
		}

		private static MemberInfo GetField(System.Type type, string fieldName)
		{
			if (type == typeof (object) || type == null)
			{
				return null;
			}
			MemberInfo member = type.GetField(fieldName, FieldBindingFlag) ?? GetField(type.BaseType, fieldName);
			return member;
		}
	}
}