using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Used to ensure a collection filtering a given IEnumerable by a certain type.
	/// </summary>
	/// <typeparam name="T">The type used like filter.</typeparam>
	// Since v5.3 
	[Obsolete("This class has no more usages and will be removed in a future version")]
	public class SafetyEnumerable<T> : IEnumerable<T>
	{
		/*
		 * This class was created to filter List<ISelectable> to an IEnumerable<Column>
		 */
		private readonly IEnumerable _collection;

		public SafetyEnumerable(IEnumerable collection)
		{
			_collection = collection;
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var element in _collection)
			{
				if (element == null)
					yield return default(T);
				else if (element is T elem)
					yield return elem;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
