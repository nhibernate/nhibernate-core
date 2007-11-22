namespace NHibernate.Search.Backend
{
    public class DeleteLuceneWork : LuceneWork
    {
        public DeleteLuceneWork(object id, System.Type entityClass) : base(id, entityClass)
        {
        }
    }
}