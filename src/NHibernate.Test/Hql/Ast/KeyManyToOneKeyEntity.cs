namespace NHibernate.Test.Hql.Ast
{
	public class KeyManyToOneKeyEntity
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private long id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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