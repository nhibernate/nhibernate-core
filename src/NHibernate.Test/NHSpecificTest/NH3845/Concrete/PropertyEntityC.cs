using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class PropertyEntityC : PropertyEntityBase, IPropertyEntityC
	{
		public virtual string Description { get; set; }
		public virtual int AnotherNumber { get; set; }
		public virtual IAnotherEntity AnotherEntity { get; set; }
	}
}
