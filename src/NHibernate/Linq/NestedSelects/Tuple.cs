using System;
using System.Linq;
using System.Reflection;

namespace NHibernate.Linq.NestedSelects
{
	internal class Tuple : IEquatable<Tuple>
	{
		public static readonly FieldInfo ItemsField = typeof (Tuple).GetField("Items");

		public object[] Items;

		public bool Equals(Tuple other)
		{
			if (other == null) return false;

			if (other.Items.Length != Items.Length)
				return false;

			return !other.Items.Where((t, index) => !Equals(t, Items[index])).Any();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Tuple);
		}

		public override int GetHashCode()
		{
			return (Items != null ? Items.Length.GetHashCode() : 0);
		}
	}
}
