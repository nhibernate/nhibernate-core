namespace NHibernate.Validator.Tests
{
	using System;

	public class Building
	{
		[Length(Min = 1, 
			Message = "{notpresent.Key} and #{key.notPresent} and {key.notPresent2} {min}")] 
		private String address;

		private long id;

		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		public long Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}