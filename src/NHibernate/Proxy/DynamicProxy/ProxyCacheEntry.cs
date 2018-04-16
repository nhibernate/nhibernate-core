#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Proxy.DynamicProxy
{
	public class ProxyCacheEntry : IEquatable<ProxyCacheEntry>
	{
		private readonly int _hashCode;
		private readonly HashSet<System.Type> _uniqueInterfaces;

		public ProxyCacheEntry(System.Type baseType, System.Type[] interfaces)
		{
			BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));

			var uniqueInterfaces = new HashSet<System.Type>();
			if (interfaces != null && interfaces.Length > 0)
			{
				uniqueInterfaces.UnionWith(interfaces.Where(i => i != null));
			}
			_uniqueInterfaces = uniqueInterfaces;

			_hashCode = 59 ^ baseType.GetHashCode();

			if (_uniqueInterfaces.Count == 0)
				return;

			foreach (var type in _uniqueInterfaces)
			{
				// This simple implementation is nonsensitive to list order. If changing it for a sensitive one,
				// take care of ordering the list.
				_hashCode ^= type.GetHashCode();
			}
		}

		public System.Type BaseType { get; }

		public IReadOnlyCollection<System.Type> Interfaces => _uniqueInterfaces;

		public override bool Equals(object obj)
		{
			return Equals(obj as ProxyCacheEntry);
		}

		public bool Equals(ProxyCacheEntry other)
		{
			if (ReferenceEquals(null, other) ||
				// hashcode inequality allows an early exit, but their equality is not enough for guaranteeing instances equality.
				_hashCode != other._hashCode ||
				BaseType != other.BaseType)
			{
				return false;
			}

			return _uniqueInterfaces.SetEquals(other.Interfaces);
		}

		public override int GetHashCode() => _hashCode;
	}
}
