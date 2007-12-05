using System;
using System.Reflection;
using NHibernate.Search.Attributes;

namespace NHibernate.Search
{
	public class AttributeUtil
	{
		public static bool IsIndexed(System.Type type)
		{
			return type.GetCustomAttributes(typeof (IndexedAttribute), true).Length != 0;
		}

		public static IndexedAttribute GetIndexed(System.Type type)
		{
			object[] objects = type.GetCustomAttributes(typeof (IndexedAttribute), true);
			if (objects.Length == 0)
				return null;
			return (IndexedAttribute) objects[0];
		}

		public static BoostAttribute GetBoost(MemberInfo element)
		{
			object[] objects = element.GetCustomAttributes(typeof (BoostAttribute), true);
			if (objects.Length == 0)
				return null;
			return (BoostAttribute) objects[0];
		}

		public static DocumentIdAttribute GetDocumentId(MemberInfo member)
		{
			object[] objects = member.GetCustomAttributes(typeof (DocumentIdAttribute), true);
			if (objects.Length == 0)
				return null;
			DocumentIdAttribute documentIdAttribute = (DocumentIdAttribute) objects[0];
			documentIdAttribute.Name = documentIdAttribute.Name ?? member.Name;
			return documentIdAttribute;
		}

		public static FieldAttribute GetField(MemberInfo member)
		{
			object[] objects = member.GetCustomAttributes(typeof (FieldAttribute), true);
			if (objects.Length == 0)
				return null;
			FieldAttribute fieldAttribute = (FieldAttribute) objects[0];
			fieldAttribute.Name = fieldAttribute.Name ?? member.Name;
			return fieldAttribute;
		}

		public static FieldBridgeAttribute GetFieldBridge(MemberInfo member)
		{
			object[] objects = member.GetCustomAttributes(typeof (FieldBridgeAttribute), true);
			if (objects.Length == 0)
				return null;
			FieldBridgeAttribute fieldBridgeAttribute = (FieldBridgeAttribute) objects[0];
			return fieldBridgeAttribute;
		}

		public static bool IsDateBridge(MemberInfo member)
		{
			return GetDateBridge(member) != null;
		}

		public static DateBridgeAttribute GetDateBridge(MemberInfo member)
		{
			object[] objects = member.GetCustomAttributes(typeof (DateBridgeAttribute), true);
			if (objects.Length == 0)
				return null;
			DateBridgeAttribute fieldBridgeAttribute = (DateBridgeAttribute) objects[0];
			return fieldBridgeAttribute;
		}
	}
}