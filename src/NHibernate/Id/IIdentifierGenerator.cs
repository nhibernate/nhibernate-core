using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// Generate a new identifier
	/// </summary>
	public interface IIdentifierGenerator {
		
		/// <summary>
		/// Generate a new identifier
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj">The entity for which the id is being generate</param>
		/// <returns>The new identifier</returns>
		object Generate(ISessionImplementor session, object obj);
	}
}
