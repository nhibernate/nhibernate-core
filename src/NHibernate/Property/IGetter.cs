using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Gets values of a particular mapped property.
	/// </summary>
	public interface IGetter
	{
		/// <summary>
		/// When implemented by a class, gets the value of the Property/Field from the object.
		/// </summary>
		/// <param name="target">The object to get the Property/Field value from.</param>
		/// <returns>
		/// The value of the Property for the target.
		/// </returns>
		/// <exception cref="PropertyAccessException">
		/// Thrown when there is a problem getting the value from the target.
		/// </exception>
		object Get( object target );

		/// <summary>
		/// When implemented by a class, gets the <see cref="System.Type"/> that the Property/Field returns.
		/// </summary>
		/// <value>The <see cref="System.Type"/> that the Property returns.</value>
		System.Type ReturnType { get; }

		/// <summary>
		/// When implemented by a class, gets the name of the Property.
		/// </summary>
		/// <value>The name of the Property or <c>null</c>.</value>
		/// <remarks>
		/// This is an optional operation - if the <see cref="IGetter"/> is not 
		/// for a Property <c>get</c> then <c>null</c> is an acceptable value to return.
		/// </remarks>
		string PropertyName { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="PropertyInfo"/> for the Property.
		/// </summary>
		/// <value>
		/// The <see cref="PropertyInfo"/> for the Property.
		/// </value>
		/// <remarks>
		/// This is an optional operation - if the <see cref="IGetter"/> is not 
		/// for a Property <c>get</c> then <c>null</c> is an acceptable value to return.
		/// It is used by the Proxies to determine which getter to intercept as the
		/// &lt;id&gt; property.
		/// </remarks>
		PropertyInfo Property { get; }
	}
}