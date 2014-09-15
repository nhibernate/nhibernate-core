using System;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.Transform
{
	/// <summary>
	/// Access a private field found by case-insensitive matching
	/// </summary>
	[Serializable]
	public class CaseInsensitiveFieldAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new FieldAccessor.FieldGetter(GetField(theClass, propertyName), theClass, propertyName);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new FieldAccessor.FieldSetter(GetField(theClass, propertyName), theClass, propertyName);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion

		private static FieldInfo GetField(System.Type type, string fieldName)
		{
			return GetField(type, fieldName, type);
		}

		private static FieldInfo GetField(System.Type type, string fieldName, System.Type originalType)
		{
			if (type == null || type == typeof(object))
			{
				throw new PropertyNotFoundException(originalType, fieldName);
			}

			var bindingFlags = BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly | 
				BindingFlags.IgnoreCase;

			var result = type.GetField(fieldName, bindingFlags);
			if (result == null)
			{
				result = GetField(type.BaseType, fieldName, originalType);
			}

			return result;
		}
	}
}
