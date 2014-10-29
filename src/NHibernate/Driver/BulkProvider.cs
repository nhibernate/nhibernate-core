using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Persister.Entity;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Driver
{
	public abstract class BulkProvider : IDisposable
	{
		protected BulkProvider()
		{
		}

		~BulkProvider()
		{
			this.Dispose(false);
		}

		public Int32 BatchSize { get; set; }

		public Int32 Timeout { get; set; }

		public abstract void Insert<T>(ISessionImplementor session, IEnumerable<T> entities) where T : class;

		public virtual void Initialize(IDictionary<String, String> properties)
		{
			var timeout = string.Empty;
			var batchSize = string.Empty;

			if (properties.TryGetValue(Environment.BulkProviderTimeout, out timeout))
			{
				this.Timeout = Convert.ToInt32(timeout);
			}

			if (properties.TryGetValue(Environment.BulkProviderBatchSize, out batchSize))
			{
				this.BatchSize = Convert.ToInt32(batchSize);
			}
		}

		protected virtual void FillIdentifier(ISessionImplementor session, IEntityPersister persister, Object entity)
		{
			if (!(persister.IdentifierGenerator is Assigned) && !(persister.IdentifierGenerator is ForeignGenerator))
			{
				var id = persister.IdentifierGenerator.Generate(session, entity);

				persister.SetIdentifier(entity, id, session.EntityMode);
			}
		}

		protected virtual void Dispose(Boolean disposing)
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
