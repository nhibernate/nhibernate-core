using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A mapping for a <c>one-to-many</c> association.
	/// </summary>
	public class OneToMany : IValue
	{
		private System.Type referencedEntityName;
		private Table referencingTable;
		private PersistentClass associatedClass;
		private bool ignoreNotFound;

		public OneToMany( PersistentClass owner )
		{
			this.referencingTable = ( owner == null ) ? null : owner.Table;
		}

		public EntityType EntityType
		{
			get { return TypeFactory.ManyToOne(referencedEntityName, null, ignoreNotFound); }
		}

		public bool IsIgnoreNotFound
		{
			get { return ignoreNotFound; }
			set { ignoreNotFound = value; }
		}

		/// <summary></summary>
		public ICollection ColumnCollection
		{
			get { return associatedClass.Key.ColumnCollection; }
		}

		/// <summary></summary>
		public int ColumnSpan
		{
			get { return associatedClass.Key.ColumnSpan; }
		}

		public System.Type ReferencedEntityName
		{
			get { return referencedEntityName; }
			set { referencedEntityName = value; }
		}

		public Table ReferencingTable
		{
			get { return referencingTable; }
		}

		public IType Type
		{
			get { return EntityType; }
		}

		/// <summary></summary>
		public PersistentClass AssociatedClass
		{
			get { return associatedClass; }
			set { associatedClass = value; }
		}

		/// <summary></summary>
		public Formula Formula
		{
			get { return null; }
		}

		/// <summary></summary>
		public Table Table
		{
			get { return referencingTable; }
		}

		/// <summary></summary>
		public bool IsNullable
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsSimpleValue
		{
			get { return false; }
		}

		public bool HasFormula
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsUnique
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsValid( IMapping mapping )
		{
			return true;
		}

		/// <summary></summary>
		public FetchMode FetchMode
		{
			get { return FetchMode.Join; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>No foreign key element for a one-to-many</remarks>
		public void CreateForeignKey( )
		{
		}

		public bool[] ColumnInsertability
		{
			get { throw new InvalidOperationException(); }
		}

		public bool[] ColumnUpdateability
		{
			get { throw new InvalidOperationException(); }
		}
	}
}
