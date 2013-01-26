using System;
using System.Linq;
using System.Reflection;

namespace NHibernate.Linq.NestedSelects
{
	internal class Tuple : IEquatable<Tuple>
	{
        public static readonly ConstructorInfo Constructor = typeof (Tuple).GetConstructor(new [] { typeof(object[]) });
		public static readonly PropertyInfo ItemsProperty = typeof (Tuple).GetProperty("Items");

        public object[] Items { get; private set; }

        public Tuple(object[] items)
        {
            Items = items;
        }

		public bool Equals(Tuple other)
		{
			if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            // SequenceEqual checks this as well
			if (other.Items.Length != Items.Length)
				return false;

			return Enumerable.SequenceEqual<object>(Items, other.Items);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Tuple);
		}

		public override int GetHashCode()
		{
            // TODO: it looks like in the scenarios this class is used, tuples will be of the same length,
            // so may be it would be better to use another hash code, for example Items[0].GetHashCode()
			return Items.Length.GetHashCode();
		}
	}
}
