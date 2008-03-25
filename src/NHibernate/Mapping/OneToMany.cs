using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A mapping for a <c>one-to-many</c> association.
	/// </summary>
	[Serializable]
	public class OneToMany : IValue
	{
		private string referencedEntityName;
		private readonly Table referencingTable;
		private PersistentClass associatedClass;
		private bool ignoreNotFound;
		private bool embedded;

		public OneToMany(PersistentClass owner)
		{
			referencingTable = (owner == null) ? null : owner.Table;
		}

		private EntityType EntityType
		{
			get { return TypeFactory.ManyToOne(ReferencedEntityName, null, false, false, IsEmbedded, IsIgnoreNotFound); }
		}

		public bool IsIgnoreNotFound
		{
			get { return ignoreNotFound; }
			set { ignoreNotFound = value; }
		}

		/// <summary></summary>
		public IEnumerable<ISelectable> ColumnIterator
		{
			get { return associatedClass.Key.ColumnIterator; }
		}

		/// <summary></summary>
		public int ColumnSpan
		{
			get { return associatedClass.Key.ColumnSpan; }
		}

		public string ReferencedEntityName
		{
			get { return referencedEntityName; }
			set { referencedEntityName = value == null ? null : string.Intern(value); }
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
		public bool IsValid(IMapping mapping)
		{
			if (referencedEntityName == null)
			{
				throw new MappingException("one to many association must specify the referenced entity");
			}
			return true;
		}

		/// <summary></summary>
		public FetchMode FetchMode
		{
			get { return FetchMode.Join; }
		}

		#region IValue Members

		public bool IsAlternateUniqueKey
		{
			get { return false; }
		}

		public void SetTypeUsingReflection(string className, string propertyName, string accesorName)
		{
		}

		public object Accept(IValueVisitor visitor)
		{
			return visitor.Accept(this);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>No foreign key element for a one-to-many</remarks>
		public void CreateForeignKey()
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

		public bool IsEmbedded
		{
			get { return embedded; }
			set { embedded = value; }
		}
	}
}