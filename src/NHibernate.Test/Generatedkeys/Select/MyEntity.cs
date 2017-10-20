namespace NHibernate.Test.Generatedkeys.Select
{
	public class MyEntity
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private int id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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