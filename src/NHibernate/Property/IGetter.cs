using System.Reflection;

namespace NHibernate.Property
{
	/// <summary>
	/// Gets values of a particular property
	/// </summary>
	public interface IGetter
	{
		/// <summary>
		/// Get the property value from the given instance
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		object Get( object target );

		/// <summary>
		/// Get the declared Type.
		/// </summary>
		/// <returns></returns>
		System.Type ReturnType { get; }

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
		/// This is used to tell the Proxy which getter to intercept as the &lt;id&gt;
		/// property. 
		/// </remarks>
		PropertyInfo Property { get; }
	}
}