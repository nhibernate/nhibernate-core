using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class PropertyEntityA : PropertyEntityBase, IPropertyEntityA
	{
		public virtual int SerialNumber { get; set; }
	}
}
