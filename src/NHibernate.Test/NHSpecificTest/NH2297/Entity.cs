using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2297
{
	public class EntityNH2297
	{
		public virtual int Id { get; set; }

		public virtual CustomCompositeUserType CustomTypeValue { get; set; }
	}
}
