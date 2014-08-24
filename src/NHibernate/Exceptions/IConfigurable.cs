using System.Collections.Generic;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// The Configurable interface defines the contract for <see cref="ISQLExceptionConverter"/> impls that
	/// want to be configured prior to usage given the currently defined Hibernate properties. 
	/// </summary>
	public interface IConfigurable
	{
		/// <summary> Configure the component, using the given settings and properties. </summary>
		/// <param name="properties">All defined startup properties. </param>
		void Configure(IDictionary<string, string> properties);
	}
}