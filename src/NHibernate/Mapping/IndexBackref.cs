using System;
using NHibernate.Properties;

namespace NHibernate.Mapping
{
	[Serializable]
	public class IndexBackref : Property
	{
		private string collectionRole;
		private string entityName;

		public string CollectionRole
		{
			get { return collectionRole; }
			set { collectionRole = value; }
		}

		public string EntityName
		{
			get { return entityName; }
			set { entityName = value; }
		}

		public override bool BackRef
		{
			get { return true; }
		}

		public override bool IsBasicPropertyAccessor
		{
			get { return false; }
		}

		protected override IPropertyAccessor PropertyAccessor
		{
			get { return new IndexPropertyAccessor(collectionRole, entityName); }
		}
	}
}
