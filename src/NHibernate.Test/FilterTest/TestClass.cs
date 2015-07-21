namespace NHibernate.Test.FilterTest
{
	public class TestClass
	{
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private bool _live;

		public bool Live
		{
			get { return _live; }
			set { _live = value; }
		}
	}
}