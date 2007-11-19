using System;
using Lucene.Net.Documents;
using NHibernate.Util;

namespace NHibernate.Search
{
	/// <summary>
	/// Put an object inside the document.
	/// </summary>
	public interface IFieldBridge
	{
		void Set(string idKeywordName, object id, Document doc, Field.Store store, Field.Index index, float? boost);
	}
}