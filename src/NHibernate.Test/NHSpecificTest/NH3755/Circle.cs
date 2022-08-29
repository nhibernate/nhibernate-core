using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public class Circle : Shape, ICircle
	{
		public override string Property2 { get; set; }
		public virtual string Property3 { get; set; }
	}
}
