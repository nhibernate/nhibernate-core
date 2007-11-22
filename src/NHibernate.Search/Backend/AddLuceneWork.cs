using Lucene.Net.Documents;

namespace NHibernate.Search.Backend
{
    public class AddLuceneWork : LuceneWork
    {
        public AddLuceneWork(object id, string idInString, System.Type entityClass, Document document) : base(id, idInString, entityClass, document)
        {
        }
    }
}