using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Sets values of a particular mapped property.
	/// </summary>
	public interface ISetter
	{
		/// <summary>
		/// When implemented by a class, sets the value of the Property/Field on the object.
		/// </summary>
		/// <param name="target">The object to set the Property value in.</param>
		/// <param name="value">The value to set the Property to.</param>
		/// <exception cref="PropertyAccessException">
		/// Thrown when there is a problem setting the value in the target.
		/// </exception>
		void Set( object target, object value );

		/// <summary>
		/// When implemented by a class, gets the name of the Property.
		/// </summary>
		/// <value>The name of the Property or <c>null</c>.</value>
		/// <remarks>
		/// This is an optional operation - if it is not implemented then 
		/// <c>null</c> is an acceptable value to return.
		/// </remarks>
		string PropertyName { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="PropertyInfo"/> for the Property.
		/// </summary>
		/// <value>
		/// The <see cref="PropertyInfo"/> for the Property.
		/// </value>
		/// <remarks>
		/// This is an optional operation - if it is not implemented then 
		/// <c>null</c> is an acceptable value to return.  It is used by
		/// the Proxies to determine which setter to intercept as the
		/// &lt;id&gt; property.
		/// </remarks>
		PropertyInfo Property { get; }
	}
}