using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Proxeability validator.
	/// </summary>
	public interface IProxyValidator
	{
		/// <summary>
		/// Validates whether <paramref name="type"/> can be specified as the base class
		/// (or an interface) for a dynamically-generated proxy.
		/// </summary>
		/// <param name="type">The type to validate.</param>
		/// <returns>
		/// A collection of errors messages, if any, or <see langword="null" /> if none were found.
		/// </returns>
		/// <remarks>
		/// When the configuration property "use_proxy_validator" is set to true(default), the result of this method
		/// is used to throw a detailed exception about the proxeability of the given <paramref name="type"/>.
		/// </remarks>
		ICollection<string> ValidateType(System.Type type);

		/// <summary>
		/// Validate if a single method can be intercepted by proxy.
		/// </summary>
		/// <param name="method">The given method to check.</param>
		/// <returns><see langword="true"/> if the method can be intercepted by proxy.
		/// <see langword="false"/> otherwise.
		/// </returns>
		/// <remarks>
		/// This method can be used internally by the <see cref="ValidateType"/> and is used
		/// by <see cref="NHibernate.Tuple.Entity.PocoEntityTuplizer"/> to log errors when
		/// a property accessor can't be intercepted by proxy.
		/// The validation of property accessors is fairly enough if you ecampsulate each property.
		/// </remarks>
		bool IsProxeable(MethodInfo method);
	}
}