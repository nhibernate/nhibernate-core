using System;
using System.Text;

namespace NHibernate.Search.Backend
{
    public class PurgeAllLuceneWork : LuceneWork
    {
        public PurgeAllLuceneWork(System.Type entity)
            : base(null, null, entity, null)
        {
        }
    }
}
