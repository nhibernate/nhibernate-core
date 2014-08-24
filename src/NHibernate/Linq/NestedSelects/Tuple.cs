using System;
using System.Linq;
using System.Reflection;

namespace NHibernate.Linq.NestedSelects
{
	internal class Tuple : IEquatable<Tuple>
	{
		public static readonly ConstructorInfo Constructor = typeof (Tuple).GetConstructor(new[] { typeof (object[]) });
		public static readonly PropertyInfo ItemsProperty = typeof (Tuple).GetProperty("Items");
		private readonly object[] _items;

		public Tuple(object[] items)
		{
			if (items == null) throw new ArgumentNullException("items");
			_items = items;
		}

		public object[] Items
		{
			get { return _items; }
		}

		public bool Equals(Tuple other)
		{
			if (other == null) return false;
			if (ReferenceEquals(this, other)) return true;

			if (other._items.Length != _items.Length)
				return false;

			return _items.SequenceEqual(other._items);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Tuple);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var length = _items.Length;
				if (length == 0)
					return 0;

				var lengthCode = length;
				var firstElement = _items[0];
				if (ReferenceEquals(firstElement, null))
					return lengthCode;

				return firstElement.GetHashCode() * 397 ^ lengthCode;
			}
		}
	}
}
