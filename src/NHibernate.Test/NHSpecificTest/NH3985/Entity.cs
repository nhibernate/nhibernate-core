using System;

namespace NHibernate.Test.NHSpecificTest.NH3985
{
	public partial class Process
	{
		public virtual Guid ProcessID { get; set; }
		public virtual string Name { get; set; }
	}
}
