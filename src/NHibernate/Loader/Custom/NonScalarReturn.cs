using System;

namespace NHibernate.Loader.Custom
{
	public abstract class NonScalarReturn : IReturn
	{
		private readonly string alias;
		private readonly LockMode lockMode;

		public NonScalarReturn(string alias, LockMode lockMode)
		{
			this.alias = alias;
			if (alias == null)
			{
				throw new HibernateException("alias must be specified");
			}
			this.lockMode = lockMode;
		}

		public string Alias
		{
			get { return alias; }
		}

		public LockMode LockMode
		{
			get { return lockMode; }
		}
	}
}