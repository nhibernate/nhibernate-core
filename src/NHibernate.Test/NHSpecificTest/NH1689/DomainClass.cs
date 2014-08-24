

namespace NHibernate.Test.NHSpecificTest.NH1689
{
	using System.Collections.Generic;

	public class DomainClass
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual IList<TargetType> GetListOfTargetType<TargetType>(string someArg)
		{
			return new List<TargetType>();
		}
	}
}
