namespace NHibernate.DomainModel
{
	public class Part
	{
		private long _id;
		private string _description;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public class SpecialPart : Part { }
	}
}
