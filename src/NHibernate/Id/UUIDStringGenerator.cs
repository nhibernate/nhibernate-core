using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// An <c>IIdentifierGenerator</c> for returning UUIDs
	/// </summary>
	public class UUIDStringGenerator : IIdentifierGenerator {
		public object Generate(ISessionImplementor cache, object ob) {
			return Guid.NewGuid().ToString();
		}
	}
}
