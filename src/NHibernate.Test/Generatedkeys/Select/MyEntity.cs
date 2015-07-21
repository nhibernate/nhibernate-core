namespace NHibernate.Test.Generatedkeys.Select
{
	public class MyEntity
	{
		private int id;
		private string name;
		protected MyEntity() {}

		public MyEntity(string name)
		{
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}