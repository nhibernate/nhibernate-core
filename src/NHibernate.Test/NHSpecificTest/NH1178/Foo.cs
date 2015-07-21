namespace NHibernate.Test.NHSpecificTest.NH1178
{
	public class Foo
	{
		private int id;
		private int number;
		private string word;

		public Foo()
		{
		}

		public Foo(int number, string word)
		{
			this.number = number;
			this.word = word;
		}

		public int Number
		{
			get { return number; }
			set { number = value; }
		}

		public string Word
		{
			get { return word; }
			set { word = value; }
		}

		public int Id
		{
			set { id = value; }
			get { return id; }
		}
	}
}