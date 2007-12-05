using Lucene.Net.Documents;

namespace NHibernate.Search.Bridge
{
    /// <summary>
    /// Put an object inside the document.
    /// </summary>
    public interface IFieldBridge
    {
        void Set(string idKeywordName, object id, Document doc, Field.Store store, Field.Index index, float? boost);
    }
}