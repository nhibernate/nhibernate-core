using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public abstract class PropertyEntityBase : IPropertyEntityBase
	{
		public virtual int PropertyEntityBaseId { get; set; }
		public virtual string Name { get; set; }
		public virtual string SharedValue { get; set; }
		public virtual IMainEntity MainEntity { get; set; }
	}
}
