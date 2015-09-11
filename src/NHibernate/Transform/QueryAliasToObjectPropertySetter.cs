using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Transform
{
	[Serializable]
	public class QueryAliasToObjectPropertySetter
	{
		private readonly IEnumerable<FieldInfo> _fields;
		private readonly IEnumerable<PropertyInfo> _properties;

		private QueryAliasToObjectPropertySetter(FieldInfo[] fields, PropertyInfo[] properties)
		{
			_fields = fields;
			_properties = properties;
		}

		public static QueryAliasToObjectPropertySetter MakeFor(System.Type objType)
		{
			var bindingFlags = BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.IgnoreCase;
			var fields = objType.GetFields(bindingFlags);
			var properties = objType.GetProperties(bindingFlags);

			return new QueryAliasToObjectPropertySetter(fields, properties);
		}

		public void SetProperty(string alias, object value, object resultObj)
		{
			var property = _properties.SingleOrDefault(prop => string.Equals(prop.Name, alias, StringComparison.OrdinalIgnoreCase));
			var field = _fields.SingleOrDefault(prop => string.Equals(prop.Name, alias, StringComparison.OrdinalIgnoreCase));
			if (field == null && property == null)
				throw new PropertyNotFoundException(resultObj.GetType(), alias, "setter");

			if (field != null)
			{
				field.SetValue(resultObj, value);
				return;
			}
			if (property != null && property.CanWrite)
				property.SetValue(resultObj, value, new object[0]);
		}
	}
}
