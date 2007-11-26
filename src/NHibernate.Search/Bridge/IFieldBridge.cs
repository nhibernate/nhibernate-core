using System;
using Lucene.Net.Documents;
using NHibernate.Util;

namespace NHibernate.Search.Bridge
{
    /// <summary>
    /// Put an object inside the document.
    /// </summary>
    public interface IFieldBridge
    {
#if NET_2_0
		void Set(string idKeywordName, object id, Document doc, Field.Store store, Field.Index index, float? boost);
#else
		void Set(string idKeywordName, object id, Document doc, Field.Store store, Field.Index index, float boost);
#endif
    }
}