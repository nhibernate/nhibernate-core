using System.Collections.Generic;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// Support for parameterizable types. A UserType or CustomUserType may be
	/// made parameterizable by implementing this interface. Parameters for a
	/// type may be set by using a nested type element for the property element
	/// </summary>
	public interface IParameterizedType
	{
		/// <summary>
		/// Gets called by Hibernate to pass the configured type parameters to 
		/// the implementation.
		/// </summary>
		void SetParameterValues(IDictionary<string, string> parameters);
	}
}