namespace NHibernate.Test.Hql.Ast
{
	public class CrazyCompositeKey
	{
		private long id;
		private long otherId;
		private int? requestedHash;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual long OtherId
		{
			get { return otherId; }
			set { otherId = value; }
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CrazyCompositeKey);
		}

		public virtual bool Equals(CrazyCompositeKey other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.id == id && other.otherId == otherId;
		}

		public override int GetHashCode()
		{
			if (!requestedHash.HasValue)
			{
				unchecked
				{
					requestedHash = (id.GetHashCode() * 397) ^ otherId.GetHashCode();
				}
			}
			return requestedHash.Value;
		}
	}
}