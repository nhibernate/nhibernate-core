using System;

namespace NHibernate.Test.TypesTest
{
	public class ChangeDefaultTypeClass
	{
		public int Id { get; set; }

		public DateTime NormalDateTimeValue { get; set; } = DateTime.Today;

		public string StringTypeLengthInType25 { get; set; }
		public string StringTypeExplicitLength20 { get; set; }
		public decimal CurrencyTypePrecisionInType5And2 { get; set; }
		public decimal CurrencyTypeExplicitPrecision6And3 { get; set; }
	}
}
