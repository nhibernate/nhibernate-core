using System.Collections;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IdentiferGenerator</c> that supports "configuration".
	/// </summary>
	public interface IConfigurable
	{
		/// <summary>
		/// Configure this instance, given the values of parameters
		/// specified by the user as <c>&lt;param&gt;</c> elements.
		/// This method is called just once, followed by instantiation.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect);
	}
}