using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2394
{
	public interface IA
	{
		int? Id { get; set; }
		TypeOfA Type { get; set; }
		TypeOfA? NullableType { get; set; }
		PhoneNumber Phone { get; set; }
		bool IsNice { get; set; }
	}

	public class A : IA
	{
		public int? Id { get; set; }
		public TypeOfA Type { get; set; }
		public TypeOfA? NullableType { get; set; }
		public PhoneNumber Phone { get; set; }
		public bool IsNice { get; set; }
	}

	public enum TypeOfA
	{
		Awesome,
		Boring,
		Cool
	}
}
