using Iesi.Collections;

namespace NHibernate.Test.HQL.Ast
{
	public class SimpleEntityWithAssociation
	{
		private long id;
		private string name;
		private ISet associatedEntities = new HashedSet();
		private ISet manyToManyAssociatedEntities = new HashedSet();

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

		public virtual ISet AssociatedEntities
		{
			get { return associatedEntities; }
			set { associatedEntities = value; }
		}

		public virtual ISet ManyToManyAssociatedEntities
		{
			get { return manyToManyAssociatedEntities; }
			set { manyToManyAssociatedEntities = value; }
		}
	}
}