using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Abstracts the notion of a "property". Defines a strategy for accessing the
	/// value of an attribute.
	/// </summary>
	public interface IPropertyAccessor
	{
		/// <summary>
		/// Create a "getter" for the named attribute
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		/// <exception cref="PropertyNotFoundException" >
		/// </exception>
		IGetter GetGetter(System.Type theClass, string propertyName); 
		
		/// <summary>
		/// Create a "setter" for the named attribute
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		/// <exception cref="PropertyNotFoundException" >
		/// </exception>
		ISetter GetSetter(System.Type theClass, string propertyName); 
	}
}
