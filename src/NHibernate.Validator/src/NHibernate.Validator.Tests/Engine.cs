namespace NHibernate.Validator.Tests
{
	public class Engine
	{
		private long horsePower;

		//TODO: Aggregate Annotations.
		//[Patterns(
		//    new Pattern[]{  
		//        Pattern(Regex = "^[A-Z0-9-]+$", Message = "must contain alphabetical characters only"),
		//        Pattern( Regex = "^....-....-....$", Message = "must match ....-....-....")
		//    })] 
		private string serialNumber;
		public string SerialNumber
		{
			get { return serialNumber; }
			set { serialNumber = value; }
		}

		public long HorsePower
		{
			get { return horsePower; }
			set { horsePower = value; }
		}
	}
}