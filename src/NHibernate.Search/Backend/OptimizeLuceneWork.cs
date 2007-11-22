using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Search.Backend
{
    public class OptimizeLuceneWork : LuceneWork
    {
        public OptimizeLuceneWork(System.Type entity) : base(null, null, entity)
        {
        }
    }
}
