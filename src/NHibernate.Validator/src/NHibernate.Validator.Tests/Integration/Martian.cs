namespace NHibernate.Validator.Tests.Integration
{
	public class Martian
	{
		private MarsAddress address;
		private MartianPk id;

		public MarsAddress Address
		{
			get { return address; }
			set { address = value; }
		}

		public MartianPk Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}