using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class PropertyEntityB : PropertyEntityBase, IPropertyEntityB
	{
		public virtual string Description { get; set; }
		public virtual string AnotherString { get; set; }
	}
}
