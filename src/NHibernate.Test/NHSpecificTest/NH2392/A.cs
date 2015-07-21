using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2392
{
	public class A
	{
		public int? Id { get; set; }
		public string StringData1 { get; set; }
		public PhoneNumber MyPhone { get; set; }
		public string StringData2 { get; set; }
	}
}
