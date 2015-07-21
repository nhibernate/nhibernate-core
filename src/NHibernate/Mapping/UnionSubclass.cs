using System;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	[Serializable]
	public class UnionSubclass : Subclass, ITableOwner
	{
		private Table table;

		public UnionSubclass(PersistentClass superclass)
			: base(superclass)
		{
		}

		#region ITableOwner Members

		Table ITableOwner.Table
		{
			set
			{
				table = value;
				Superclass.AddSubclassTable(table);
			}
		}

		#endregion

		public override Table Table
		{
			get { return table; }
		}

		protected internal override IEnumerable<Property> NonDuplicatedPropertyIterator
		{
			get { return PropertyClosureIterator; }
		}

		public override Table IdentityTable
		{
			get { return Table; }
		}
	}
}