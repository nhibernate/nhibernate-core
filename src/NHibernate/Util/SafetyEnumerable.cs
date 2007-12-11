using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Used to ensecure a collection filtering a given IEnumerable by a certain type.
	/// </summary>
	/// <typeparam name="T">The type used like filter.</typeparam>
	public class SafetyEnumerable<T> : IEnumerable<T>
	{
		/*
		 * This class was created to filter List<ISelectable> to an IEnumerable<Column>
		 */
		private readonly IEnumerable collection;

		public SafetyEnumerable(IEnumerable collection)
		{
			this.collection = collection;
		}

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			IEnumerator enumerator = collection.GetEnumerator();
			while(enumerator.MoveNext())
			{
				object element = enumerator.Current;
				if(typeof(T).IsAssignableFrom(element.GetType()))
					yield return (T)element;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}
}
