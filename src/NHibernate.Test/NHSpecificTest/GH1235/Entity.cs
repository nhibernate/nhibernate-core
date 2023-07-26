using System;

namespace NHibernate.Test.NHSpecificTest.GH1235
{
	class MultiTableEntity
	{
		public virtual int Id { get; set; }
		public virtual int Version { get; set; }
		public virtual string Name { get; set; }
		public virtual string OtherName { get; set; }
	}
}
