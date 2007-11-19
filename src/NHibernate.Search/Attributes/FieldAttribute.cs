using System;
using Lucene.Net.Documents;

namespace NHibernate.Search.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class FieldAttribute: Attribute
	{
		private string name = null;
		private Store store = Store.No;
		private Index index;

		public FieldAttribute(Index index)
		{
			this.index = index;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Store Store
		{
			get { return store; }
			set { store = value; }
		}

		public Index Index
		{
			get { return index; }
		}
	}
}