namespace NHibernate.Test.NHSpecificTest.NH1274ExportExclude
{
	public class Person
	{
		private int id;
		private int iq;
		private int shoeSize;
		private string name;
		private Home_Drop home_drop;
		private Home_Export home_export;
		private Home_Validate home_validate;
		private Home_Update home_update;

		public Person()
		{

		}

		public Person(string name, int iq, int shoeSize)
		{
			this.name = name;
			this.iq = iq;
			this.shoeSize = shoeSize;
		}

		virtual public int Id
		{
			get { return id; }
			set { id = value; }
		}

		virtual public string Name
		{
			get { return name; }
			set { name = value; }
		}

		virtual public int IQ
		{
			get { return iq; }
			set { iq = value; }
		}

		virtual public int ShoeSize
		{
			get { return shoeSize; }
			set { shoeSize = value; }
		}

		virtual public Home_Drop Home_Drop
		{
			get { return home_drop; }
			set { home_drop = value; }
		}

		virtual public Home_Export Home_Export
		{
			get { return home_export; }
			set { home_export = value; }
		}

		virtual public Home_Validate Home_Validate
		{
			get { return home_validate; }
			set { home_validate = value; }
		}

		virtual public Home_Update Home_Update
		{
			get { return home_update; }
			set { home_update = value; }
		}

	}
}