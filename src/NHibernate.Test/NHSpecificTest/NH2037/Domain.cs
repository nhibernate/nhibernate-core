namespace NHibernate.Test.NHSpecificTest.NH2037
{
	public class Country
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual bool Equals(Country other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id == Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!(obj is Country)) return false;
			return Equals((Country) obj);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}

	public class City
	{
		public virtual int Id { get; set; }
		public virtual Country Country { get; set; }
		public virtual int CityCode { get; set; }
		public virtual string Name { get; set; }
	}
}
