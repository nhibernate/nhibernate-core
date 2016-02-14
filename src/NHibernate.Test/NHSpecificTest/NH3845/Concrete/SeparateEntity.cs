using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class SeparateEntity : ISeparateEntity
	{
		public virtual int SeparateEntityId { get; set; }
		public virtual IMainEntity MainEntity { get; set; }
		public virtual int SeparateEntityValue { get; set; }
	}
}
