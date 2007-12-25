namespace NHibernate.Validator.Tests
{
	public class Address
	{
		[NotNull]
		public static string blacklistedZipCode;

		[Length(Max = 20), NotNull] 
		private string country;

		[Range(Min = -2, Max = 50, Message = "{floor.out.of.range} (escaping #{el})")] 
		public int floor;

		private long id;
		private bool internalValid = true;
		private string line1;
		private string line2;
		private string state;
		private string zip;

		[Min(1), Range(Max = 2000)]
		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Country
		{
			get { return country; }
			set { country = value; }
		}

		[NotNull]
		public string Line1
		{
			get { return line1; }
			set { line1 = value; }
		}

		[Length(Max = 3), NotNull]
		public string State
		{
			get { return state; }
			set { state = value; }
		}

		[Length(Max = 5, Message = "{long}")]
		[Pattern(Regex = "[0-9]+")]
		[NotNull]
		public string Zip
		{
			get { return zip; }
			set { zip = value; }
		}

		public string Line2
		{
			get { return line2; }
			set { line2 = value; }
		}

		[AssertTrue]
		public bool InternalValid
		{
			get { return internalValid; }
			set { internalValid = value; }
		}
	}
}