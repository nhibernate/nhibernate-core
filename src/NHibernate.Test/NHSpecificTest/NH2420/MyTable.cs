using System;

namespace NHibernate.Test.NHSpecificTest.NH2420
{
	public class MyTable 
	{
		public virtual int Id { get; protected set; }
		public virtual string String { get; set; }
	}
}