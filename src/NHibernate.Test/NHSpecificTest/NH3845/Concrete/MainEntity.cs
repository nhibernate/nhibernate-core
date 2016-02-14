using System.Collections.Generic;
using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class MainEntity : IMainEntity
	{
		public virtual int MainEntityId { get; set; }
		public virtual string Text { get; set; }
		public virtual ISet<IPropertyEntityBase> Properties { get; protected set; } = new HashSet<IPropertyEntityBase>();
		public virtual ISet<ISeparateEntity> SeparateEntities { get; protected set; } = new HashSet<ISeparateEntity>();
	}
}
