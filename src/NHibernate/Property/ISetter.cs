using System.Reflection;

namespace NHibernate.Properties
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
		void Set(object target, object value);

		/// <summary>
		/// When implemented by a class, gets the name of the Property.
		/// </summary>
		/// <value>The name of the Property or <see langword="null" />.</value>
		/// <remarks>
		/// This is an optional operation - if it is not implemented then 
		/// <see langword="null" /> is an acceptable value to return.
		/// </remarks>
		string PropertyName { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="MethodInfo"/> for the <c>set</c>
		/// accessor of the property.
		/// </summary>
		/// <remarks>
		/// This is an optional operation - if the <see cref="ISetter"/> is not 
		/// for a property <c>set</c> then <see langword="null" /> is an acceptable value to return.
		/// It is used by the proxies to determine which setter to intercept for the
		/// identifier property.
		/// </remarks>
		MethodInfo Method { get; }
	}
}