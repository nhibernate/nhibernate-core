using System.Collections.Generic;
using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class MainEntity : IMainEntity
	{
		private ISet<IPropertyEntityBase> _properties = new HashSet<IPropertyEntityBase>();
		private ISet<ISeparateEntity> _separateEntities = new HashSet<ISeparateEntity>();

		public virtual int MainEntityId { get; set; }
		public virtual string Text { get; set; }

		public virtual ISet<IPropertyEntityBase> Properties
		{
			get { return _properties; }
			protected set { _properties = value ?? new HashSet<IPropertyEntityBase>(); }
		}

		public virtual ISet<ISeparateEntity> SeparateEntities
		{
			get { return _separateEntities; }
			protected set { _separateEntities = value ?? new HashSet<ISeparateEntity>(); }
		}
	}
}
