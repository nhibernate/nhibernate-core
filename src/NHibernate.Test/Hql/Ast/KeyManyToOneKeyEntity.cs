namespace NHibernate.Test.Hql.Ast
{
	public class KeyManyToOneKeyEntity
	{
		private long id;
		private string name;
		private int? requestedHash;

		protected KeyManyToOneKeyEntity() {}

		public KeyManyToOneKeyEntity(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as KeyManyToOneKeyEntity);
		}

		public virtual bool Equals(KeyManyToOneKeyEntity other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.id == id && Equals(other.name, name);
		}

		public override int GetHashCode()
		{
			if(!requestedHash.HasValue)
			{
				unchecked
				{
					requestedHash = (id.GetHashCode() * 397) ^ (name != null ? name.GetHashCode() : 0);
				}
			}
			return requestedHash.Value;
		}
	}
}