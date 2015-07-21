namespace NHibernate.Test.NHSpecificTest.NH1714
{
	public class LogClass
	{
		private byte[] byteData;
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public byte[] ByteData
		{
			get { return byteData; }
			set { byteData = value; }
		}
	}
}