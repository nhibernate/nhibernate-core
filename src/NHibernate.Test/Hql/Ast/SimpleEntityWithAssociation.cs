using System;
using System.Collections.Generic;

namespace NHibernate.Test.Hql.Ast
{
	public class SimpleEntityWithAssociation
	{
		private long id;
		private string name;
		private ISet<SimpleAssociatedEntity> associatedEntities = new HashSet<SimpleAssociatedEntity>();
		private ISet<SimpleEntityWithAssociation> manyToManyAssociatedEntities = new HashSet<SimpleEntityWithAssociation>();

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

		public virtual ISet<SimpleAssociatedEntity> AssociatedEntities
		{
			get { return associatedEntities; }
			set { associatedEntities = value; }
		}

		public virtual ISet<SimpleEntityWithAssociation> ManyToManyAssociatedEntities
		{
			get { return manyToManyAssociatedEntities; }
			set { manyToManyAssociatedEntities = value; }
		}

		public virtual SimpleAssociatedEntity AddAssociation(string aName)
		{
			var result = new SimpleAssociatedEntity {Name = aName, Owner = this};
			AddAssociation(result);
			return result;
		}

		public virtual void AddAssociation(SimpleAssociatedEntity association)
		{
			association.BindToOwner(this);
		}

		public virtual void RemoveAssociation(SimpleAssociatedEntity association)
		{
			if (AssociatedEntities.Contains(association))
			{
				association.UnbindFromCurrentOwner();
			}
			else
			{
				throw new ArgumentException("SimpleAssociatedEntity [" + association + "] not currently bound to this [" + this + "]");
			}
		}
	}
}