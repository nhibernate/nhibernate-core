using System;

namespace NHibernate.Test.NHSpecificTest.NH266
{
	public class C : A
	{
		private char _code;

		public char Code
		{
			get { return _code; }
			set { _code = value; }
		}
	}
}
