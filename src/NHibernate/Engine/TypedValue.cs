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

		/// <summary>
		/// Constructor for typed value that may represent a simple value or a list value (for a parameter list).
		/// If knowing what is value, use <see cref="TypedValue(IType, object, bool)"/> instead.
		/// </summary>
		/// <param name="type">The type of the value (or of its elements if it is a list value)</param>
		/// <param name="value">The value.</param>
		/// <remarks>The logic for infering if the value should be considered as a list value is minimal and will not
		/// catch all cases, like hashset.</remarks>
		public TypedValue(IType type, object value) : this (type, value, !type.IsCollectionType && value is ICollection && !type.ReturnedClass.IsArray)
		{
		}

		/// <summary>
		/// Construct a typed value.
		/// </summary>
		/// <param name="type">The type of the value (or of its elements if it is a list value)</param>
		/// <param name="value">The value.</param>
		/// <param name="isList"><see langword="true" /> if the value is a list value (for a parameter list),
		/// <see langword="false" /> otherwise.</param>
		public TypedValue(IType type, object value, bool isList)
		{
			this.type = type;
			this.value = value;
			comparer = isList ? (IEqualityComparer<TypedValue>) new ParameterListComparer() : new DefaultComparer();
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
			public bool Equals(TypedValue x, TypedValue y)
			{
				if (y == null) return false;
				if (x.type.ReturnedClass != y.type.ReturnedClass)
					return false;
				return IsEquals(x.type, x.value as IEnumerable, y.value as IEnumerable);
			}

			public int GetHashCode(TypedValue obj)
			{
				return GetHashCode(obj.type, obj.value as IEnumerable);
			}

			private int GetHashCode(IType type, IEnumerable values)
			{
				if (values == null)
					return 0;

				unchecked
				{
					int result = 0;

					foreach (object obj in values)
						result += obj == null ? 0 : type.GetHashCode(obj);

					return result;
				}
			}

			private bool IsEquals(IType type, IEnumerable x, IEnumerable y)
			{
				if (x == y)
					return true;

				if (x == null || y == null)
					return false;

				if (x is ICollection xCol && y is ICollection yCol && xCol.Count != yCol.Count)
					return false;

				var ye = y.GetEnumerator();
				try
				{
					foreach (var xItem in x)
					{
						if (!ye.MoveNext())
						{
							// y has less elements than x
							return false;
						}
						if (!type.IsEqual(xItem, ye.Current))
							return false;
					}

					if (ye.MoveNext())
					{
						// y has more elements than x
						return false;
					}
				}
				finally
				{
					// The old non generic IEnumerator is not disposable, but in most cases the concrete enumerator
					// will be a generic one, disposable. https://stackoverflow.com/a/11179175/1178314
					(ye as IDisposable)?.Dispose();
				}

				return true;
			}
		}

		[Serializable]
		public class DefaultComparer : IEqualityComparer<TypedValue>
		{
			public bool Equals(TypedValue x, TypedValue y)
			{
				if (y == null) return false;
				if (x.type.ReturnedClass != y.type.ReturnedClass)
					return false;
				return x.type.IsEqual(y.value, x.value);
			}

			public int GetHashCode(TypedValue obj)
			{
				return obj.value == null ? 0 : obj.type.GetHashCode(obj.value);
			}
		}
	}
}
