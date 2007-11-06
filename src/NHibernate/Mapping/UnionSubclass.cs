using System;
using Iesi.Collections;

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

		public override ISet SynchronizedTables
		{
			get { return synchronizedTables; }
		}
	}
}