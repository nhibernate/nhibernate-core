using System;

namespace NHibernate.Test.NHSpecificTest.GH1062
{
	public class MyEntity
	{
		public virtual int Id { get; protected set; }

		public virtual string Name { get; set; }
	}
}
