namespace NHibernate.Test.NHSpecificTest.NH1612
{
	public class City : Area
	{
		public virtual Country Country { get; protected set; }

		protected City() {}

		public City(string code, string name) : base(code, name) {}

		public virtual void SetParent(Country country)
		{
			if (Country != null)
			{
				Country.Cities.Remove(this);
			}
			Country = country;
			if (country != null)
			{
				country.Cities.Add(this);
			}
		}
	}
}