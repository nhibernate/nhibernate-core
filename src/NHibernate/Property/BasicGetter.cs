using System;
using System.Reflection;

namespace NHibernate.Property
{
	
	/// <summary>
	/// 
	/// </summary>
	public sealed class BasicGetter : IGetter
	{
		private System.Type clazz;
		private PropertyInfo property;
		private string propertyName;

		public BasicGetter(System.Type clazz, PropertyInfo property, string propertyName) 
		{
			this.clazz = clazz;
			this.property = property;
			this.propertyName = propertyName;
		}

		#region IGetter Members

		public object Get(object target)
		{
			try 
			{
				return property.GetValue(target, new object[0]);
			} 
			catch (Exception e) 
			{
				throw new PropertyAccessException(e, "Exception occurred", false, clazz, propertyName);
			}
		}

		public System.Type ReturnType
		{
			get { return property.PropertyType; }
		}

		public string PropertyName
		{
			get { return property.Name; }
		}

		public PropertyInfo Property
		{
			get{ return property; }
		}

		#endregion
	}
}
