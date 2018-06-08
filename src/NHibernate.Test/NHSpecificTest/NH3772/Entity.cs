using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3772
{
	public class Entity
	{
		private ICollection<SubEntity> _subEntities;
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<SubEntity> SubEntities
		{
			get => _subEntities ?? (_subEntities = new HashSet<SubEntity>());
			set => _subEntities = value;
		}
	}

	public class SubEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
