using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Compares objects by reference equality
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	class ReferenceComparer<T> : IEqualityComparer, IEqualityComparer<T> where T : class
	{
		private ReferenceComparer()
		{
		}

		public bool Equals(T x, T y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			return ReferenceEquals(x, y);
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
		}

		public static readonly ReferenceComparer<T> Instance = new ReferenceComparer<T>();
	}
}
