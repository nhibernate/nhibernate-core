using System;

namespace NHibernate.Test.NHSpecificTest.GH3215
{
	class Member
	{
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime Date { get; set; }
	}

	class Request
	{
		public virtual Guid Id { get; set; }
		public virtual Member Member { get; set; }
	}
}
