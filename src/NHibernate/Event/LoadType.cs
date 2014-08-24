using System;
namespace NHibernate.Event
{
	public sealed class LoadType
	{
		private readonly string name;
		private bool nakedEntityReturned;
		private bool allowNulls;
		private bool checkDeleted;
		private bool allowProxyCreation;
		private bool exactPersister;

		internal LoadType(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public bool IsAllowNulls
		{
			get { return allowNulls; }
		}

		internal LoadType SetAllowNulls(bool allowNulls)
		{
			this.allowNulls = allowNulls;
			return this;
		}

		public bool IsNakedEntityReturned
		{
			get { return nakedEntityReturned; }
		}

		internal LoadType SetNakedEntityReturned(bool immediateLoad)
		{
			nakedEntityReturned = immediateLoad;
			return this;
		}

		public bool IsCheckDeleted
		{
			get { return checkDeleted; }
		}

		internal LoadType SetCheckDeleted(bool checkDeleted)
		{
			this.checkDeleted = checkDeleted;
			return this;
		}

		public bool IsAllowProxyCreation
		{
			get { return allowProxyCreation; }
		}

		internal LoadType SetAllowProxyCreation(bool allowProxyCreation)
		{
			this.allowProxyCreation = allowProxyCreation;
			return this;
		}

		public bool ExactPersister
		{
			// NH Specific : NH-295 Allow strongly typed from cache
			// so far we are only use if for session.Get
			get { return exactPersister; }
		}

		internal LoadType SetExactPersister(bool exactPersister)
		{
			// NH Specific : NH-295 Allow strongly typed from cache
			this.exactPersister = exactPersister;
			return this;
		}

		public override string ToString()
		{
			return name;
		}
	}
}