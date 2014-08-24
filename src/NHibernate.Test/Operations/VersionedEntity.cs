using System.Collections.Generic;

namespace NHibernate.Test.Operations
{
	public class VersionedEntity
	{
		public VersionedEntity()
		{
			Children = new HashSet<VersionedEntity>();
		}

		public virtual string Id { get; set; }
		public virtual string Name { get; set; }
		public virtual long Version { get; set; }
		public virtual VersionedEntity Parent { get; set; }
		public virtual ISet<VersionedEntity> Children { get; set; }
	}
}