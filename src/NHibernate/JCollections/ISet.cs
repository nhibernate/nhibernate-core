using System;

using System.Collections;

namespace NHibernate.JCollections
{
	/// <summary>
	/// A .NET porting of the Java Set interface
	/// </summary>
	public interface ISet : ICollection
	{
		bool Add(object o);
		bool AddAll(ICollection c);
		void Clear();
		bool Contains(object o);
		bool ContainsAll(ICollection c);
		bool Equals(object o);
		int GetHashCode();
		bool IsEmpty();
		// IEnumerator GetEnumerator(); // this method is defined at IEnumerable (ancestor of ICollection)
		bool Remove(object o);
		bool RemoveAll(ICollection c);
		bool RetainAll(ICollection c);
		// int Count { get; } // this property is defined at ICollection
		object[] ToArray();
		Array ToArray(System.Type type);
	}
}
