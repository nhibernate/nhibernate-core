namespace NHibernate.Test.NHSpecificTest.GH3539
{
	public class Person
	{
		private int _id;
		private int _age;
		private CardInfo _cardInfo;

		protected Person() { }

		public Person(int age)
		{
			_cardInfo = new();
			_age = age;
		}

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual int Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public virtual CardInfo CardInfo
		{
			get { return _cardInfo; }
			set { _cardInfo = value; }
		}
	}
}
