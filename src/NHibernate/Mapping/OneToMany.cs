using System;
using NHibernate.Type;

namespace NHibernate.Mapping 
{
	public class OneToMany 
	{
		private EntityType type;
		private Table referencingTable;

		public EntityType Type 
		{
			get { return type; }
			set { type = value; }
		}

		public OneToMany(PersistentClass owner) 
		{
			this.referencingTable = (owner==null) ? null : owner.Table;
		}

		public Table ReferencingTable 
		{
			get { return referencingTable; }
		}
	}
}
