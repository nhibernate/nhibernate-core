using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Summary description for ISetter.
	/// </summary>
	public interface ISetter
	{
		/// <summary>
		/// Set the property value from the given instance
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		/// <exception cref="HibernateException">
		/// </exception>
		void Set(object target, object value);

		/// <summary>
		/// Optional operation (return null)
		/// </summary>
		/// <returns></returns>
		string PropertyName { get; }

		/// <summary>
		/// Optional operation (return null)
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This is used to tell the Proxy which setter to intercept as the &lt;id&gt;
		/// property. 
		/// </remarks>
		PropertyInfo Property { get; }
	}
}
