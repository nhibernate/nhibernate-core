namespace NHibernate.Search.Backend
{
    public class DeleteLuceneWork : LuceneWork
    {
        public DeleteLuceneWork(object id, string idInString, System.Type entityClass) : base(id, idInString, entityClass)
        {
        }
    }
}