using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Implementation of FieldNamingStrategy for fields that are prefixed with
	/// an underscore and the PropertyName is changed to lower case.
	/// </summary>
	public class LowerCaseUnderscoreStrategy : IFieldNamingStrategy 
	{
		#region IFieldNamingStrategy Members

		public string GetFieldName(string propertyName) 
		{
			return "_" + propertyName.ToLower();
		}

		#endregion
	}
}


