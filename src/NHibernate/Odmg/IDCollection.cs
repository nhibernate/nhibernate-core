using System;
using System.Collections;

namespace NHibernate.Odmg
{
	/// <summary>
	/// A translation of ODMG DColletion
	/// </summary>
	public interface IDCollection : ICollection {

		object SelectElement(string predicate);
		IEnumerator Select(string predicate);
		IDCollection Query(string predicate);
		bool ExistsElement(string predicate);
	}
}
