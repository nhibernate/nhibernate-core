using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Implementation of FieldNamingStrategy for fields that are the 
	/// Camel Case version of the PropertyName
	/// </summary>
	public class CamelCaseStrategy : IFieldNamingStrategy
	{
		
		#region IFieldNamingStrategy Members

		public string GetFieldName(string propertyName)
		{
			return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
		}

		#endregion
	}
}
