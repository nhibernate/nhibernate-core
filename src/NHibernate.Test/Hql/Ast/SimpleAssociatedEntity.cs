namespace NHibernate.Test.Hql.Ast
{
	public class SimpleAssociatedEntity
	{
		private long id;
		private string name;
		private int? requestedHash;
		private SimpleEntityWithAssociation owner;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual SimpleEntityWithAssociation Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		public virtual void BindToOwner(SimpleEntityWithAssociation owner)
		{
			if (owner != this.owner)
			{
				UnbindFromCurrentOwner();
			}
			this.owner = owner;
			if (owner != null)
			{
				owner.AssociatedEntities.Add(this);
			}
		}

		public virtual void UnbindFromCurrentOwner()
		{
			if (owner != null)
			{
				owner.AssociatedEntities.Remove(this);
				owner = null;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SimpleAssociatedEntity);
		}

		public virtual bool Equals(SimpleAssociatedEntity other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Id, Id);
		}

		public override int GetHashCode()
		{
			if (!requestedHash.HasValue)
			{
				requestedHash = Id.GetHashCode();
			}
			return requestedHash.Value;
		}
	}
}