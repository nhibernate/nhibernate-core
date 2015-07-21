namespace NHibernate.Test.Extendshbm
{
	public class Person
	{
		private long id;
		private string name;
		private char sex;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual char Sex
		{
			get { return sex; }
			set { sex = value; }
		}
	}
}