using Lucene.Net.Documents;

namespace NHibernate.Search.Backend
{
    public abstract class LuceneWork
    {
        private readonly object id;
        private readonly string idInString;
        private readonly System.Type entityClass;
        private readonly Document document;

        // Flag indicating that the lucene work has to be indexed in batch mode
        private bool isBatch;

        #region Constructors

        protected LuceneWork(object id, string idInString, System.Type entityClass) : this(id, idInString, entityClass, null)
        {
        }

        protected LuceneWork(object id, string idInString, System.Type entityClass, Document document)
        {
            this.id = id;
            this.idInString = idInString;
            this.entityClass = entityClass;
            this.document = document;
        }

        #endregion

        #region Property methods

        public System.Type EntityClass
        {
            get { return entityClass; }
        }

        public object Id
        {
            get { return id; }
        }

        public string IdInString
        {
            get { return idInString; }
        }

        public Document Document
        {
            get { return document; }
        }

        public bool IsBatch
        {
            get { return isBatch; }
            set { isBatch = value; }
        }

        #endregion
    }
}