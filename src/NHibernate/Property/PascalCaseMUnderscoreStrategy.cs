using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Implementation of FieldNamingStrategy for fields that are prefixed with
	/// an "m_" and the PropertyName.
	/// </summary>
	public class PascalCaseMUnderscoreStrategy : IFieldNamingStrategy
	{
		
		#region IFieldNamingStrategy Members

		public string GetFieldName(string propertyName)
		{
			return "m_" + propertyName;
		}

		#endregion
	}
}
