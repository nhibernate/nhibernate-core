using System.Collections;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Properties
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
		object Get(object target);

		/// <summary>
		/// When implemented by a class, gets the <see cref="System.Type"/> that the Property/Field returns.
		/// </summary>
		/// <value>The <see cref="System.Type"/> that the Property returns.</value>
		System.Type ReturnType { get; }

		/// <summary>
		/// When implemented by a class, gets the name of the Property.
		/// </summary>
		/// <value>The name of the Property or <see langword="null" />.</value>
		/// <remarks>
		/// This is an optional operation - if the <see cref="IGetter"/> is not 
		/// for a Property <c>get</c> then <see langword="null" /> is an acceptable value to return.
		/// </remarks>
		string PropertyName { get; }

		/// <summary>
		/// When implemented by a class, gets the <see cref="MethodInfo"/> for the <c>get</c>
		/// accessor of the property.
		/// </summary>
		/// <remarks>
		/// This is an optional operation - if the <see cref="IGetter"/> is not 
		/// for a property <c>get</c> then <see langword="null" /> is an acceptable value to return.
		/// It is used by the proxies to determine which getter to intercept for the
		/// identifier property.
		/// </remarks>
		MethodInfo Method { get; }

		/// <summary> Get the property value from the given owner instance. </summary>
		/// <param name="owner">The instance containing the value to be retrieved. </param>
		/// <param name="mergeMap">a map of merged persistent instances to detached instances </param>
		/// <param name="session">The session from which this request originated. </param>
		/// <returns> The extracted value. </returns>
		object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session);
	}
}
