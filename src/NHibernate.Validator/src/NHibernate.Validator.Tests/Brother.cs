namespace NHibernate.Validator.Tests
{
	using System;

	public class Brother
	{
		[NotNull, Valid] private Address address;

		private Brother elder;
		private String name;
		private Brother youngerBrother;

		public Address Address
		{
			get { return address; }
			set { address = value; }
		}

		[NotNull]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Valid]
		public Brother Elder
		{
			get { return elder; }
			set { elder = value; }
		}

		[Valid]
		public Brother YoungerBrother
		{
			get { return youngerBrother; }
			set { youngerBrother = value; }
		}

		public override bool Equals(object obj)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 5;
		}
	}
}