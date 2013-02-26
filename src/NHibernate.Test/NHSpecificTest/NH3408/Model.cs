namespace NHibernate.Test.NHSpecificTest.NH3408
{
	using System;

	public class Country
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual byte[] Picture { get; set; }

		public virtual DateTime[] NationalHolidays { get; set; }
	}
}