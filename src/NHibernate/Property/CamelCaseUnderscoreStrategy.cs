using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Implementation of FieldNamingStrategy for fields that are prefixed with
	/// an underscore and the PropertyName is changed to camelCase.
	/// </summary>
	public class CamelCaseUnderscoreStrategy : IFieldNamingStrategy	
	{
		#region IFieldNamingStrategy Members

		public string GetFieldName(string propertyName)
		{
			return "_" + propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
		}

		#endregion
	}
}
