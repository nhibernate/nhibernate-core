namespace NHibernate.Loader.Custom
{
	/// <summary> Represents some non-scalar (entity/collection) return within the query result. </summary>
	public abstract class NonScalarReturn : IReturn
	{
		private readonly string alias;
		private readonly LockMode lockMode;

		public NonScalarReturn(string alias, LockMode lockMode)
		{
			this.alias = alias;
			if (string.IsNullOrEmpty(alias))
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