using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// The general contract between a class that generates unique
	/// identifiers and the <c>Session</c>. It is not intended that
	/// this interface ever be exposed to the application. It <b>is</b>
	/// intended that users implement this interface to provide
	/// custom identifier generation strategies.
	/// 
	/// Implementors should provide a public default constructor.
	/// 
	/// Implementations that accept configuration parameters should
	/// also implement <c>Configurable</c>.
	/// 
	/// Implementors MUST be threadsafe.
	/// </summary>
	public class IdentityGenerator : IIdentifierGenerator {
		
		public object Generate(ISessionImplementor s, object obj) {
			return null;
		}
	}
}
