#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;

namespace NHibernate.Proxy.DynamicProxy
{
	[Serializable]
	public class ProxyCacheEntry
	{
		private readonly int hashCode;

		public ProxyCacheEntry(System.Type baseType, System.Type[] interfaces)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			BaseType = baseType;
			Interfaces = interfaces ?? new System.Type[0];

			if (Interfaces.Length == 0)
			{
				hashCode = baseType.GetHashCode();
				return;
			}

			// duplicated type exclusion
			var set = new HashSet<System.Type>(Interfaces) { baseType };
			hashCode = 59;
			foreach (System.Type type in set)
			{
				hashCode ^= type.GetHashCode();
			}
		}

		public System.Type BaseType { get; private set; }
		public System.Type[] Interfaces { get; private set; }

		public override bool Equals(object obj)
		{
			var that = obj as ProxyCacheEntry;
			if (ReferenceEquals(null, that))
			{
				return false;
			}

			return hashCode == that.GetHashCode();
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}