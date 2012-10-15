using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> An ordered pair of a value and its Hibernate type. </summary>
	[Serializable]
	public sealed class TypedValue
	{
		// Because NH-875 we have a different implementation
		// The DefaultComparer is the comparer used in H3.2.5
		// The ParameterListComparer is the comparer introduced in NH to fix NH-845 

		private readonly IType type;
		private readonly object value;
		private readonly IEqualityComparer<TypedValue> comparer;

		public TypedValue(IType type, object value, EntityMode entityMode)
		{
			this.type = type;
			this.value = value;
			var values = value as ICollection;
			if (!type.IsCollectionType && values != null && !type.ReturnedClass.IsArray)
				comparer = new ParameterListComparer(entityMode);
			else
				comparer = new DefaultComparer(entityMode);
		}

		public object Value
		{
			get { return value; }
		}

		public IType Type
		{
			get { return type; }
		}

		public IEqualityComparer<TypedValue> Comparer
		{
			get { return comparer; }
		}

		public override int GetHashCode()
		{
			return comparer.GetHashCode(this);
		}

		public override bool Equals(object obj)
		{
			return comparer.Equals(this, obj as TypedValue);
		}

		public override string ToString()
		{
			return value == null ? "null" : value.ToString();
		}

		[Serializable]
		public class ParameterListComparer : IEqualityComparer<TypedValue>
		{
			private readonly EntityMode entityMode;

			public ParameterListComparer(EntityMode entityMode)
			{
				this.entityMode = entityMode;
			}

			public bool Equals(TypedValue x, TypedValue y)
			{
				if (y == null) return false;
				if (x.type.ReturnedClass != y.type.ReturnedClass)
					return false;
				return IsEquals(x.type, x.value as ICollection, y.value as ICollection);
			}

			public int GetHashCode(TypedValue obj)
			{
				return GetHashCode(obj.type, obj.value as ICollection);
			}

			private int GetHashCode(IType type, ICollection values)
			{
				if (values == null)
					return 0;

				unchecked
				{
					int result = 0;

					foreach (object obj in values)
						result += obj == null ? 0 : type.GetHashCode(obj, entityMode);

					return result;
				}
			}

			private bool IsEquals(IType type, ICollection x, ICollection y)
			{
				if (x == y)
					return true;

				if (x == null || y == null)
					return false;

				if (x.Count != y.Count)
					return false;

				IEnumerator xe = x.GetEnumerator();
				IEnumerator ye = y.GetEnumerator();

				while (xe.MoveNext())
				{
					ye.MoveNext();
					if (!type.IsEqual(xe.Current, ye.Current, entityMode))
						return false;
				}

				return true;
			}
		}

		[Serializable]
		public class DefaultComparer : IEqualityComparer<TypedValue>
		{
			private readonly EntityMode entityMode;

			public DefaultComparer(EntityMode entityMode)
			{
				this.entityMode = entityMode;
			}

			public bool Equals(TypedValue x, TypedValue y)
			{
				if (y == null) return false;
				if (x.type.ReturnedClass != y.type.ReturnedClass)
					return false;
				return x.type.IsEqual(y.value, x.value, entityMode);
			}

			public int GetHashCode(TypedValue obj)
			{
				return obj.value == null ? 0 : obj.type.GetHashCode(obj.value, entityMode);
			}
		}
	}
}
