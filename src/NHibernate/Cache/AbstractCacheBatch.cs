using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Cache
{
	/// <summary>
	/// An abstract batch used for implementing a batch operation of <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	internal abstract partial class AbstractCacheBatch
	{
		public AbstractCacheBatch(ISessionImplementor session, ICacheConcurrencyStrategy cacheConcurrencyStrategy)
		{
			Session = session;
			CacheConcurrencyStrategy = cacheConcurrencyStrategy;
		}

		protected ISessionImplementor Session { get; }

		public ICacheConcurrencyStrategy CacheConcurrencyStrategy { get; }

		public abstract int BatchSize { get; }

		public abstract void Execute();
	}

	/// <summary>
	/// An abstract batch used for implementing a batch operation of <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	internal abstract partial class AbstractCacheBatch<TData> : AbstractCacheBatch
	{
		private List<TData> _batch = new List<TData>();

		public AbstractCacheBatch(ISessionImplementor session, ICacheConcurrencyStrategy cacheConcurrencyStrategy)
			: base(session, cacheConcurrencyStrategy)
		{
		}

		public void Add(TData data)
		{
			_batch.Add(data);
		}

		public override int BatchSize => _batch.Count;

		public override sealed void Execute()
		{
			Execute(_batch.ToArray());
		}

		protected abstract void Execute(TData[] data);
	}
}
