using System;
using System.Collections.Generic;

namespace NHibernate.Search.Impl
{
	public class DeleteLuceneWork : LuceneWork
	{
		public DeleteLuceneWork(object id, System.Type entityClass) : base(id, entityClass)
		{
		}
	}
}