using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public class Square : Shape, ISquare
	{
		public override string Property2 { get; set; }
		public virtual string Property3 { get; set; }
	}
}
