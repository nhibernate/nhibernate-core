using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class OneToMany
	{
		private EntityType type;
		private Table referencingTable;

		/// <summary></summary>
		public EntityType Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public OneToMany( PersistentClass owner )
		{
			this.referencingTable = ( owner == null ) ? null : owner.Table;
		}

		/// <summary></summary>
		public Table ReferencingTable
		{
			get { return referencingTable; }
		}
	}
}