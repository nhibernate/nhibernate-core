#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;

namespace NHibernate.Proxy.DynamicProxy
{
	public class ProxyCacheEntry : IEquatable<ProxyCacheEntry>
	{
		private readonly int _hashCode;

		public ProxyCacheEntry(System.Type baseType, System.Type[] interfaces)
		{
			if (baseType == null)
				throw new ArgumentNullException(nameof(baseType));
			BaseType = baseType;
			_uniqueInterfaces = new HashSet<System.Type>(interfaces ?? new System.Type[0]);

			if (_uniqueInterfaces.Count == 0)
			{
				_hashCode = baseType.GetHashCode();
				return;
			}
			
			var allTypes = new List<System.Type>(_uniqueInterfaces) { baseType };
			_hashCode = 59;
			foreach (System.Type type in allTypes)
			{
				// This simple implementation is nonsensitive to list order. If changing it for a sensitive one,
				// take care of ordering the list.
				_hashCode ^= type.GetHashCode();
			}
		}

		public System.Type BaseType { get; }
		public IReadOnlyCollection<System.Type> Interfaces { get { return _uniqueInterfaces; } }
		
		private HashSet<System.Type> _uniqueInterfaces;

		public override bool Equals(object obj)
		{
			var that = obj as ProxyCacheEntry;
			return Equals(that);
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