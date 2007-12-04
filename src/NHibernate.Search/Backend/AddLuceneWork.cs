using Lucene.Net.Documents;

namespace NHibernate.Search.Impl
{
	public class AddLuceneWork : LuceneWork
	{
		private readonly Document document;

		public AddLuceneWork(object id, System.Type entityClass, Document document) : base(id, entityClass)
		{
			this.document = document;
		}


		public Document Document
		{
			get { return document; }
		}
	}
}