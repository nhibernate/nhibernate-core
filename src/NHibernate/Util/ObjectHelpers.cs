
namespace NHibernate.Util
{
	/// <summary>
	/// Various small helper methods.
	/// </summary>
	public static class ObjectHelpers
	{
		/// <summary>
		/// Return an identifying string representation for the object, taking
		/// NHibernate proxies into account. The returned string will be "null",
		/// "classname@hashcode(hash)", or "entityname#identifier". If the object
		/// is an uninitialized NHibernate proxy, take care not to initialize it.
		/// </summary>
		public static string IdentityToString(object obj)
		{
			if (obj == null)
			{
				return "null";
			}

			var proxy = obj as Proxy.INHibernateProxy;

			if (null != proxy)
			{
				var init = proxy.HibernateLazyInitializer;
				return StringHelper.Unqualify(init.EntityName) + "#" + init.Identifier;
			}

			return StringHelper.Unqualify(obj.GetType().FullName) + "@" + obj.GetHashCode() + "(hash)";
		}
	}
}
