using System;
using System.Collections;
using NHibernate.Type;

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
		/// <param name="type"></param>
		/// <param name="parms">Param values that are keyed by parameter name.</param>
		/// <param name="d"></param>
		void Configure(IType type, IDictionary parms, Dialect.Dialect d);
	}
}
