using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	public enum Something
	{
		A,B,C,D,E,F
	}

	public class SimpleWithEnums
	{
		public virtual Something Something { get; set; }
		public virtual IList<Something> Things { get; set; }
	}
}