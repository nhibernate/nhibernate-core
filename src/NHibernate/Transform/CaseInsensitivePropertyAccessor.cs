using System;
using System.Linq;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.Transform
{
	/// <summary>
	/// Access a Property found by case-insensitive matching
	/// </summary>
	[Serializable]
	public class CaseInsensitivePropertyAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			var property = GetProperty(theClass, propertyName);
			if (property == null)
				throw new PropertyNotFoundException(theClass, propertyName);
			return new BasicPropertyAccessor.BasicGetter(theClass, property, propertyName);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			var property = GetProperty(theClass, propertyName);
			if (property == null)
				throw new PropertyNotFoundException(theClass, propertyName);
			return new BasicPropertyAccessor.BasicSetter(theClass, property, propertyName);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion

		private static PropertyInfo GetProperty(System.Type type, string propertyName)
		{
			PropertyInfo result = null;
			if (type == typeof(object) || type == null)
			{
				return result;
			}

			var bindingFlags = BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly |
				BindingFlags.IgnoreCase;

			result = type.GetProperties(bindingFlags)
				.FirstOrDefault(p => string.Compare(p.Name, propertyName, true) == 0);

			if (result == null)
				result = GetProperty(type.BaseType, propertyName);
			else
				if (!result.CanWrite)
					result = null;

			return result;
		}
	}
}
