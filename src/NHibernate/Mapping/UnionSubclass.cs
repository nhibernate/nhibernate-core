using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

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

		public override ISet<string> SynchronizedTables
		{
			get { return synchronizedTables; }
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