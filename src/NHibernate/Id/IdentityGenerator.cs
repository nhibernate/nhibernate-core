using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// Indicates to the <c>Session</c> that identity (ie identity/autoincrement column) key generation
	/// should be used
	/// </summary>
	public class IdentityGenerator : IIdentifierGenerator {
		
		public object Generate(ISessionImplementor s, object obj) {
			return null;
		}
	}
}
