using System;
using Lucene.Net.Documents;

namespace NHibernate.Search.Impl
{
	public class LuceneWork
	{
		private System.Type entityClass;
		private object id;


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