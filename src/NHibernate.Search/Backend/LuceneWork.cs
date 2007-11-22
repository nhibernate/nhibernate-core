namespace NHibernate.Search.Backend
{
    public class LuceneWork
    {
        private readonly System.Type entityClass;
        private readonly object id;

        protected LuceneWork(object id, System.Type entityClass)
        {
            this.entityClass = entityClass;
            this.id = id;
        }

        public System.Type EntityClass
        {
            get { return entityClass; }
        }

        public object Id
        {
            get { return id; }
        }
    }
}