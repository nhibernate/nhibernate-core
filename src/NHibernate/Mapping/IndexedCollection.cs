using System;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Indexed collections include IList, IDictionary, Arrays
	/// and primitive Arrays.
	/// </summary>
	[Serializable]
	public abstract class IndexedCollection : Collection
	{
		public const string DefaultIndexColumnName = "idx";

		private SimpleValue index;
		private string indexNodeName;

		protected IndexedCollection(PersistentClass owner) : base(owner)
		{
		}

		public SimpleValue Index
		{
			get { return index; }
			set { index = value; }
		}

		public override bool IsIndexed
		{
			get { return true; }
		}

		public virtual bool IsList
		{
			get { return false; }
		}

		public string IndexNodeName
		{
			get { return indexNodeName; }
			set { indexNodeName = value; }
		}

		public override void CreatePrimaryKey()
		{
			if (!IsOneToMany)
			{
				PrimaryKey pk = new PrimaryKey();
				pk.AddColumns(new SafetyEnumerable<Column>(Key.ColumnIterator));

				// index should be last column listed
				bool isFormula = false;
				foreach (ISelectable selectable in Index.ColumnIterator)
				{
					if(selectable.IsFormula)
						isFormula = true;
				}
				if (isFormula)
				{
					//if it is a formula index, use the element columns in the PK
					pk.AddColumns(new SafetyEnumerable<Column>(Element.ColumnIterator));
				}
				else
				{
					pk.AddColumns(new SafetyEnumerable<Column>(Index.ColumnIterator));
				}

				CollectionTable.PrimaryKey = pk;
			}
		}

		public override void Validate(IMapping mapping)
		{
			base.Validate(mapping);
			if (!Index.IsValid(mapping))
			{
				throw new MappingException(
					string.Format("collection index mapping has wrong number of columns: {0} type: {1}", Role, Index.Type.Name));
			}
			if (indexNodeName != null && !indexNodeName.StartsWith("@"))
			{
				throw new MappingException("index node must be an attribute: " + indexNodeName);
			}
		}
	}
}
