using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// Summary description for UUIDStringGenerator.
	/// </summary>
	public class UUIDStringGenerator : IIdentifierGenerator {
		public object Generate(ISessionImplementor cache, object ob) {
			return Guid.NewGuid().ToString();
		}
	}
}
