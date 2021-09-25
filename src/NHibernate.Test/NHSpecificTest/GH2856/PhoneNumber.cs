namespace NHibernate.Test.NHSpecificTest.GH2856
{
	public class PhoneNumber
	{
		public PhoneNumber(string number, string ext = null)
		{
			Number = number;
			Ext = ext;
		}

		public string Number { get; }

		public string Ext { get; }
	}
}
