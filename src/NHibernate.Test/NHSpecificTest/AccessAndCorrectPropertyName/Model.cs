namespace NHibernate.Test.NHSpecificTest.AccessAndCorrectPropertyName
{
	public class Person
	{
		private string _firstName;

		public virtual int Id { get; set; }

		public virtual string FiRsTNaMe
		{
			get { return _firstName; }
			set { _firstName = value; }
		}
	}

	public class Dog
	{
		private string name;

		public virtual int Id { get; set; }

		public virtual string xyz
		{
			get { return name; }
			set { name = value; }
		}
	}
}
